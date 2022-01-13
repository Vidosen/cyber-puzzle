using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Operations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Views
{
    public class GuardMatrixExchangerView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _indexText;
        [SerializeField] private string _indexTextFormat;
        [SerializeField] private RowType _rowType;
        private RectTransform _transform;
        private SwapCommandsProcessor _swapCommandsProcessor;
        private Subject<PointerEventData> _onDragSubject = new Subject<PointerEventData>();

        public RowType RowType => _rowType;
        public RectTransform Transform => _transform;
        public int RowIndex { get; private set; } = -1;
        public bool IsMoving { get; private set; }

        public IObservable<PointerEventData> OnDragObservable => _onDragSubject;

        [Inject]
        private void Construct(SwapCommandsProcessor swapCommandsProcessor)
        {
            _swapCommandsProcessor = swapCommandsProcessor;
        }

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void Initialize(int index)
        {
            ChangeRowIndex(index);
        }
        public void Rescale(float scaleFactor)
        {
            if (scaleFactor > 0 && Transform != null)
            {
                Transform.sizeDelta = Transform.sizeDelta * scaleFactor;   
            }
        }

        public void ChangeRowIndex(int index)
        {
            RowIndex = index;
            _indexText.text = string.Format(_indexTextFormat, RowIndex);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsMoving = true;
            _swapCommandsProcessor.StartSwap(RowType, RowIndex);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsMoving = false;
            _swapCommandsProcessor.FinishSwap(RowIndex);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _onDragSubject.OnNext(eventData);
        }
    }
}