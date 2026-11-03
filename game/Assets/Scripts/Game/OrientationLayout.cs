using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Game
{
    /// <summary>
    /// Turns a canvas with the screen: portrait scales to a 1080-wide reference, landscape to a
    /// 1080-tall one, so either way the short side is 1080 units. Screens hand it a rearrange
    /// callback, which runs now and again whenever the screen turns, including while hidden.
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class OrientationLayout : MonoBehaviour
    {
        public static bool IsLandscape => Screen.width > Screen.height;

        event Action<bool> Rearrange;
        bool _applied, _landscape;

        /// <summary>Rearranges for the current orientation now, and again on every turn.</summary>
        public void OnTurn(Action<bool> rearrange)
        {
            Rearrange += rearrange;
            Apply(force: true);
        }

        void OnEnable() => Apply(force: false);
        void Update() => Apply(force: false);

        void Apply(bool force)
        {
            bool landscape = IsLandscape;
            if (_applied && !force && landscape == _landscape) return;
            _applied = true;
            _landscape = landscape;

            var scaler = GetComponent<CanvasScaler>();
            scaler.referenceResolution = landscape ? new Vector2(1920, 1080) : new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = landscape ? 1f : 0f;
            Rearrange?.Invoke(landscape);
        }
    }
}
