using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Game
{
    /// <summary>
    /// Fills the parent with a sprite like CSS background-size: cover, keeping the focus
    /// column in view.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class CoverFit : MonoBehaviour
    {
        public float Aspect = 16f / 9f;
        public float Focus = 0.5f;

        Vector2 _lastParentSize;
        float _lastAspect, _lastFocus;

        public void Show(Image image, Sprite sprite, float focus)
        {
            image.sprite = sprite;
            image.enabled = sprite != null;
            if (sprite == null) return;
            Aspect = sprite.rect.width / sprite.rect.height;
            Focus = focus;
            Fit();
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
            rect.anchoredPosition = new Vector2(Mathf.Clamp((0.5f - Focus) * size.x, -slack, slack), 0);
        }
    }
}
