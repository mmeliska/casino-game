using Starflare.State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Starflare.Game
{
    /// <summary>
    /// One playthrough, and the scene's root: it loads the save, owns the player state and the
    /// one Ink story (through <see cref="StoryBridge"/>), builds the <see cref="City"/> with them,
    /// and saves both on every settled change and at the end of every scene. It is the only
    /// thing that touches the <see cref="SaveStore"/>. Starting over is a new playthrough,
    /// booted from a wiped save. Editor and Development builds also get the debug panel
    /// (<c>Playthrough.Debug.cs</c>).
    /// </summary>
    public sealed partial class Playthrough : MonoBehaviour
    {
        /// <summary>Where the save files go; null means the device's persistent data path. Tests set it.</summary>
        public static string SaveFolder;

        public PlayerState State { get; private set; }
        public StoryBridge Story { get; private set; }
        public ClockPace Pace { get; } = new ClockPace();

        SaveStore _saves;
        City _city;

        void Awake()
        {
            _saves = new SaveStore(SaveFolder ?? Application.persistentDataPath);
            State = _saves.Load();
            // The playthrough is the one owner of the save: every settled change writes it.
            State.Changed += SaveAll;
            Story = StoryBridge.Load(_saves.LoadStory(), State);

            _city = gameObject.AddComponent<City>();
            _city.Init(State, Story, Pace);
            _city.SceneEnded += SaveAll;
            _city.StartOverRequested += StartOver;
            AddDebugPanel();
        }

        /// <summary>The debug panel, in Editor and Development builds only (<c>Playthrough.Debug.cs</c>).</summary>
        partial void AddDebugPanel();

        void Start() => _city.Begin();

        void OnApplicationPause(bool paused)
        {
            if (paused) SaveAll();
        }

        void OnApplicationQuit() => SaveAll();

        void OnDestroy() => State.Changed -= SaveAll;

        public void StartOver()
        {
            State.Changed -= SaveAll;
            _saves.Wipe();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>Both files together, so the story's flags and the cash they refer to agree.</summary>
        void SaveAll()
        {
            if (Story == null) return;
            _saves.Save(State);
            _saves.SaveStory(Story.SaveJson());
        }
    }
}
