using System;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// Dot's register (A1.1), turned around for you to sign: your name, used from then on, and
    /// a box with Mr., Ms. and Prefer not to answer. Both are needed before you can sign.
    /// </summary>
    public sealed class RegisterCard : MonoBehaviour
    {
        const string Plate = "motel_office_night";
        const int NameLimit = 24;
        static readonly Color Paper = new Color(0.93f, 0.9f, 0.82f);

        static readonly (RegisterBox box, string label)[] Boxes =
        {
            (RegisterBox.Mr, "Mr."),
            (RegisterBox.Ms, "Ms."),
            (RegisterBox.PreferNotToAnswer, "Prefer not to answer"),
        };

        Action<string, RegisterBox> _onSigned;
        RegisterBox _box;

        GameObject _root;
        Image _plate;
        CoverFit _plateFit;
        TMP_InputField _name;
        Button _sign;
        readonly Image[] _boxImages = new Image[Boxes.Length];
        readonly TMP_Text[] _boxLabels = new TMP_Text[Boxes.Length];

        void Awake() => Build();

        public void Show(Action<string, RegisterBox> onSigned)
        {
            _onSigned = onSigned;
            _box = RegisterBox.Unticked;
            _name.text = "";
            _plateFit.Show(_plate, Art.Plate(Plate), Art.Focus(Plate));
            _root.SetActive(true);
            Refresh();
        }

        void Tick(RegisterBox box)
        {
            _box = box;
            Refresh();
        }

        void Refresh()
        {
            for (int i = 0; i < Boxes.Length; i++)
            {
                bool ticked = Boxes[i].box == _box;
                _boxImages[i].color = ticked ? Gold : ButtonColor;
                _boxLabels[i].color = ticked ? InkBlack : Color.white;
            }
            _sign.interactable = _name.text.Trim().Length > 0 && _box != RegisterBox.Unticked;
        }

        void Sign()
        {
            _root.SetActive(false);
            var done = _onSigned;
            _onSigned = null;
            done?.Invoke(_name.text.Trim(), _box);
        }

        void Build()
        {
            var canvas = MakeCanvas(transform, "Register Canvas", 12);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plate = MakeImage(backdrop.transform, "Plate", Color.white);
            _plate.raycastTarget = false;
            _plateFit = _plate.gameObject.AddComponent<CoverFit>();
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0f, 0f, 0f, 0.6f));
            Stretch(shade.rectTransform);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            var card = MakeImage(safe.transform, "Card", Paper);
            card.sprite = Rounded;
            card.type = Image.Type.Sliced;
            var column = AddVertical(card.gameObject, 24, new RectOffset(64, 64, 64, 64));

            var title = MakeText(column, "SLACK WATER MOTEL", 52, TextAnchor.MiddleCenter, 72);
            title.color = InkBlack;
            title.fontStyle = FontStyles.Bold;
            var subtitle = MakeText(column, "Guest register", 40, TextAnchor.MiddleCenter, 56);
            subtitle.color = InkBlack;
            subtitle.fontStyle = FontStyles.Italic;

            var nameLabel = MakeText(column, "Name", 38, TextAnchor.LowerLeft, 80);
            nameLabel.color = InkBlack;
            _name = MakeNameField(column);

            var boxLabel = MakeText(column, "Tick one", 38, TextAnchor.LowerLeft, 80);
            boxLabel.color = InkBlack;
            var row = MakeRow(column, "Boxes");
            row.GetComponent<LayoutElement>().preferredHeight = 130;
            for (int i = 0; i < Boxes.Length; i++)
            {
                var box = Boxes[i].box;
                var button = MakeButton(row, Boxes[i].label, () => Tick(box));
                _boxImages[i] = button.GetComponent<Image>();
                _boxLabels[i] = button.GetComponentInChildren<TMP_Text>();
                _boxLabels[i].fontSize = 36;
            }

            var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
            spacer.transform.SetParent(column, false);
            spacer.GetComponent<LayoutElement>().flexibleHeight = 1;

            _sign = MakeButton(column, "Sign", Sign);
            _sign.GetComponentInChildren<TMP_Text>().color = Gold;

            OnTurn(canvas, landscape =>
            {
                // Landscape is only 1080 tall, so the card goes wider and trims its margins.
                var centre = new Vector2(0.5f, 0.5f);
                if (landscape) Pin(card.rectTransform, centre, centre, new Vector2(-560, -500), new Vector2(560, 500));
                else Pin(card.rectTransform, centre, centre, new Vector2(-470, -410), new Vector2(470, 650));
                int margin = landscape ? 40 : 64;
                column.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(64, 64, margin, margin);
            });
            _root.SetActive(false);
        }

        TMP_InputField MakeNameField(Transform parent)
        {
            var field = MakeImage(parent, "Name Field", Color.white);
            field.sprite = Rounded;
            field.type = Image.Type.Sliced;
            field.gameObject.AddComponent<LayoutElement>().preferredHeight = 120;

            // The text area clips the typed line and the caret inside the field's margins.
            var area = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
            area.transform.SetParent(field.transform, false);
            var areaRect = (RectTransform)area.transform;
            Stretch(areaRect);
            areaRect.offsetMin = new Vector2(32, 0);
            areaRect.offsetMax = new Vector2(-32, 0);

            var text = MakeText(areaRect, "", 54, TextAnchor.MiddleLeft, 0);
            text.color = InkBlack;
            text.richText = false;
            NoWrap(text);
            Stretch(text.rectTransform);

            var placeholder = MakeText(areaRect, "Your name", 54, TextAnchor.MiddleLeft, 0);
            placeholder.color = new Color(0.45f, 0.42f, 0.38f);
            placeholder.fontStyle = FontStyles.Italic;
            Stretch(placeholder.rectTransform);

            var input = field.gameObject.AddComponent<TMP_InputField>();
            input.targetGraphic = field;
            input.textViewport = areaRect;
            input.textComponent = text;
            input.placeholder = placeholder;
            input.characterLimit = NameLimit;
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.onValueChanged.AddListener(_ => Refresh());
            return input;
        }
    }
}
