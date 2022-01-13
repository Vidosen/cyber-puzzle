using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Operations;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Views.Cells
{
        public abstract class BaseCellView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public RectTransform Transform => _transform == null ? GetComponent<RectTransform>() : _transform;
        public abstract ICell Model { get; }

        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
        public IObservable<PointerEventData> OnDragObservable => _onDragSubject;
        public Vector2 UnscaledDeltaMove { get; private set; }
        
        private RectTransform _transform;
        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>();
        private Subject<PointerEventData> _onDragSubject = new Subject<PointerEventData>();
        private ScrollCommandsProcessor _scrollCommandsProcessor;
        protected Canvas _canvas;

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

        public abstract void Initialize(ICell cellModel, bool animateShow);
        public void Rescale(float scaleFactor)
        {
            if (scaleFactor > 0 && Transform != null)
            {
                Transform.sizeDelta = Transform.sizeDelta * scaleFactor;   
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UnscaledDeltaMove = Vector2.zero;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _scrollCommandsProcessor.FinishScroll(Model.HorizontalId, Model.VerticalId);
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
        
        public virtual async UniTask HideAnimation()
        {
            var destroyEffect = _transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutQuad);
            await destroyEffect.AsyncWaitForCompletion();
        }
    }
        public abstract class BaseCellView<TCell> : BaseCellView where TCell : ICell
        {
            public override ICell Model => _concrecteModel;
            protected TCell _concrecteModel;
            public override void Initialize(ICell cellModel, bool animateShow)
            {
                _concrecteModel = (TCell)cellModel;
            }
        }
}