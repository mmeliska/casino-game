using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// The ball on the approach: drag it left or right along the foul line and let go; it snaps
    /// to a board, and the board number shows while you hold it. Placement, not a throw: nothing
    /// about how your finger moved counts, only where you let go. No bowler is drawn (Mike,
    /// 2026-11-23); the ball is what you line up.
    /// </summary>
    public sealed class ApproachBall : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>Where the ball sits, just behind the foul line.</summary>
        public const double RestFt = -0.7;

        LaneView _view;
        Image _ball;
        TMP_Text _label;
        Func<bool> _canDrag;
        Action<int> _moved;
        Action _done;

        /// <summary>The board it sits on, counted from the bowler's own gutter.</summary>
        public int Board { get; private set; } = LaneGeometry.MiddleBoard;

        public Hand Hand { get; set; }

        public static ApproachBall Make(LaneView view, Func<bool> canDrag, Action<int> moved, Action done)
        {
            var image = MakeImage(view.transform, "Approach Ball", Color.red);
            image.sprite = Circle;
            var label = MakeText(image.transform, "", 40, TextAnchor.MiddleCenter, 0);
            label.name = "Board";
            NoWrap(label);
            label.fontStyle = FontStyles.Bold;
            Shadowed(label);
            Pin(label.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-60, 10), new Vector2(60, 70));
            var ball = image.gameObject.AddComponent<ApproachBall>();
            ball._view = view;
            ball._ball = image;
            ball._label = label;
            ball._canDrag = canDrag;
            ball._moved = moved;
            ball._done = done;
            label.gameObject.SetActive(false);
            return ball;
        }

        /// <summary>The ball's picture, or a circle in its colour while the picture isn't in.</summary>
        public void Show(BowlingBall ball, Sprite picture)
        {
            _ball.sprite = picture != null ? picture : Circle;
            _ball.color = picture != null ? Color.white : BallLook.ColourOf(ball);
            _ball.preserveAspect = true;
        }

        /// <summary>Puts the ball on a board (clamped to the lane) without anyone dragging it.</summary>
        public void Place(int board)
        {
            Board = Math.Max(Throw.MinStand, Math.Min(Throw.MaxStand, board));
            Layout();
        }

        /// <summary>Sits the ball over its board, sized for the depth; call when the view's size changes.</summary>
        public void Layout()
        {
            double laneBoard = Hand == Hand.Right ? Board : LaneGeometry.Mirror(Board);
            var rect = (RectTransform)transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            float size = Mathf.Max(48, _view.BallPx(RestFt) * 2);
            rect.sizeDelta = new Vector2(size, size);
            var at = _view.Project(LaneGeometry.Point(laneBoard, RestFt));
            var viewRect = _view.rectTransform.rect;
            rect.anchoredPosition = at - viewRect.center + new Vector2(0, size * 0.1f);
            _label.text = Board.ToString();
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (!_canDrag()) return;
            _label.gameObject.SetActive(true);
            OnDrag(data);
        }

        public void OnDrag(PointerEventData data)
        {
            if (!_canDrag()) return;
            DragTo(data.position, data.pressEventCamera);
        }

        public void OnEndDrag(PointerEventData data)
        {
            _label.gameObject.SetActive(false);
            if (_canDrag()) _done();
        }

        /// <summary>The ball follows a screen point along the foul line, a board at a time.</summary>
        public void DragTo(Vector2 screenPoint, Camera camera = null)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_view.rectTransform, screenPoint, camera, out var local)) return;
            double board = LaneGeometry.Board(_view.LaneXAt(local.x, RestFt));
            if (Hand == Hand.Left) board = LaneGeometry.Mirror(board);
            int snapped = (int)Math.Round(Math.Max(Throw.MinStand, Math.Min(Throw.MaxStand, board)));
            if (snapped == Board) return;
            Place(snapped);
            _moved(snapped);
        }

        public bool Showing
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
    }
}
