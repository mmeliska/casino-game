using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Fades a group in, after an optional pause: a place's actions after you arrive, so the
    /// plate shows on its own first, or a line on black. Taps go through it only once it's in.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class Reveal : MonoBehaviour
    {
        const float MaxStep = 1f / 30f;

        CanvasGroup _group;
        float _wait, _seconds, _shown = 1f;

        void Awake() => _group = GetComponent<CanvasGroup>();

        public void Play(float seconds, float delay = 0f)
        {
            _seconds = seconds;
            _wait = delay;
            _shown = seconds > 0 || delay > 0 ? 0f : 1f;
            Apply();
        }

        void Update()
        {
            if (_shown >= 1f) return;
            float step = Mathf.Min(Time.unscaledDeltaTime, MaxStep);
            if (_wait > 0)
            {
                _wait -= step;
                return;
            }
            _shown = _seconds > 0 ? Mathf.Min(1f, _shown + step / _seconds) : 1f;
            Apply();
        }

        void Apply()
        {
            _group.alpha = Mathf.SmoothStep(0, 1, _shown);
            _group.blocksRaycasts = _shown >= 1f;
        }
    }
}
