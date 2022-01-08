using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Views
{
    public class ValueCellView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private TextMeshProUGUI _valueText;
        public RectTransform Transform => _transform == null ? GetComponent<RectTransform>() : _transform;
        public ICell Model => _cellModel;

        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
        public IObservable<PointerEventData> OnDragObservable => _onDragSubject;
        public Vector2 UnscaledDeltaMove { get; private set; }

        private ValueCell _cellModel;
        private RectTransform _transform;
        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>();
        private Subject<PointerEventData> _onDragSubject = new Subject<PointerEventData>();
        private ScrollCommandsProcessor _scrollCommandsProcessor;
        private Canvas _canvas;

        [Inject]
        private void Construct(ScrollCommandsProcessor scrollCommandsProcessor)
        {
            _scrollCommandsProcessor = scrollCommandsProcessor;
        }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _transform = GetComponent<RectTransform>();
        }
        public void Initialize(ICell cellModel, bool animate)
        {
            _cellModel = cellModel as ValueCell;
            UpdateView();
            if (animate)
            {
                _transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutQuad);
            }
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
            _valueText.text = _cellModel.Value.ToTextString();
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
            if (!_scrollCommandsProcessor.IsExecutingCommand.Value)
                CheckStartScrollAbility();
        }
                
        private void CheckStartScrollAbility()
        {
            var scaleFactor = _canvas.scaleFactor;
            var horizontalDeltaScroll = UnscaledDeltaMove.x / scaleFactor;
            var verticalDeltaScroll = UnscaledDeltaMove.y / scaleFactor;
            if (Mathf.Abs(horizontalDeltaScroll) > _transform.sizeDelta.x  * 0.05f)
            {
                _scrollCommandsProcessor.StartScroll(RowType.Horizontal, Model.HorizontalId,
                    Model.VerticalId);
                return;
            }
            if (Mathf.Abs(verticalDeltaScroll) > _transform.sizeDelta.y * 0.05f)
            {
                _scrollCommandsProcessor.StartScroll(RowType.Vertical, Model.HorizontalId,
                    Model.VerticalId);
            }
        }
        
        public async UniTask HideAnimation()
        {
            var destroyEffect = _transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutQuad);
            await destroyEffect.AsyncWaitForCompletion();
        }
    }
}