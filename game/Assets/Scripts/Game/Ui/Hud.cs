using Starflare.State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The bar across the top of place screens and scenes: where you are, the date and time,
    /// your cash, and the phone. On a portrait place screen the eye sits under the phone, to
    /// look around the plate; while you look, it all steps aside for a close button.
    /// During a scene the buttons step away, since you can't leave.
    /// The same in portrait and landscape, except the eye, which only portrait needs.
    /// </summary>
    public sealed class Hud : MonoBehaviour
    {
        const float Margin = 36;
        static readonly Vector3[] Corners = new Vector3[4];

        PlayerState _state;
        GameObject _root, _info, _corner, _phone, _eye, _close;
        Image _shade, _backing;
        Text _title, _date, _cash, _owed;
        RectTransform _pill;
        bool _buttons, _looking, _fitDirty;
        (int cash, int owed, int day, int minutes) _shown;

        public void Init(PlayerState state, UnityAction openPhone, UnityAction look, UnityAction stopLooking)
        {
            _state = state;
            _state.Changed += Refresh;
            Build(openPhone, look, stopLooking);
            _root.SetActive(false);
        }

        void OnDestroy() => _state.Changed -= Refresh;

        /// <summary>Shows the bar for a place; <paramref name="buttons"/> is false during a scene.</summary>
        public void Show(string place, bool buttons)
        {
            _title.text = place;
            _buttons = buttons;
            _phone.SetActive(buttons);
            Looking(false);
            _shown = default;
            _root.SetActive(true);
            Refresh();
        }

        public void Hide() => _root.SetActive(false);

        /// <summary>While you look around, everything steps aside but the close button.</summary>
        public void Looking(bool on)
        {
            _looking = on;
            _info.SetActive(!on);
            _shade.enabled = !on;
            _corner.SetActive(!on);
            _eye.SetActive(_buttons && !on && !OrientationLayout.IsLandscape);
            _close.SetActive(on);
        }

        // The pill's width is only known after layout, so a refresh fits the backing a frame later.
        void LateUpdate()
        {
            if (!_fitDirty || !_root.activeSelf) return;
            _fitDirty = false;
            FitBacking();
        }

        /// <summary>
        /// Runs the backing, and the room for the name and date, up to just before the cash pill,
        /// which moves as the amount grows and when the phone steps away.
        /// </summary>
        void FitBacking()
        {
            var info = (RectTransform)_info.transform;
            _pill.GetWorldCorners(Corners);
            float right = info.InverseTransformPoint(Corners[0]).x - info.rect.xMax - 16;
            var backing = _backing.rectTransform;
            if (Mathf.Approximately(backing.offsetMax.x, right)) return;
            backing.offsetMax = new Vector2(right, backing.offsetMax.y);
            _title.rectTransform.offsetMax = new Vector2(right - 24, _title.rectTransform.offsetMax.y);
            _date.rectTransform.offsetMax = new Vector2(right - 24, _date.rectTransform.offsetMax.y);
        }

        /// <summary>Redraws from the state: on Show, and whenever the state says it changed (a scene can spend or borrow).</summary>
        void Refresh()
        {
            _fitDirty = true;
            if (!_root.activeSelf) return;
            var clock = _state.Clock;
            var now = (_state.BankrollNow, _state.Debts, clock.Day, (int)clock.Part * GameClock.MinutesPerPart + clock.MinutesIntoPart);
            if (now == _shown) return;
            _shown = now;
            _date.text = $"{clock.DateLabel}  •  {clock.TimeLabel}";
            _cash.text = Money(_state.BankrollNow);
            _owed.text = "Owed " + Money(_state.Debts);
            _owed.gameObject.SetActive(_state.Debts > 0);
        }

        void Build(UnityAction openPhone, UnityAction look, UnityAction stopLooking)
        {
            // Over the scene (10), under the register card (12) and the phone (13).
            var canvas = MakeCanvas(transform, "Hud Canvas", 11);
            _root = canvas.gameObject;

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            var info = new GameObject("Info", typeof(RectTransform));
            info.transform.SetParent(safe.transform, false);
            Stretch((RectTransform)info.transform);
            _info = info;

            // A shade from the top of the screen, under the notch too, keeps the text readable on any plate.
            var shade = _shade = MakeImage(canvas.transform, "Shade", new Color(0, 0, 0, 0.7f));
            shade.sprite = Fade;
            shade.raycastTarget = false;
            shade.transform.SetAsFirstSibling();

            // A see-through backing keeps the name and date readable on a bright plate. Its right
            // edge, and the name's and date's, follow the cash pill (FitBacking).
            _backing = MakeImage(info.transform, "Backing", new Color(0, 0, 0, 0.5f));
            _backing.sprite = Rounded;
            _backing.type = Image.Type.Sliced;
            _backing.raycastTarget = false;
            Pin(_backing.rectTransform, Vector2.up, Vector2.one, new Vector2(Margin - 8, -150), new Vector2(-460, -22));

            var accent = MakeImage(info.transform, "Accent", Teal);
            accent.raycastTarget = false;
            Pin(accent.rectTransform, Vector2.up, Vector2.up, new Vector2(Margin + 4, -136), new Vector2(Margin + 12, -40));

            // The title shrinks to fit rather than run past the backing.
            _title = MakeText(info.transform, "", 52, TextAnchor.MiddleLeft, 0);
            _title.fontStyle = FontStyle.Bold;
            _title.resizeTextForBestFit = true;
            _title.resizeTextMinSize = 34;
            _title.resizeTextMaxSize = 52;
            _title.verticalOverflow = VerticalWrapMode.Truncate;
            _title.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(2, -2);
            Pin(_title.rectTransform, Vector2.up, Vector2.one, new Vector2(Margin + 36, -96), new Vector2(-460, -34));

            _date = MakeText(info.transform, "", 34, TextAnchor.MiddleLeft, 0);
            _date.fontStyle = FontStyle.Italic;
            _date.color = new Color(Paper.r, Paper.g, Paper.b, 0.85f);
            _date.horizontalOverflow = HorizontalWrapMode.Overflow;
            _date.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(2, -2);
            Pin(_date.rectTransform, Vector2.up, Vector2.one, new Vector2(Margin + 36, -140), new Vector2(-460, -96));

            // Top right: the cash pill, then the phone. The pill slides into the corner when the phone steps away.
            var rowGo = new GameObject("Corner", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            rowGo.transform.SetParent(safe.transform, false);
            _corner = rowGo;
            var row = rowGo.GetComponent<HorizontalLayoutGroup>();
            row.spacing = 16;
            row.childAlignment = TextAnchor.UpperRight;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = row.childForceExpandHeight = false;
            Pin((RectTransform)rowGo.transform, Vector2.up, Vector2.one,
                new Vector2(0, -Margin - IconButtonSize), new Vector2(-Margin, -Margin));

            var pill = MakePanel(rowGo.transform, "Cash", Panel);
            pill.raycastTarget = false;
            _pill = pill.rectTransform;
            var pillRow = pill.gameObject.AddComponent<HorizontalLayoutGroup>();
            pillRow.padding = new RectOffset(26, 30, 0, 0);
            pillRow.spacing = 16;
            pillRow.childAlignment = TextAnchor.MiddleLeft;
            pillRow.childControlWidth = pillRow.childControlHeight = true;
            pillRow.childForceExpandWidth = pillRow.childForceExpandHeight = false;
            pill.gameObject.AddComponent<LayoutElement>().preferredHeight = IconButtonSize;
            // The pill's edge isn't one of its items.
            pill.transform.Find("Edge").gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

            var icon = MakeImage(pill.transform, "Icon", Paper);
            icon.sprite = CashGlyph;
            icon.raycastTarget = false;
            var iconLayout = icon.gameObject.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = iconLayout.preferredHeight = 56;

            var amounts = new GameObject("Amounts", typeof(RectTransform));
            amounts.transform.SetParent(pill.transform, false);
            var amountsColumn = AddVertical(amounts, 0, new RectOffset(0, 0, 0, 0)).GetComponent<VerticalLayoutGroup>();
            amountsColumn.childAlignment = TextAnchor.MiddleLeft;
            // Expanding would make the pill flexible, and the corner row would stretch it across the bar.
            amountsColumn.childForceExpandWidth = false;
            _cash = MakeText(amounts.transform, "", 40, TextAnchor.MiddleLeft, 0);
            _cash.horizontalOverflow = HorizontalWrapMode.Overflow;
            _owed = MakeText(amounts.transform, "", 26, TextAnchor.MiddleLeft, 0);
            _owed.color = new Color(Paper.r, Paper.g, Paper.b, 0.7f);
            _owed.horizontalOverflow = HorizontalWrapMode.Overflow;

            var phone = MakePhoneButton(rowGo.transform, openPhone);
            var phoneLayout = phone.GetComponent<LayoutElement>();
            phoneLayout.ignoreLayout = false;
            phoneLayout.preferredWidth = phoneLayout.preferredHeight = IconButtonSize;
            _phone = phone.gameObject;

            var eye = MakeIconButton(safe.transform, "Look Button", EyeGlyph, look);
            _eye = eye.gameObject;
            var eyeRect = (RectTransform)eye.transform;
            eyeRect.anchorMin = eyeRect.anchorMax = eyeRect.pivot = Vector2.one;
            eyeRect.anchoredPosition = new Vector2(-Margin, -Margin - IconButtonSize - 20);

            // The close button takes the phone's corner while the rest of the bar is away.
            var close = MakeIconButton(safe.transform, "Close Look Button", CloseGlyph, stopLooking);
            _close = close.gameObject;
            var closeRect = (RectTransform)close.transform;
            closeRect.anchorMin = closeRect.anchorMax = closeRect.pivot = Vector2.one;
            closeRect.anchoredPosition = new Vector2(-Margin, -Margin);

            OnTurn(canvas, landscape =>
            {
                Pin(shade.rectTransform, Vector2.up, Vector2.one, new Vector2(0, landscape ? -240 : -320), Vector2.zero);
                // A 16:9 plate fills a landscape screen whole, so there's nothing to look round.
                _eye.SetActive(_buttons && !landscape && !_looking);
                _fitDirty = true;
            });
        }
    }
}
