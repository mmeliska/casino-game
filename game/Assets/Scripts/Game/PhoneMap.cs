using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The map on your phone: Northlight's base map with a pin for every place open to you. Drag
    /// to pan, pinch (or scroll) to zoom, tap a pin to go there. Until the place icons exist, a
    /// pin is a circle with the place's name beside it.
    /// </summary>
    public sealed class PhoneMap : MonoBehaviour
    {
        /// <summary>A place on the map. <c>At</c> is its pin as a fraction of the map from the top left.</summary>
        public readonly struct Place
        {
            public readonly string Id, Name;
            public readonly Vector2 At;
            public readonly bool LabelLeft;

            public Place(string id, string name, float x, float y, bool labelLeft = false)
            {
                Id = id;
                Name = name;
                At = new Vector2(x, y);
                LabelLeft = labelLeft;
            }
        }

        const string MapResource = "Map/city_map";
        const float MaxZoom = 4f;
        const float MaxStartZoom = 2.5f;

        GameObject _phone;
        RectTransform _viewport, _content;
        ScrollRect _scroll;
        Action<string> _go;
        Vector2 _focusMin, _focusMax;
        bool _framed;

        void Awake() => Build();

        /// <summary>Opens the map with these places on it; <paramref name="go"/> gets the one you tap.</summary>
        public void Show(IEnumerable<Place> places, string here, Action<string> go)
        {
            _go = go;
            ClearChildren(_content);
            _focusMin = Vector2.one;
            _focusMax = Vector2.zero;
            foreach (var place in places)
            {
                MakePin(place, place.Id == here);
                _focusMin = Vector2.Min(_focusMin, place.At);
                _focusMax = Vector2.Max(_focusMax, place.At);
            }
            _phone.SetActive(true);
            _framed = false;
        }

        void Close() => _phone.SetActive(false);

        void Pick(string id)
        {
            Close();
            _go(id);
        }

        void LateUpdate()
        {
            if (!_phone.activeSelf) return;
            var view = _viewport.rect.size;
            if (view.x <= 0 || view.y <= 0) return;

            // Frame the open places once the phone has its size: fit them in the middle of the
            // screen, zoomed in no further than MaxStartZoom.
            if (!_framed)
            {
                _framed = true;
                var span = Vector2.Max(_focusMax - _focusMin, new Vector2(0.01f, 0.01f));
                float size = Mathf.Min(0.6f * view.x / span.x, 0.6f * view.y / span.y, MinSize * MaxStartZoom);
                SetSize(size);
                var centre = (_focusMin + _focusMax) / 2;
                SetPosition(new Vector2((0.5f - centre.x) * size, (centre.y - 0.5f) * size));
            }

            if (Input.touchCount == 2)
            {
                var a = Input.GetTouch(0);
                var b = Input.GetTouch(1);
                float before = Vector2.Distance(a.position - a.deltaPosition, b.position - b.deltaPosition);
                if (before > 0) Zoom(Vector2.Distance(a.position, b.position) / before, (a.position + b.position) / 2);
            }
            else if (Input.mouseScrollDelta.y != 0)
                Zoom(1 + 0.1f * Input.mouseScrollDelta.y, Input.mousePosition);
            // Two fingers zoom; the scroll view would read them as a drag.
            _scroll.enabled = Input.touchCount < 2;

            // The map always covers the screen, even after the phone turns.
            if (_content.sizeDelta.x < MinSize) SetSize(MinSize);
            SetPosition(_content.anchoredPosition);
        }

        /// <summary>The smallest the map gets: just covering the phone's screen.</summary>
        float MinSize => Mathf.Max(_viewport.rect.width, _viewport.rect.height);

        /// <summary>Zooms about a screen point, keeping the spot under it in place.</summary>
        void Zoom(float factor, Vector2 screenPoint)
        {
            float before = _content.sizeDelta.x;
            float size = Mathf.Clamp(before * factor, MinSize, MinSize * MaxZoom);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewport, screenPoint, null, out var point);
            var position = point - (point - _content.anchoredPosition) * (size / before);
            SetSize(size);
            SetPosition(position);
        }

        void SetSize(float size) => _content.sizeDelta = new Vector2(size, size);

        /// <summary>Moves the map, keeping its edges outside the phone's screen.</summary>
        void SetPosition(Vector2 position)
        {
            var slack = (_content.sizeDelta - _viewport.rect.size) / 2;
            _content.anchoredPosition = new Vector2(
                Mathf.Clamp(position.x, -slack.x, slack.x),
                Mathf.Clamp(position.y, -slack.y, slack.y));
        }

        void MakePin(Place place, bool here)
        {
            var anchor = new Vector2(place.At.x, 1 - place.At.y);
            var ring = MakeImage(_content, place.Name + " Pin", InkBlack);
            ring.sprite = Circle;
            ring.rectTransform.anchorMin = ring.rectTransform.anchorMax = anchor;
            ring.rectTransform.sizeDelta = new Vector2(64, 64);
            ring.gameObject.AddComponent<Button>().onClick.AddListener(() =>
            {
                if (here) Close();
                else Pick(place.Id);
            });

            var dot = MakeImage(ring.transform, "Dot", here ? Color.white : Gold);
            dot.sprite = Circle;
            dot.raycastTarget = false;
            dot.rectTransform.sizeDelta = new Vector2(44, 44);

            // The name is part of the button too, so it's an easy tap.
            var label = MakeText(ring.transform, place.Name, 40, place.LabelLeft ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft, 0);
            if (here) label.text += "\n<size=30><i>You're here</i></size>";
            label.fontStyle = FontStyle.Bold;
            label.raycastTarget = true;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.gameObject.AddComponent<Outline>().effectDistance = new Vector2(3, -3);
            var fit = label.gameObject.AddComponent<ContentSizeFitter>();
            fit.horizontalFit = fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var side = place.LabelLeft ? 0f : 1f;
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(side, 0.5f);
            label.rectTransform.pivot = new Vector2(1 - side, 0.5f);
            label.rectTransform.anchoredPosition = new Vector2(place.LabelLeft ? -14 : 14, 0);
        }

        void Build()
        {
            var canvas = MakeCanvas(transform, "Phone Canvas", 8);
            _phone = canvas.gameObject;

            // Tapping outside the phone puts it away.
            var shade = MakeImage(canvas.transform, "Shade", new Color(0f, 0f, 0f, 0.7f));
            Stretch(shade.rectTransform);
            shade.gameObject.AddComponent<Button>().onClick.AddListener(Close);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            var body = MakeImage(safe.transform, "Phone", new Color(0.1f, 0.1f, 0.12f));
            body.sprite = Rounded;
            body.type = Image.Type.Sliced;
            body.pixelsPerUnitMultiplier = 0.4f;

            var screen = MakeImage(body.transform, "Screen", new Color(0.2f, 0.27f, 0.33f));
            screen.sprite = Rounded;
            screen.type = Image.Type.Sliced;
            screen.pixelsPerUnitMultiplier = 0.8f;
            Stretch(screen.rectTransform);
            screen.rectTransform.offsetMin = new Vector2(24, 24);
            screen.rectTransform.offsetMax = new Vector2(-24, -24);
            screen.gameObject.AddComponent<RectMask2D>();
            _viewport = screen.rectTransform;

            var map = MakeImage(screen.transform, "Map", Color.white);
            map.sprite = Resources.Load<Sprite>(MapResource);
            if (map.sprite == null) Debug.LogWarning($"No map at Resources/{MapResource}; showing the pins on blank.");
            map.enabled = map.sprite != null;
            _content = map.rectTransform;
            _content.anchorMin = _content.anchorMax = _content.pivot = new Vector2(0.5f, 0.5f);

            _scroll = screen.gameObject.AddComponent<ScrollRect>();
            _scroll.viewport = _viewport;
            _scroll.content = _content;
            _scroll.movementType = ScrollRect.MovementType.Clamped;
            // The scroll wheel zooms instead.
            _scroll.scrollSensitivity = 0;

            var bar = MakeImage(screen.transform, "Top Bar", new Color(0.06f, 0.05f, 0.05f, 0.85f));
            Pin(bar.rectTransform, Vector2.up, Vector2.one, new Vector2(0, -120), Vector2.zero);
            var title = MakeText(bar.transform, "Northlight", 48, TextAnchor.MiddleLeft, 0);
            title.color = Gold;
            title.fontStyle = FontStyle.Bold;
            Stretch(title.rectTransform);
            title.rectTransform.offsetMin = new Vector2(40, 0);
            var close = MakeButton(bar.transform, "Close", Close);
            Pin((RectTransform)close.transform, new Vector2(1, 0), Vector2.one, new Vector2(-220, 16), new Vector2(-20, -16));

            // Portrait: the phone upright, nearly filling the screen. Landscape: turned on its side.
            OnTurn(canvas, landscape =>
            {
                if (landscape)
                    Pin(body.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(-850, 40), new Vector2(850, -40));
                else
                    Pin(body.rectTransform, Vector2.zero, Vector2.one, new Vector2(48, 72), new Vector2(-48, -72));
            });
            _phone.SetActive(false);
        }
    }
}
