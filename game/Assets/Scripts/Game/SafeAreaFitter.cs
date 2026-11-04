using UnityEngine;

namespace Starflare.Game
{
    /// <summary>Keeps a full-screen panel inside the notch and home-indicator safe area.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        void OnEnable() => Fit();
        void Update() => Fit();

        void Fit()
        {
            // Compare against the anchors themselves, not the last safe area applied: screens
            // stretch the panel after adding this, and when a phone turns the safe area and
            // the screen size can change a frame apart.
            var safe = Screen.safeArea;
            float width = Screen.width, height = Screen.height;
            if (width == 0 || height == 0) return;
            var min = new Vector2(safe.xMin / width, safe.yMin / height);
            var max = new Vector2(safe.xMax / width, safe.yMax / height);

            var rect = (RectTransform)transform;
            if (rect.anchorMin == min && rect.anchorMax == max && rect.offsetMin == Vector2.zero && rect.offsetMax == Vector2.zero)
                return;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
