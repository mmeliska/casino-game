using System;
using System.Collections.Generic;
using System.IO;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Autosave as JSON files in one folder: the player state, and the Ink story state beside
    /// it. Only settled state is ever saved, so a phone session cut off mid-hand comes back to
    /// the table before that hand, with the bet still in the bankroll.
    ///
    /// The player save carries a <c>Version</c>; an older file is migrated step by step on the
    /// raw JSON before it's read (see <see cref="Migrate"/>), so a schema change never resets
    /// anyone. A file this code can't read at all starts the game fresh, with a warning.
    /// </summary>
    public sealed class SaveStore
    {
        readonly string _savePath;
        readonly string _storyPath;
        readonly string _snapshotsFolder;

        /// <summary>Saves under <paramref name="folder"/>, normally <c>Application.persistentDataPath</c>.</summary>
        public SaveStore(string folder)
        {
            _savePath = Path.Combine(folder, "save.json");
            _storyPath = Path.Combine(folder, "story.json");
            _snapshotsFolder = Path.Combine(folder, "snapshots");
        }

        public PlayerState Load()
        {
            if (!File.Exists(_savePath)) return new PlayerState();
            try
            {
                return Parse(File.ReadAllText(_savePath));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't read the save, starting fresh: {e.Message}");
                return new PlayerState();
            }
        }

        public void Save(PlayerState state) => Write(_savePath, JsonUtility.ToJson(state, true));

        /// <summary>The Ink story's saved state (flags, visit counts), or null on a new game.</summary>
        public string LoadStory() => File.Exists(_storyPath) ? File.ReadAllText(_storyPath) : null;

        public void SaveStory(string storyJson) => Write(_storyPath, storyJson);

        public void Wipe()
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
            if (File.Exists(_storyPath)) File.Delete(_storyPath);
        }

        /// <summary>
        /// Copies both files, as they are now, into a named snapshot for the debug panel. Snapshots
        /// sit apart from the save, so Start over (<see cref="Wipe"/>) leaves them.
        /// </summary>
        public void TakeSnapshot(string name)
        {
            string folder = SnapshotFolder(name);
            Directory.CreateDirectory(folder);
            CopyIfThere(_savePath, Path.Combine(folder, "save.json"));
            CopyIfThere(_storyPath, Path.Combine(folder, "story.json"));
        }

        /// <summary>Puts a snapshot's files back as the save. The next <see cref="Load"/> reads them, migrating as usual.</summary>
        public void RestoreSnapshot(string name)
        {
            string folder = SnapshotFolder(name);
            if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"No snapshot '{name}'.");
            Wipe();
            CopyIfThere(Path.Combine(folder, "save.json"), _savePath);
            CopyIfThere(Path.Combine(folder, "story.json"), _storyPath);
        }

        public void DeleteSnapshot(string name)
        {
            string folder = SnapshotFolder(name);
            if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
        }

        /// <summary>Snapshot names, last first when they start with the time, as the debug panel's do.</summary>
        public List<string> Snapshots()
        {
            var names = new List<string>();
            if (!Directory.Exists(_snapshotsFolder)) return names;
            foreach (string folder in Directory.GetDirectories(_snapshotsFolder)) names.Add(Path.GetFileName(folder));
            names.Sort(StringComparer.Ordinal);
            names.Reverse();
            return names;
        }

        string SnapshotFolder(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.StartsWith("."))
                throw new ArgumentException($"'{name}' can't be a snapshot name.", nameof(name));
            return Path.Combine(_snapshotsFolder, name);
        }

        static void CopyIfThere(string from, string to)
        {
            if (File.Exists(from)) Write(to, File.ReadAllText(from));
        }

        /// <summary>Reads a player save of any version this code has known. Throws if it can't.</summary>
        public static PlayerState Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new FormatException("The save is empty.");
            int version = JsonUtility.FromJson<VersionProbe>(json).Version;
            json = Migrate(json, version);
            var state = JsonUtility.FromJson<PlayerState>(json);
            if (state == null) throw new FormatException("The save isn't a player state.");
            state.Version = PlayerState.CurrentVersion;
            return state;
        }

        /// <summary>
        /// Brings a save forward one version at a time. Each step takes the raw JSON of version
        /// n and returns version n + 1, so a step still works after the C# type has moved on.
        /// </summary>
        static string Migrate(string json, int fromVersion)
        {
            if (fromVersion > PlayerState.CurrentVersion)
                throw new FormatException($"The save is version {fromVersion}; this build reads up to {PlayerState.CurrentVersion}.");
            for (int v = fromVersion; v < PlayerState.CurrentVersion; v++)
                json = Steps[v](json);
            return json;
        }

        /// <summary><c>Steps[n]</c> turns version n into n + 1.</summary>
        static readonly Func<string, string>[] Steps =
        {
            // 0 -> 1: the first saves had no Version field; the fields are otherwise the same.
            json => json,
            // 1 -> 2: the hidden scales arrive. A save without them reads them as level, all zero.
            json => json,
        };

        [Serializable]
        sealed class VersionProbe
        {
            public int Version;
        }

        static void Write(string path, string json)
        {
            // Write then swap, so a kill mid-write leaves the last good save.
            string temp = path + ".tmp";
            File.WriteAllText(temp, json);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);
        }
    }
}
