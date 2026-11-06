using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// A dip through black between screens, so nothing cuts. A quick dip when the screen
    /// changes and you stay put (a scene starts or ends, another view of the same place, sitting
    /// down at a table); a slower one when you go somewhere else. The swap happens while the
    /// screen is dark, and taps are held off until it's back. It starts dark, so the first
    /// screen fades in.
    /// </summary>
    public sealed class ScreenFader : MonoBehaviour
    {
        const float QuickOut = 0.15f, QuickIn = 0.2f;
        const float AwayOut = 0.3f, AwayIn = 0.45f;

        /// <summary>The longest step one frame takes, so a hitch (a plate loading) doesn't skip the fade.</summary>
        const float MaxStep = 1f / 30f;

        enum Phase { Idle, Out, In }

        GameObject _root;
        Image _black;
        Phase _phase = Phase.In;
        float _dark = 1f;
        float _outSeconds = AwayOut, _inSeconds = AwayIn;
        readonly List<Action> _swaps = new List<Action>();
        bool _swapping;

        public bool IsBusy => _phase != Phase.Idle;

        void Awake() => Build();

        /// <summary>
        /// Fades to black, runs <paramref name="swap"/>, and fades back. Asked while already dark
        /// (from inside another swap), it swaps at once; asked mid-fade, it joins the dip under way.
        /// </summary>
        public void Cover(Action swap, bool away = false)
        {
            if (swap == null) throw new ArgumentNullException(nameof(swap));
            if (_swapping)
            {
                swap();
                return;
            }
            if (_phase == Phase.Idle)
            {
                _outSeconds = away ? AwayOut : QuickOut;
                _inSeconds = away ? AwayIn : QuickIn;
            }
            else if (away)
            {
                _outSeconds = AwayOut;
                _inSeconds = AwayIn;
            }
            _swaps.Add(swap);
            _root.SetActive(true);
            if (_dark >= 1f) Swap();
            else _phase = Phase.Out;
        }

        void Swap()
        {
            _swapping = true;
            try
            {
                while (_swaps.Count > 0)
                {
                    var swap = _swaps[0];
                    _swaps.RemoveAt(0);
                    swap();
                }
            }
            finally
            {
                _swapping = false;
            }
            _phase = Phase.In;
        }

        void Update()
        {
            if (_phase == Phase.Idle) return;
            float step = Mathf.Min(Time.unscaledDeltaTime, MaxStep);
            if (_phase == Phase.Out)
            {
                _dark = Mathf.Min(1f, _dark + step / _outSeconds);
                if (_dark >= 1f) Swap();
            }
            else
            {
                _dark = Mathf.Max(0f, _dark - step / _inSeconds);
                if (_dark <= 0f)
                {
                    _phase = Phase.Idle;
                    _root.SetActive(false);
                }
            }
            _black.color = new Color(0, 0, 0, Mathf.SmoothStep(0, 1, _dark));
        }

        void Build()
        {
            // Over everything but the debug panel (50), so the swap is never seen.
            var canvas = MakeCanvas(transform, "Fader Canvas", 30);
            _root = canvas.gameObject;
            _black = MakeImage(canvas.transform, "Black", Color.black);
            Stretch(_black.rectTransform);
        }
    }
}
