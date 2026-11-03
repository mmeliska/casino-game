using System.IO;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Autosave as JSON files: the player state, and the Ink story state beside it. Only
    /// settled state is ever saved, so a phone session cut off mid-hand comes back to the
    /// table before that hand, with the bet still in the bankroll.
    /// </summary>
    public static class SaveStore
    {
        static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
        static string StoryPath => Path.Combine(Application.persistentDataPath, "story.json");

        public static PlayerState Load()
        {
            if (!File.Exists(SavePath)) return new PlayerState();
            try
            {
                return JsonUtility.FromJson<PlayerState>(File.ReadAllText(SavePath)) ?? new PlayerState();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Couldn't read the save, starting fresh: {e.Message}");
                return new PlayerState();
            }
        }

        public static void Save(PlayerState state) => Write(SavePath, JsonUtility.ToJson(state, true));

        /// <summary>The Ink story's saved state (flags, visit counts), or null on a new game.</summary>
        public static string LoadStory() => File.Exists(StoryPath) ? File.ReadAllText(StoryPath) : null;

        public static void SaveStory(string storyJson) => Write(StoryPath, storyJson);

        public static void Wipe()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            if (File.Exists(StoryPath)) File.Delete(StoryPath);
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
