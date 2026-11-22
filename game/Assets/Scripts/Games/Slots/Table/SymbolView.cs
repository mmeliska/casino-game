using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Slots
{
    /// <summary>
    /// One symbol on screen: a reel cell, or a picture on the paytable glass. It holds every
    /// way a symbol can be drawn and shows the one its look asks for, so a turning reel only
    /// swaps what's showing. Sized by anchors, never by pixels, so it fits any cell.
    /// </summary>
    public sealed class SymbolView
    {
        const int MaxBars = 3;

        readonly Image _art, _cherries;
        readonly TMP_Text _glyph;
        readonly CanvasGroup _fade;
        readonly GameObject[] _bars = new GameObject[MaxBars];
        readonly Image[] _barSlabs = new Image[MaxBars];
        SymbolLook _showing;

        public SymbolView(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(parent, false);
            Rect = (RectTransform)go.transform;
            _fade = go.GetComponent<CanvasGroup>();
            _fade.blocksRaycasts = false;
            Stretch(Rect);

            _art = Fitted(MakeImage(Rect, "Art", Color.white));
            _cherries = Fitted(MakeImage(Rect, "Cherries", Color.white));
            _cherries.sprite = SlotGlyphs.Cherries;

            _glyph = MakeText(Rect, "", 100, TextAnchor.MiddleCenter, 0);
            _glyph.name = "Glyph";
            _glyph.fontStyle = FontStyles.Bold;
            _glyph.enableAutoSizing = true;
            _glyph.fontSizeMin = 10;
            _glyph.fontSizeMax = 400;
            _glyph.outlineWidth = 0.12f;
            _glyph.outlineColor = new Color32(30, 10, 10, 255);
            Stretch(_glyph.rectTransform);
            Inset(_glyph.rectTransform, 0.08f);

            for (int i = 0; i < MaxBars; i++)
            {
                var slab = MakeImage(Rect, $"Bar {i + 1}", Color.black);
                slab.sprite = Rounded;
                slab.type = Image.Type.Sliced;
                slab.pixelsPerUnitMultiplier = 3;
                slab.raycastTarget = false;
                var label = MakeText(slab.transform, "BAR", 40, TextAnchor.MiddleCenter, 0);
                label.fontStyle = FontStyles.Bold;
                label.enableAutoSizing = true;
                label.fontSizeMin = 6;
                label.fontSizeMax = 200;
                label.characterSpacing = 8;
                Stretch(label.rectTransform);
                label.rectTransform.anchorMin = new Vector2(0.06f, 0.1f);
                label.rectTransform.anchorMax = new Vector2(0.94f, 0.9f);
                _bars[i] = slab.gameObject;
                _barSlabs[i] = slab;
            }
            Show(SymbolLook.Blank());
        }

        public RectTransform Rect { get; }

        /// <summary>Dims a symbol at full spin, where a real reel blurs.</summary>
        public float Alpha { set => _fade.alpha = value; }

        public void Show(SymbolLook look)
        {
            if (ReferenceEquals(look, _showing)) return;
            _showing = look;
            bool art = look.Art != null;
            _art.enabled = art;
            if (art) _art.sprite = look.Art;
            _cherries.enabled = !art && look.Drawing == SymbolDrawing.Cherries;
            _glyph.enabled = !art && look.Drawing == SymbolDrawing.Glyph;
            if (_glyph.enabled)
            {
                _glyph.text = look.Glyph;
                _glyph.color = look.Ink;
            }

            int bars = !art && look.Drawing == SymbolDrawing.Bars ? Mathf.Clamp(look.Bars, 1, MaxBars) : 0;
            for (int i = 0; i < MaxBars; i++)
            {
                _bars[i].SetActive(i < bars);
                if (i >= bars) continue;
                _barSlabs[i].color = look.Ink;
                // Slabs stack in the middle of the cell, each about a quarter of it tall with a gap.
                const float height = 0.25f, gap = 0.04f;
                float stack = bars * height + (bars - 1) * gap;
                float top = 0.5f + stack / 2 - i * (height + gap);
                var rect = (RectTransform)_bars[i].transform;
                rect.anchorMin = new Vector2(0.1f, top - height);
                rect.anchorMax = new Vector2(0.9f, top);
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
        }

        static Image Fitted(Image image)
        {
            image.preserveAspect = true;
            image.raycastTarget = false;
            Stretch(image.rectTransform);
            Inset(image.rectTransform, 0.08f);
            return image;
        }

        static void Inset(RectTransform rect, float by)
        {
            rect.anchorMin = new Vector2(by, by);
            rect.anchorMax = new Vector2(1 - by, 1 - by);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
