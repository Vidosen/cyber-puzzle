using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public abstract class BaseLineView : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public BaseLineSlotView LineSlotView { get; private set; }
        public abstract Vector2 SnapDirection { get; protected set; }
        
        [SerializeField] protected TextMeshProUGUI LineText;
        private Canvas _canvas;
        private RectTransform _thisTransform;

        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;

        private void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            LineSlotView = GetComponentInParent<BaseLineSlotView>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            ThisTransform.anchoredPosition += SnapDirection * Vector2.Dot(SnapDirection, eventData.delta) / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ThisTransform.localPosition = Vector3.zero;
            Debug.Log("BaseLineView.OnEndDrag");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.LogWarning("BaseLineView.OnBeginDrag not implemented");
        }
    }
}
