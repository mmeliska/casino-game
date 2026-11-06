using UnityEngine;
using UnityEngine.EventSystems;

namespace Starflare.Ui
{
    /// <summary>
    /// Looking around a plate a portrait screen crops: while it's on, dragging the backdrop pans
    /// the plate edge to edge. The close button ends it, and the plate stays where you left it.
    /// </summary>
    public sealed class LookAround : MonoBehaviour, IDragHandler
    {
        CoverFit _fit;
        Canvas _canvas;

        public bool On { get; set; }

        public void Init(CoverFit fit)
        {
            _fit = fit;
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (On) _fit.PanBy(eventData.delta.x / _canvas.scaleFactor);
        }
    }
}
