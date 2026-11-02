using UnityEngine;

namespace Starflare.Game
{
    /// <summary>Keeps a full-screen panel inside the notch and home-indicator safe area.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        Rect _applied;
        Vector2Int _appliedScreen;

        void OnEnable() => Fit();
        void Update() => Fit();

        void Fit()
        {
            // When a phone turns, the safe area and the screen size can change a frame apart,
            // so refit on either.
            var safe = Screen.safeArea;
            var screen = new Vector2Int(Screen.width, Screen.height);
            if (safe == _applied && screen == _appliedScreen || screen.x == 0 || screen.y == 0) return;
            _applied = safe;
            _appliedScreen = screen;

            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(safe.xMin / screen.x, safe.yMin / screen.y);
            rect.anchorMax = new Vector2(safe.xMax / screen.x, safe.yMax / screen.y);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
