using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Ui
{
    /// <summary>
    /// Fills the parent with a sprite like CSS background-size: cover, keeping the focus
    /// column in view. It can also be panned off the focus, edge to edge (LookAround), and kept there.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class CoverFit : MonoBehaviour
    {
        public float Aspect = 16f / 9f;
        public float Focus = 0.5f;

        Vector2 _lastParentSize;
        float _lastAspect, _lastFocus;
        float _look;

        public void Show(Image image, Sprite sprite, float focus)
        {
            image.sprite = sprite;
            image.enabled = sprite != null;
            if (sprite == null) return;
            Aspect = sprite.rect.width / sprite.rect.height;
            Focus = focus;
            _look = 0;
            Fit();
        }

        /// <summary>Pans the sprite sideways from where it sits, in canvas units, up to its edges.</summary>
        public void PanBy(float dx)
        {
            _look += dx;
            Fit();
        }

        /// <summary>Makes the column the sprite's panned to its focus, and returns it.</summary>
        public float KeepLook()
        {
            var rect = (RectTransform)transform;
            if (rect.sizeDelta.x > 0) Focus = 0.5f - rect.anchoredPosition.x / rect.sizeDelta.x;
            _look = 0;
            Fit();
            return Focus;
        }

        void LateUpdate()
        {
            var parent = (RectTransform)transform.parent;
            if (parent.rect.size != _lastParentSize || Aspect != _lastAspect || Focus != _lastFocus) Fit();
        }

        void Fit()
        {
            var rect = (RectTransform)transform;
            var parentSize = ((RectTransform)transform.parent).rect.size;
            _lastParentSize = parentSize;
            _lastAspect = Aspect;
            _lastFocus = Focus;
            if (parentSize.x <= 0 || parentSize.y <= 0) return;

            var size = parentSize.x / parentSize.y > Aspect
                ? new Vector2(parentSize.x, parentSize.x / Aspect)
                : new Vector2(parentSize.y * Aspect, parentSize.y);
            float slack = (size.x - parentSize.x) / 2;

            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            float x = Mathf.Clamp((0.5f - Focus) * size.x, -slack, slack);
            _look = Mathf.Clamp(_look, -slack - x, slack - x);
            rect.anchoredPosition = new Vector2(x + _look, 0);
        }
    }
}
