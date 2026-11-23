using System;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// A sweep you stop with a tap: shared by aim, hook and speed. It runs out and back at a set
    /// rate; where you stop it is the input. Its value is 0 to 1; the lane turns that into boards,
    /// a hook or a speed.
    /// </summary>
    public sealed class Meter : MonoBehaviour
    {
        Image _fill, _marker;
        TMP_Text _value;
        Func<double, string> _describe;
        bool _fills;
        double _phase, _rate;

        public Button Button { get; private set; }
        public bool Running { get; private set; }
        public double Value { get; private set; }

        /// <summary>
        /// A meter row: its label, a track and its reading. A filling meter (hook, speed) grows
        /// from the left; the aim meter's marker moves, with ticks at the given values.
        /// </summary>
        public static Meter Make(Transform parent, string label, bool fills, Func<double, string> describe, params double[] ticks)
        {
            var button = MakeButton(parent, label, () => { });
            button.GetComponent<LayoutElement>().preferredHeight = 92;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.fontSize = 34;
            text.rectTransform.anchorMax = new Vector2(0.24f, 1);

            var track = MakeImage(button.transform, "Track", new Color(1, 1, 1, 0.12f));
            track.raycastTarget = false;
            Pin(track.rectTransform, new Vector2(0.25f, 0.3f), new Vector2(0.78f, 0.7f), Vector2.zero, Vector2.zero);
            foreach (double tick in ticks)
            {
                var t = MakeImage(track.transform, "Tick", new Color(1, 1, 1, 0.45f));
                t.raycastTarget = false;
                Pin(t.rectTransform, new Vector2((float)tick, 0), new Vector2((float)tick, 1), new Vector2(-2, 0), new Vector2(2, 0));
            }
            var fill = MakeImage(track.transform, "Fill", Teal);
            fill.raycastTarget = false;
            var marker = MakeImage(track.transform, "Marker", Gold);
            marker.raycastTarget = false;

            var value = MakeText(button.transform, "", 34, TextAnchor.MiddleRight, 0);
            value.name = "Reading";
            NoWrap(value);
            Pin(value.rectTransform, new Vector2(0.78f, 0), new Vector2(1, 1), Vector2.zero, new Vector2(-20, 0));

            var meter = button.gameObject.AddComponent<Meter>();
            meter.Button = button;
            meter._fill = fill;
            meter._marker = marker;
            meter._value = value;
            meter._describe = describe;
            meter._fills = fills;
            meter.Show();
            return meter;
        }

        /// <summary>Starts the sweep from empty at this many sweeps (empty to full) a second.</summary>
        public void Begin(double rate)
        {
            _rate = rate;
            _phase = 0;
            Value = 0;
            Running = true;
            Show();
        }

        /// <summary>Stops the sweep where it is; that's the reading.</summary>
        public double Stop()
        {
            Running = false;
            Show();
            return Value;
        }

        /// <summary>Shows a fixed reading, stopped: a coached frame's number, or the last throw's.</summary>
        public void Set(double value)
        {
            Running = false;
            Value = Math.Max(0, Math.Min(1, value));
            Show();
        }

        /// <summary>Clears the reading for the next throw.</summary>
        public void Clear()
        {
            Running = false;
            Value = 0;
            Show(blank: true);
        }

        void Update()
        {
            if (!Running) return;
            _phase = (_phase + Math.Min(Time.unscaledDeltaTime, 0.1f) * _rate) % 2;
            Value = _phase < 1 ? _phase : 2 - _phase;
            Show();
        }

        void Show(bool blank = false)
        {
            float v = (float)Value;
            _fill.enabled = _fills && !blank;
            Pin(_fill.rectTransform, Vector2.zero, new Vector2(v, 1), Vector2.zero, Vector2.zero);
            _marker.enabled = !blank;
            Pin(_marker.rectTransform, new Vector2(v, -0.25f), new Vector2(v, 1.25f), new Vector2(-5, 0), new Vector2(5, 0));
            _value.text = blank ? "" : _describe(Value);
        }
    }
}
