using System;
using Minigames.MatrixBreaching.Matrix;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Views
{
    public class ValueCellView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private TextMeshProUGUI _valueText;
        public RectTransform Transform => _transform == null? GetComponent<RectTransform>() : _transform;
        public ICell Model => _cellModel;

        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
        public IObservable<PointerEventData> OnDragObservable => _onDragSubject;
        public Vector2 UnscaledDeltaMove { get; private set; }

        private ValueCell _cellModel;
        private RectTransform _transform;
        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>();
        private Subject<PointerEventData> _onDragSubject = new Subject<PointerEventData>();
        private ScrollCommandsProcessor _scrollCommandsProcessor;

        [Inject]
        private void Construct(ScrollCommandsProcessor scrollCommandsProcessor)
        {
            _scrollCommandsProcessor = scrollCommandsProcessor;
        }

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }
        public void Initialize(ICell cellModel)
        {
            _cellModel = cellModel as ValueCell;
            UpdateView();
        }
        public void Rescale(float scaleFactor)
        {
            if (scaleFactor > 0 && Transform != null)
            {
                Transform.sizeDelta = Transform.sizeDelta * scaleFactor;   
            }
        }

        private void UpdateView()
        {
            _valueText.text = ((int)_cellModel.ValueType).ToString();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UnscaledDeltaMove = Vector2.zero;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _scrollCommandsProcessor.FinishScroll(_cellModel.HorizontalId, _cellModel.VerticalId);
            UnscaledDeltaMove = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            UnscaledDeltaMove += eventData.delta;
            _onDragSubject.OnNext(eventData);
        }
    }
}