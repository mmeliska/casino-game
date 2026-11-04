using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// The standalone blackjack scene, kept for testing the table: it stands in for the city as
    /// the owner of the player state, loading and saving the same file and sitting you down.
    /// </summary>
    public sealed class BlackjackScene : MonoBehaviour
    {
        SaveStore _saves;
        PlayerState _state;
        BlackjackTable _table;

        void Awake()
        {
            _saves = new SaveStore(Application.persistentDataPath);
            _table = gameObject.AddComponent<BlackjackTable>();
            _table.StartOverRequested += StartOver;
            Take(_saves.Load());
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) Save();
        }

        void OnApplicationQuit() => Save();

        void OnDestroy()
        {
            if (_state != null) _state.Changed -= Save;
        }

        void Take(PlayerState state)
        {
            if (_state != null) _state.Changed -= Save;
            _state = state;
            _state.Changed += Save;
            _table.SitDown(_state);
        }

        void StartOver()
        {
            _saves.Wipe();
            Take(new PlayerState());
            Save();
        }

        void Save() => _saves.Save(_state);
    }
}
