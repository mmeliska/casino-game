using System;
using System.Collections.Generic;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The map on your phone: Northlight's base map with a pin for every place open to you. Drag
    /// to pan, pinch (or scroll) to zoom, tap a pin to go there. Until the place icons exist, a
    /// pin is a circle with the place's name beside it.
    ///
    /// Going somewhere is a trip you watch: a marker runs from your pin to the one you tapped
    /// while the top bar counts through the minutes it takes (a tap skips it). The phone stays
    /// up when it gets there; the city puts it away under the dip to black (<see cref="Hide"/>).
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

        /// <summary>How long a trip takes to watch, whatever it costs on the clock (starting value).</summary>
        const float TripSeconds = 1.2f;

        GameObject _phone, _close, _skip;
        TMP_Text _tripClock;
        RectTransform _marker, _trail;
        RectTransform _viewport, _content;
        ScrollRect _scroll;
        Action<string> _go;
        Vector2 _focusMin, _focusMax;
        bool _framed;

        // A trip under way: from your pin to the tapped one, how far along (below 0 for none),
        // the clock it left at and what it costs.
        Vector2? _hereAt;
        Vector2 _tripFrom, _tripTo;
        string _tripId;
        float _trip = -1f;
        int _leftAt, _tripMinutes;

        void Awake() => Build();

        /// <summary>
        /// Opens the map with these places on it. <paramref name="go"/> gets the one you tap, once
        /// you've watched the trip there, which counts on from <paramref name="now"/> by
        /// <paramref name="tripMinutes"/>.
        /// </summary>
        public void Show(IEnumerable<Place> places, string here, GameClock now, int tripMinutes, Action<string> go)
        {
            _go = go;
            _leftAt = now.MinuteOfDay;
            _tripMinutes = tripMinutes;
            ClearChildren(_content);
            _focusMin = Vector2.one;
            _focusMax = Vector2.zero;
            _hereAt = null;
            foreach (var place in places)
            {
                MakePin(place, place.Id == here);
                if (place.Id == here) _hereAt = place.At;
                _focusMin = Vector2.Min(_focusMin, place.At);
                _focusMax = Vector2.Max(_focusMax, place.At);
            }
            MakeTrip();
            _phone.SetActive(true);
            _framed = false;
        }

        /// <summary>Puts the phone away, mid-trip or not.</summary>
        public void Hide()
        {
            _trip = -1f;
            _skip.SetActive(false);
            _tripClock.gameObject.SetActive(false);
            _close.SetActive(true);
            _phone.SetActive(false);
        }

        void Close() => Hide();

        // Pinch zoom reads two active touches through EnhancedTouch, which is off until asked.
        void OnEnable() => EnhancedTouchSupport.Enable();

        void OnDisable() => EnhancedTouchSupport.Disable();

        void Pick(Place place)
        {
            if (_trip >= 0) return;
            if (_hereAt == null)
            {
                _go(place.Id);
                return;
            }
            _tripId = place.Id;
            _tripFrom = _hereAt.Value;
            _tripTo = place.At;
            _trip = 0f;
            _close.SetActive(false);
            _tripClock.gameObject.SetActive(true);
            _skip.SetActive(true);
            _marker.gameObject.SetActive(true);
            _trail.gameObject.SetActive(true);
            _scroll.enabled = false;
            StepTrip(0f);
        }

        /// <summary>Moves the marker on and the clock with it; at the far pin, hands the place over.</summary>
        void StepTrip(float seconds)
        {
            _trip = Mathf.Min(1f, _trip + seconds / TripSeconds);
            float along = Mathf.SmoothStep(0, 1, _trip);
            var at = Vector2.Lerp(_tripFrom, _tripTo, along);
            _marker.anchorMin = _marker.anchorMax = new Vector2(at.x, 1 - at.y);

            // The trail runs from your pin to the marker, in the map's own units.
            float size = _content.sizeDelta.x;
            var run = new Vector2((at.x - _tripFrom.x) * size, (_tripFrom.y - at.y) * size);
            _trail.anchorMin = _trail.anchorMax = new Vector2(_tripFrom.x, 1 - _tripFrom.y);
            _trail.sizeDelta = new Vector2(run.magnitude, 10);
            _trail.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(run.y, run.x) * Mathf.Rad2Deg);

            _tripClock.text = GameClock.TimeLabelAt(_leftAt + Mathf.RoundToInt(_tripMinutes * along));
            if (_trip < 1f) return;
            _trip = -1f;
            _skip.SetActive(false);
            _go(_tripId);
        }

        void LateUpdate()
        {
            if (!_phone.activeSelf) return;
            if (_trip >= 0)
            {
                StepTrip(Mathf.Min(Time.unscaledDeltaTime, 1f / 30f));
                return;
            }
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

            var touches = Touch.activeTouches;
            if (touches.Count == 2)
            {
                Touch a = touches[0], b = touches[1];
                float before = Vector2.Distance(a.screenPosition - a.delta, b.screenPosition - b.delta);
                if (before > 0) Zoom(Vector2.Distance(a.screenPosition, b.screenPosition) / before, (a.screenPosition + b.screenPosition) / 2);
            }
            else if (Mouse.current != null && Mouse.current.scroll.ReadValue().y != 0)
            {
                // A wheel notch reads about 120 on some platforms and a few units on others; cap a step at 10%.
                float notch = Mathf.Clamp(Mouse.current.scroll.ReadValue().y / 120f, -1f, 1f);
                Zoom(1 + 0.1f * notch, Mouse.current.position.ReadValue());
            }
            // Two fingers zoom; the scroll view would read them as a drag.
            _scroll.enabled = touches.Count < 2;

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
                else Pick(place);
            });

            var dot = MakeImage(ring.transform, "Dot", here ? Color.white : Gold);
            dot.sprite = Circle;
            dot.raycastTarget = false;
            dot.rectTransform.sizeDelta = new Vector2(44, 44);

            // The name is part of the button too, so it's an easy tap.
            var label = MakeText(ring.transform, place.Name, 40, place.LabelLeft ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft, 0);
            if (here) label.text += "\n<size=30><i>You're here</i></size>";
            label.fontStyle = FontStyles.Bold;
            label.raycastTarget = true;
            NoWrap(label);
            label.gameObject.AddComponent<Outline>().effectDistance = new Vector2(3, -3);
            var fit = label.gameObject.AddComponent<ContentSizeFitter>();
            fit.horizontalFit = fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var side = place.LabelLeft ? 0f : 1f;
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(side, 0.5f);
            label.rectTransform.pivot = new Vector2(1 - side, 0.5f);
            label.rectTransform.anchoredPosition = new Vector2(place.LabelLeft ? -14 : 14, 0);
        }

        /// <summary>The trail and marker for a trip, under the pins and over them; hidden until you pick one.</summary>
        void MakeTrip()
        {
            var trail = MakeImage(_content, "Trail", new Color(Teal.r, Teal.g, Teal.b, 0.85f));
            trail.raycastTarget = false;
            _trail = trail.rectTransform;
            _trail.pivot = new Vector2(0, 0.5f);
            _trail.anchoredPosition = Vector2.zero;
            _trail.SetAsFirstSibling();
            _trail.gameObject.SetActive(false);

            var marker = MakeImage(_content, "Marker", Teal);
            marker.sprite = Circle;
            marker.raycastTarget = false;
            _marker = marker.rectTransform;
            _marker.sizeDelta = new Vector2(48, 48);
            _marker.anchoredPosition = Vector2.zero;
            var middle = MakeImage(marker.transform, "Middle", Paper);
            middle.sprite = Circle;
            middle.raycastTarget = false;
            middle.rectTransform.sizeDelta = new Vector2(24, 24);
            _marker.gameObject.SetActive(false);
        }

        void Build()
        {
            var canvas = MakeCanvas(transform, "Phone Canvas", 13);
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
            title.fontStyle = FontStyles.Bold;
            Stretch(title.rectTransform);
            title.rectTransform.offsetMin = new Vector2(40, 0);
            var close = MakeButton(bar.transform, "Close", Close);
            Pin((RectTransform)close.transform, new Vector2(1, 0), Vector2.one, new Vector2(-220, 16), new Vector2(-20, -16));
            _close = close.gameObject;

            // On a trip, the clock takes the close button's place.
            _tripClock = MakeText(bar.transform, "", 48, TextAnchor.MiddleRight, 0);
            _tripClock.color = Paper;
            NoWrap(_tripClock);
            Stretch(_tripClock.rectTransform);
            _tripClock.rectTransform.offsetMax = new Vector2(-40, 0);
            _tripClock.gameObject.SetActive(false);

            // On a trip, a tap anywhere gets you there.
            var skip = MakeImage(canvas.transform, "Skip", Color.clear);
            Stretch(skip.rectTransform);
            skip.gameObject.AddComponent<Button>().onClick.AddListener(() =>
            {
                if (_trip >= 0) StepTrip(TripSeconds);
            });
            _skip = skip.gameObject;
            _skip.SetActive(false);

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
