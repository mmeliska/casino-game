using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Starflare.Game
{
    /// <summary>
    /// The small set of uGUI builders every screen uses. Screens build their UI in code,
    /// for portrait and landscape, so the scene file stays generated.
    /// </summary>
    public static class UiKit
    {
        public static readonly Color Gold = new Color(0.95f, 0.76f, 0.25f);
        public static readonly Color InkBlack = new Color(0.07f, 0.06f, 0.06f);
        public static readonly Color ButtonColor = new Color(0.12f, 0.12f, 0.16f, 0.92f);
        public static readonly Color CardRed = new Color(0.78f, 0.1f, 0.12f);
        public static readonly Color CardBlack = new Color(0.08f, 0.08f, 0.08f);

        static Font _font;
        static Sprite _rounded, _circle, _mapGlyph;

        public static Font Font => _font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        /// <summary>A white rounded rectangle, nine-sliced, for cards, buttons and panels.</summary>
        public static Sprite Rounded
        {
            get
            {
                if (_rounded != null) return _rounded;
                const int size = 64, radius = 20;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
                var pixels = new Color32[size * size];
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(radius - x - 0.5f, x + 0.5f - (size - radius), 0);
                    float dy = Mathf.Max(radius - y - 0.5f, y + 0.5f - (size - radius), 0);
                    float edge = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(edge * 255));
                }
                texture.SetPixels32(pixels);
                texture.Apply();
                _rounded = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100,
                    0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
                return _rounded;
            }
        }

        /// <summary>A white disc, for map pins.</summary>
        public static Sprite Circle => _circle ??= Draw(64, (x, y) =>
            Mathf.Clamp01(32 - Mathf.Sqrt((x - 32) * (x - 32) + (y - 32) * (y - 32)) + 0.5f));

        /// <summary>A white folded map in three panels, the middle one shaded, for the map button.</summary>
        public static Sprite MapGlyph => _mapGlyph ??= Draw(128, (x, y) =>
        {
            const float left = 8, right = 120, panel = (right - left) / 3, rise = 14;
            if (x < left - 1 || x > right + 1) return 0;
            int fold = Mathf.Clamp((int)((x - left) / panel), 0, 2);
            // The panels zigzag: each fold edge sits higher or lower than the last.
            float t = (x - left) / panel - fold;
            float lift = (fold % 2 == 0 ? t : 1 - t) * rise;
            float bottom = 14 + lift, top = 100 + lift;
            float edge = Mathf.Clamp01(Mathf.Min(Mathf.Min(x - left, right - x), Mathf.Min(y - bottom, top - y)) + 0.5f);
            return edge * (fold == 1 ? 0.65f : 1f);
        });

        /// <summary>Draws a white sprite from a coverage function of the pixel centre.</summary>
        static Sprite Draw(int size, System.Func<float, float, float> alpha)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha(x + 0.5f, y + 0.5f) * 255));
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        /// <summary>The map button every screen outside the games carries: it opens the phone's map.</summary>
        public static Button MakeMapButton(Transform parent, UnityAction onClick)
        {
            var image = MakeImage(parent, "Map Button", ButtonColor);
            image.sprite = Rounded;
            image.type = Image.Type.Sliced;
            image.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            image.rectTransform.sizeDelta = new Vector2(150, 150);
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var glyph = MakeImage(image.transform, "Glyph", Gold);
            glyph.sprite = MapGlyph;
            glyph.raycastTarget = false;
            glyph.rectTransform.sizeDelta = new Vector2(96, 96);
            return button;
        }

        /// <summary>
        /// An overlay canvas that turns with the screen (OrientationLayout): 1080x1920 in
        /// portrait, 1920x1080 in landscape. Pass a rearrange callback to OnTurn.
        /// </summary>
        public static Canvas MakeCanvas(Transform parent, string name, int sortingOrder)
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(parent, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<OrientationLayout>();
            return canvas;
        }

        /// <summary>Runs a screen's rearrange callback now and whenever the screen turns.</summary>
        public static void OnTurn(Canvas canvas, System.Action<bool> rearrange) =>
            canvas.GetComponent<OrientationLayout>().OnTurn(rearrange);

        /// <summary>Anchors a rect and sets its edges as offsets from those anchors.</summary>
        public static void Pin(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        /// <summary>Moves children into a parent in this order, for screens that regroup when they turn.</summary>
        public static void Arrange(Transform parent, params Component[] children)
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].transform.SetParent(parent, false);
                children[i].transform.SetSiblingIndex(i);
            }
        }

        public static RectTransform MakeRow(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var group = go.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 16;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = false;
            return (RectTransform)go.transform;
        }

        public static RectTransform AddVertical(GameObject go, float spacing, RectOffset padding)
        {
            var group = go.AddComponent<VerticalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = false;
            return (RectTransform)go.transform;
        }

        public static Button MakeButton(Transform parent, string label, UnityAction onClick)
        {
            var image = MakeImage(parent, label + " Button", ButtonColor);
            image.sprite = Rounded;
            image.type = Image.Type.Sliced;
            var layout = image.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 140;
            layout.flexibleWidth = 1;
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var text = MakeText(image.transform, label, 44, TextAnchor.MiddleCenter, 0);
            Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(24, 0);
            text.rectTransform.offsetMax = new Vector2(-24, 0);
            return button;
        }

        public static Text MakeText(Transform parent, string content, int size, TextAnchor anchor, float height)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Font;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = Color.white;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            if (height > 0) go.AddComponent<LayoutElement>().preferredHeight = height;
            return text;
        }

        public static Image MakeImage(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }
        }

        public static string Money(int amount, bool signed = false)
        {
            string body = "$" + System.Math.Abs(amount).ToString("N0");
            if (amount < 0) return "-" + body;
            return signed && amount > 0 ? "+" + body : body;
        }
    }
}
