using System.IO;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Autosave as one JSON file. Only settled state is ever saved, so a phone session cut
    /// off mid-hand comes back to the table before that hand, with the bet still in the bankroll.
    /// </summary>
    public static class SaveStore
    {
        static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

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

        public static void Save(PlayerState state)
        {
            // Write then swap, so a kill mid-write leaves the last good save.
            string temp = SavePath + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(state, true));
            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(temp, SavePath);
        }
    }
}
