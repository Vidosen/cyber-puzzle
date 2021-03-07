using System;
using Prototype.Scripts.Data;
using Prototype.Scripts.Matrix;
using Prototype.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public abstract class BaseSlot<TVector> : MonoBehaviour, IDropHandler, IDisposable where TVector : BaseVector
    {
        private RectTransform _thisTransform;
        private TVector _vector;

        public TVector Vector
        {
            get => _vector;
            set
            {
                _vector = value;
                _vector.ThisTransform.SetParent(ThisTransform);
                _vector.ThisTransform.localScale = Vector3.one;
                _vector.ThisTransform.anchoredPosition = Vector3.zero;
            }
        }

        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        
        public void OnDrop(PointerEventData eventData)
        {
            var onDragObj = eventData.pointerDrag;
            var oneVector = onDragObj.GetComponent<BaseVector>();
            if (oneVector is null)
                return;
            else if (oneVector.GetType() == Vector.GetType() && oneVector != Vector)
            {
                Debug.Log($"{GetType().Name}.OnDrop: " + onDragObj.name);
                GameService.Instance.SwapRequest(oneVector, Vector);
            }

        }

        public BaseSlot<TVector> Initialize(TVector vector)
        {
            Vector = vector;
            return this;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
