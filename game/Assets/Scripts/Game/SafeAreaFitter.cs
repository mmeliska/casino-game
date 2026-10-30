using UnityEngine;

namespace Starflare.Game
{
    /// <summary>Keeps a full-screen panel inside the notch and home-indicator safe area.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        Rect _applied;

        void Update()
        {
            var safe = Screen.safeArea;
            if (safe == _applied || Screen.width == 0 || Screen.height == 0) return;
            _applied = safe;

            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            rect.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
