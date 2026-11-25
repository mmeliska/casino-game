using System;
using UnityEngine;

namespace Starflare.Ui
{
    /// <summary>
    /// A swipe up from the bottom edge of the screen that pulls a sheet up under the finger: the
    /// debug panel's way in. It reads the pointer a frame at a time (screen pixels), so it never
    /// blocks a tap meant for the screen below; a press that something on screen would drag (the
    /// bowling ball, a scratcher, a list) is left to it.
    /// </summary>
    public sealed class EdgeSwipe
    {
        public enum Outcome
        {
            None,
            /// <summary>The finger is pulling the sheet; <see cref="Progress"/> says how far.</summary>
            Dragging,
            /// <summary>Let go still moving up, or far enough up: the sheet should finish opening.</summary>
            Open,
            /// <summary>Let go low after pulling back down: the sheet should drop away.</summary>
            Cancel,
        }

        /// <summary>
        /// How far up the sheet must be pulled, as a share of its height, to stay open when the
        /// finger was last moving down. A flick that's still going up opens it from anywhere.
        /// </summary>
        public const float OpenAt = 0.25f;

        /// <summary>How far the finger must rise, as a share of the screen's height, before the sheet moves.</summary>
        public const float StartRise = 0.02f;

        readonly Func<Vector2, bool> _taken;
        bool _wasPressed, _pressing, _dragging, _goingUp;
        Vector2 _start;
        float _lastY;

        /// <param name="taken">True when a press at that point belongs to something on screen.</param>
        public EdgeSwipe(Func<Vector2, bool> taken = null) => _taken = taken;

        /// <summary>How far up the sheet is, 0 (off the screen) to 1 (all the way up).</summary>
        public float Progress { get; private set; }

        public bool IsDragging => _dragging;

        /// <summary>
        /// One frame of the pointer. A swipe starts only on a fresh press at or below
        /// <paramref name="edgeTop"/>, so a finger already down that slides into the edge doesn't
        /// count, and only once it rises more than it moves sideways.
        /// </summary>
        public Outcome Step(bool pressed, Vector2 position, float edgeTop, float screenHeight, float sheetHeight)
        {
            bool fresh = pressed && !_wasPressed;
            _wasPressed = pressed;

            if (!_pressing)
            {
                if (fresh && position.y <= edgeTop && (_taken == null || !_taken(position)))
                {
                    _pressing = true;
                    _start = position;
                }
                return Outcome.None;
            }

            if (!pressed)
            {
                _pressing = false;
                if (!_dragging) return Outcome.None;
                _dragging = false;
                return _goingUp || Progress >= OpenAt ? Outcome.Open : Outcome.Cancel;
            }

            float rise = position.y - _start.y;
            if (!_dragging)
            {
                if (rise <= StartRise * screenHeight || rise <= Mathf.Abs(position.x - _start.x)) return Outcome.None;
                _dragging = true;
                _goingUp = true;
            }
            // The last way the finger moved; a frame it holds still doesn't change it.
            else if (position.y != _lastY) _goingUp = position.y > _lastY;
            _lastY = position.y;
            Progress = sheetHeight > 0 ? Mathf.Clamp01(rise / sheetHeight) : 1f;
            return Outcome.Dragging;
        }
    }
}
