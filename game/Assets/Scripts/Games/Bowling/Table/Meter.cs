using System;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>How a meter shows its sweep.</summary>
    public enum MeterStyle
    {
        /// <summary>No track of its own: the aim's sweep is the gold arrow swinging on the lane.</summary>
        OnTheLane,

        /// <summary>A half-circle dial whose needle swings from straight to a big curve: the hook.</summary>
        Dial,

        /// <summary>A power bar that fills and drains, cool to hot: the speed.</summary>
        Power,
    }

    /// <summary>
    /// A sweep you stop with a tap: shared by aim, hook and speed. It runs out and back at a set
    /// rate; where you stop it is the input. Its value is 0 to 1; the lane turns that into boards,
    /// a hook or a speed. Each shows it its own way (<see cref="MeterStyle"/>), like the meters of
    /// a 90s console bowling game (Mike, 2026-11-24).
    /// </summary>
    public sealed class Meter : MonoBehaviour
    {
        static readonly Color Cool = new Color(0.36f, 0.79f, 0.82f);
        static readonly Color Hot = new Color(0.86f, 0.3f, 0.2f);

        Image _fill, _marker;
        HookGauge _dial;
        TMP_Text _value;
        Func<double, string> _describe;
        MeterStyle _style;
        double _phase, _rate;

        public Button Button { get; private set; }
        public bool Running { get; private set; }
        public double Value { get; private set; }

        /// <summary>A meter row: its label, its track (by style) and its reading; the whole row is the button.</summary>
        public static Meter Make(Transform parent, string label, MeterStyle style, Func<double, string> describe, float height = 92)
        {
            var button = MakeButton(parent, label, () => { });
            button.GetComponent<LayoutElement>().preferredHeight = height;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.fontSize = 34;
            text.rectTransform.anchorMax = new Vector2(0.24f, 1);

            var meter = button.gameObject.AddComponent<Meter>();
            meter.Button = button;
            meter._describe = describe;
            meter._style = style;

            if (style == MeterStyle.Dial)
            {
                var dialGo = new GameObject("Dial", typeof(RectTransform), typeof(CanvasRenderer), typeof(HookGauge));
                dialGo.transform.SetParent(button.transform, false);
                meter._dial = dialGo.GetComponent<HookGauge>();
                meter._dial.raycastTarget = false;
                Pin(meter._dial.rectTransform, new Vector2(0.3f, 0.06f), new Vector2(0.72f, 0.94f), Vector2.zero, Vector2.zero);
            }
            else if (style == MeterStyle.Power)
            {
                var track = MakeImage(button.transform, "Track", new Color(0, 0, 0, 0.55f));
                track.raycastTarget = false;
                Pin(track.rectTransform, new Vector2(0.25f, 0.2f), new Vector2(0.78f, 0.8f), Vector2.zero, Vector2.zero);
                for (int i = 1; i < 4; i++)
                {
                    var t = MakeImage(track.transform, "Tick", new Color(1, 1, 1, 0.3f));
                    t.raycastTarget = false;
                    Pin(t.rectTransform, new Vector2(i / 4f, 0), new Vector2(i / 4f, 1), new Vector2(-1.5f, 0), new Vector2(1.5f, 0));
                }
                meter._fill = MakeImage(track.transform, "Fill", Cool);
                meter._fill.raycastTarget = false;
                meter._marker = MakeImage(track.transform, "Marker", Gold);
                meter._marker.raycastTarget = false;
            }

            var value = MakeText(button.transform, "", 34, TextAnchor.MiddleRight, 0);
            value.name = "Reading";
            NoWrap(value);
            Pin(value.rectTransform, new Vector2(style == MeterStyle.OnTheLane ? 0.3f : 0.74f, 0), new Vector2(1, 1), Vector2.zero, new Vector2(-20, 0));
            meter._value = value;
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
            if (_style == MeterStyle.Power)
            {
                _fill.enabled = !blank;
                _fill.color = Color.Lerp(Cool, Hot, v);
                Pin(_fill.rectTransform, Vector2.zero, new Vector2(v, 1), Vector2.zero, Vector2.zero);
                _marker.enabled = !blank;
                Pin(_marker.rectTransform, new Vector2(v, -0.2f), new Vector2(v, 1.2f), new Vector2(-4, 0), new Vector2(4, 0));
            }
            if (_dial != null)
            {
                _dial.Value = blank ? (double?)null : Value;
                _dial.Redraw();
            }
            _value.text = blank ? (_style == MeterStyle.OnTheLane ? "Tap: the arrow swings, tap again to stop" : "") : _describe(Value);
            _value.alpha = blank ? 0.6f : 1f;
            _value.fontSize = blank ? 26 : 34;
        }
    }
}
