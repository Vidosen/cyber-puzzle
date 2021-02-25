using System;
using Prototype.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public abstract class BaseSlot<TVector> : MonoBehaviour, IDropHandler where TVector : BaseVector
    {
        private RectTransform _thisTransform;

        public TVector Vector { get; set; }
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        
        public void OnDrop(PointerEventData eventData)
        {
            var onDragObj = eventData.pointerDrag;
            Debug.Log("BaseLineSlotView.OnDrop: "+ onDragObj.name);
            
        }
    }
}
