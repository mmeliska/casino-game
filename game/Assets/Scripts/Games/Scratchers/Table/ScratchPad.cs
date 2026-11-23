using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Starflare.Scratchers
{
    /// <summary>
    /// Takes a finger or the mouse across the whole ticket, so one stroke can run over several
    /// spots, and hands each bit of it on as a stroke of the coin between two screen points.
    /// </summary>
    public sealed class ScratchPad : MonoBehaviour, IInitializePotentialDragHandler, IPointerDownHandler, IDragHandler
    {
        /// <summary>A stroke from one screen point to the next, and the camera the canvas is seen through.</summary>
        public event Action<Vector2, Vector2, Camera> Stroke;

        Vector2 _last;

        /// <summary>Scratching starts at the first pixel of movement, not after the drag threshold.</summary>
        public void OnInitializePotentialDrag(PointerEventData eventData) => eventData.useDragThreshold = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _last = eventData.position;
            Stroke?.Invoke(_last, _last, eventData.pressEventCamera);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Stroke?.Invoke(_last, eventData.position, eventData.pressEventCamera);
            _last = eventData.position;
        }
    }
}
