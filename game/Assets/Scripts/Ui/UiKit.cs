using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Starflare.Ui
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

        // The city and story screens: dark glass panels with teal edges.
        public static readonly Color Teal = new Color(0.36f, 0.79f, 0.82f);
        public static readonly Color Edge = new Color(0.36f, 0.79f, 0.82f, 0.45f);
        public static readonly Color Panel = new Color(0.04f, 0.07f, 0.08f, 0.88f);
        public static readonly Color ChoiceFill = new Color(0.15f, 0.26f, 0.26f, 0.92f);
        public static readonly Color Paper = new Color(0.93f, 0.95f, 0.95f);

        static Sprite _rounded, _roundedFrame, _circle, _fade, _phoneGlyph, _eyeGlyph, _closeGlyph, _cashGlyph;

        /// <summary>
        /// The one typeface, TextMesh Pro's default (set up by Starflare > Rebuild Project Setup,
        /// which imports the TMP essentials; card suits and other glyphs outside its atlas come
        /// from the essentials' dynamic fallback). Liberation Sans has no star or emoji; FontTests
        /// checks every character the scripts and story put on screen.
        /// </summary>
        public static TMP_FontAsset FontAsset =>
            TMP_Settings.defaultFontAsset ?? throw new System.InvalidOperationException(
                "TextMesh Pro's essentials aren't imported. Run Starflare > Rebuild Project Setup.");

        /// <summary>A white rounded rectangle, nine-sliced, for cards, buttons and panels.</summary>
        public static Sprite Rounded => _rounded ??= MakeRounded(0);

        /// <summary>The outline of <see cref="Rounded"/>, nine-sliced, for a panel's edge.</summary>
        public static Sprite RoundedFrame => _roundedFrame ??= MakeRounded(3);

        /// <summary>A rounded rectangle, filled, or just its outline <paramref name="line"/> pixels thick.</summary>
        static Sprite MakeRounded(float line)
        {
            const int size = 64, radius = 20;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Max(radius - x - 0.5f, x + 0.5f - (size - radius), 0);
                float dy = Mathf.Max(radius - y - 0.5f, y + 0.5f - (size - radius), 0);
                float inside = radius - Mathf.Sqrt(dx * dx + dy * dy);
                float edge = Mathf.Clamp01(inside + 0.5f);
                if (line > 0) edge *= Mathf.Clamp01(line - inside + 0.5f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(edge * 255));
            }
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100,
                0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }

        /// <summary>A white disc, for map pins.</summary>
        public static Sprite Circle => _circle ??= Draw(64, (x, y) =>
            Mathf.Clamp01(32 - Mathf.Sqrt((x - 32) * (x - 32) + (y - 32) * (y - 32)) + 0.5f));

        /// <summary>White fading to clear from top to bottom, for a shade behind the top bar.</summary>
        public static Sprite Fade => _fade ??= Draw(64, (x, y) => y / 64);

        /// <summary>A white phone outline with its speaker slot, for the phone button.</summary>
        public static Sprite PhoneGlyph => _phoneGlyph ??= Draw(128, (x, y) =>
            Mathf.Max(Stroke(Box(x, y, 64, 64, 30, 54, 12), 8), Fill(Box(x, y, 64, 100, 9, 3, 3))));

        /// <summary>A white stack of three bills, for the cash pill.</summary>
        public static Sprite CashGlyph => _cashGlyph ??= Draw(128, (x, y) =>
        {
            const float line = 7;
            float front = Box(x, y, 64, 50, 52, 24, 6), middle = Box(x, y, 64, 64, 52, 24, 6), back = Box(x, y, 64, 78, 52, 24, 6);
            float seal = Mathf.Sqrt((x - 64) * (x - 64) + (y - 50) * (y - 50)) - 10;
            // Each bill shows only above the one in front of it.
            float behindFront = Fill(line / 2 - front), behindMiddle = Fill(line / 2 - Mathf.Min(front, middle));
            return Mathf.Max(Mathf.Max(Stroke(front, line), Stroke(seal, 6)),
                Mathf.Max(Stroke(middle, line) * behindFront, Stroke(back, line) * behindMiddle));
        });

        /// <summary>A white eye, an almond outline round a pupil, for the look-around button.</summary>
        public static Sprite EyeGlyph => _eyeGlyph ??= Draw(128, (x, y) =>
        {
            // The almond is where two discs overlap, one centred above the eye and one below,
            // each meeting the other at the eye's corners.
            const float radius = 67.3f, offset = 37.3f, line = 8, pupil = 15;
            float above = Mathf.Sqrt((x - 64) * (x - 64) + (y - 64 - offset) * (y - 64 - offset)) - radius;
            float below = Mathf.Sqrt((x - 64) * (x - 64) + (y - 64 + offset) * (y - 64 + offset)) - radius;
            float outline = Mathf.Clamp01(line / 2 - Mathf.Abs(Mathf.Max(above, below) + line / 2) + 0.5f);
            float centre = Mathf.Clamp01(pupil - Mathf.Sqrt((x - 64) * (x - 64) + (y - 64) * (y - 64)) + 0.5f);
            return Mathf.Max(outline, centre);
        });

        /// <summary>A white cross, two strokes corner to corner, for a close button.</summary>
        public static Sprite CloseGlyph => _closeGlyph ??= Draw(128, (x, y) =>
        {
            const float arm = 34, line = 10;
            float u = (x - 64) / Mathf.Sqrt(2), v = (y - 64) / Mathf.Sqrt(2);
            float one = new Vector2(Mathf.Max(Mathf.Abs(u + v) - arm, 0), u - v).magnitude;
            float other = new Vector2(Mathf.Max(Mathf.Abs(u - v) - arm, 0), u + v).magnitude;
            return Fill(Mathf.Min(one, other) - line / 2);
        });

        /// <summary>Signed distance from a point to a rounded box: negative inside, positive outside.</summary>
        static float Box(float x, float y, float centreX, float centreY, float halfWidth, float halfHeight, float radius)
        {
            float qx = Mathf.Abs(x - centreX) - halfWidth + radius, qy = Mathf.Abs(y - centreY) - halfHeight + radius;
            return new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - radius;
        }

        /// <summary>Coverage of a line <paramref name="width"/> thick along a shape's edge, from its signed distance.</summary>
        static float Stroke(float distance, float width) => Mathf.Clamp01(width / 2 - Mathf.Abs(distance) + 0.5f);

        /// <summary>Coverage of a shape's inside, from its signed distance.</summary>
        static float Fill(float distance) => Mathf.Clamp01(0.5f - distance);

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

        /// <summary>The phone button every screen outside the games carries: it opens the phone's map.</summary>
        public static Button MakePhoneButton(Transform parent, UnityAction onClick) =>
            MakeIconButton(parent, "Phone Button", PhoneGlyph, onClick);

        /// <summary>A square glass button with a light glyph on it, like the phone button.</summary>
        public static Button MakeIconButton(Transform parent, string name, Sprite glyphSprite, UnityAction onClick)
        {
            var image = MakePanel(parent, name, Panel);
            image.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            image.rectTransform.sizeDelta = new Vector2(IconButtonSize, IconButtonSize);
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var glyph = MakeImage(image.transform, "Glyph", Paper);
            glyph.sprite = glyphSprite;
            glyph.raycastTarget = false;
            glyph.rectTransform.sizeDelta = new Vector2(64, 64);
            return button;
        }

        public const float IconButtonSize = 104;

        /// <summary>A rounded panel with a thin teal edge, the city and story screens' glass.</summary>
        public static Image MakePanel(Transform parent, string name, Color fill)
        {
            var image = MakeImage(parent, name, fill);
            image.sprite = Rounded;
            image.type = Image.Type.Sliced;
            var edge = MakeImage(image.transform, "Edge", Edge);
            edge.sprite = RoundedFrame;
            edge.type = Image.Type.Sliced;
            edge.raycastTarget = false;
            Stretch(edge.rectTransform);
            return image;
        }

        /// <summary>A story choice or a place's action: a teal glass bar with its label on the left.</summary>
        public static Button MakeChoiceButton(Transform parent, string label, UnityAction onClick)
        {
            var image = MakePanel(parent, label + " Button", ChoiceFill);
            var layout = image.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 112;
            layout.flexibleWidth = 1;
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var text = MakeText(image.transform, label, 44, TextAnchor.MiddleLeft, 0);
            Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(44, 0);
            text.rectTransform.offsetMax = new Vector2(-32, 0);
            return button;
        }

        /// <summary>
        /// An overlay canvas that turns with the screen (OrientationLayout): 1080x1920 in
        /// portrait, 1920x1080 in landscape. Pass a rearrange callback to OnTurn.
        /// </summary>
        public static Canvas MakeCanvas(Transform parent, string name, int sortingOrder)
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

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

        /// <summary>A TextMesh Pro label that wraps and never clips; <paramref name="anchor"/> is where the text sits in it.</summary>
        public static TMP_Text MakeText(Transform parent, string content, int size, TextAnchor anchor, float height)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = FontAsset;
            text.fontSize = size;
            text.alignment = Align(anchor);
            text.color = Color.white;
            text.text = content;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            if (height > 0) go.AddComponent<LayoutElement>().preferredHeight = height;
            return text;
        }

        /// <summary>Keeps the text on one line, however long it gets.</summary>
        public static void NoWrap(TMP_Text text) => text.textWrappingMode = TextWrappingModes.NoWrap;

        /// <summary>A soft dark drop shadow, for text that sits straight on a plate.</summary>
        public static void Shadowed(TMP_Text text)
        {
            var material = text.fontMaterial;
            material.EnableKeyword(ShaderUtilities.Keyword_Underlay);
            material.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0, 0, 0, 0.8f));
            material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.35f);
            material.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.35f);
            material.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.2f);
            material.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.25f);
        }

        static TextAlignmentOptions Align(TextAnchor anchor) => anchor switch
        {
            TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
            TextAnchor.UpperCenter => TextAlignmentOptions.Top,
            TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
            TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
            TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
            TextAnchor.MiddleRight => TextAlignmentOptions.Right,
            TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
            TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
            _ => TextAlignmentOptions.BottomRight,
        };

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
