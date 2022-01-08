using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Views;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations.ViewProcessors
{
    public class ScrollViewProcessor : IInitializable, IDisposable
    {
        private readonly GuardMatrixPresenter _matrixPresenter;
        private readonly ScrollCommandsProcessor _scrollCommandsProcessor;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private Canvas _canvas;
        
        private List<ValueCellView> _scrollingNeighbourCells = new List<ValueCellView>();
        private IDisposable _scrollStream;
        private float _deltaCellScroll;
        private float _scrollThreshold;
        private ValueCellView _scrollingCell;

        public ScrollViewProcessor(GuardMatrixPresenter matrixPresenter, ScrollCommandsProcessor scrollCommandsProcessor)
        {
            _matrixPresenter = matrixPresenter;
            _scrollCommandsProcessor = scrollCommandsProcessor;
        }
        public async void Initialize()
        {
            _canvas = _matrixPresenter.GetComponentInParent<Canvas>();
            await _matrixPresenter.IsInitialized.Where(isInit => isInit).ToUniTask(true);
            
            foreach (var cellView in _matrixPresenter.CellViews)
            {
                cellView.OnDragObservable
                    .Where(_=>!_scrollCommandsProcessor.IsExecutingCommand.Value)
                    .Subscribe(_ => CheckStartScrollAbility(cellView))
                    .AddTo(_compositeDisposable);
            }

            _scrollCommandsProcessor.IsExecutingCommand.Where(isExecuting => isExecuting)
                .Subscribe(_ => OnStartedScroll()).AddTo(_compositeDisposable);
            _scrollCommandsProcessor.IsExecutingCommand.Where(isExecuting => !isExecuting)
                .Subscribe(_ => OnFinishedScroll()).AddTo(_compositeDisposable);
        }

        private void OnStartedScroll()
        {
            _scrollingCell = _matrixPresenter.GeCellView(_scrollCommandsProcessor.HorizontalIndex,
                _scrollCommandsProcessor.VerticalIndex);
            switch (_scrollCommandsProcessor.RowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    _scrollingNeighbourCells.AddRange(_matrixPresenter.GetHorizontalCellViews(_scrollCommandsProcessor.VerticalIndex));
                    _scrollThreshold = _scrollingCell.Transform.sizeDelta.x * 0.4f;
                    break;
                case RowType.Vertical:
                    _scrollingNeighbourCells.AddRange(_matrixPresenter.GetVerticalCellViews(_scrollCommandsProcessor.HorizontalIndex));
                    _scrollThreshold = _scrollingCell.Transform.sizeDelta.y * 0.4f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _scrollStream = _scrollingCell.OnDragObservable.Subscribe(data => OnCellScroll(data));
        }

        private void OnCellScroll(PointerEventData data)
        {
            if (_matrixPresenter.TryGetCellMoveAnimationTweener(_scrollingCell, out var tweener) &&
                tweener.IsActive() && tweener.IsPlaying())
                return;
            
            switch (_scrollCommandsProcessor.RowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    _deltaCellScroll += data.delta.x / _canvas.scaleFactor;
                    break;
                case RowType.Vertical:
                    _deltaCellScroll -= data.delta.y / _canvas.scaleFactor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CheckScrollAbility();
            UpdateScrollPositions(data);
        }

        private void UpdateScrollPositions(PointerEventData eventData)
        {
            switch (_scrollCommandsProcessor.RowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    _scrollingNeighbourCells.ForEach(cell => cell.Transform.anchoredPosition += Vector2.right * eventData.delta.x / _canvas.scaleFactor);
                    break;
                case RowType.Vertical:
                    _scrollingNeighbourCells.ForEach(cell => cell.Transform.anchoredPosition += Vector2.up * eventData.delta.y / _canvas.scaleFactor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckScrollAbility()
        {
            if (Mathf.Abs(_deltaCellScroll) > _scrollThreshold)
            {
                var scrollValue = Mathf.RoundToInt(_deltaCellScroll / _scrollThreshold);
                _scrollCommandsProcessor.ApplyScroll(scrollValue);
                _deltaCellScroll -= scrollValue * _scrollThreshold;
            }
        }

        private void OnFinishedScroll()
        {
            _scrollingNeighbourCells.ForEach(cell=>_matrixPresenter.UpdateCellViewPos(cell));
            _scrollStream?.Dispose();
            _scrollingNeighbourCells.Clear();
        }

        private void CheckStartScrollAbility(ValueCellView cellView)
        {
            var scaleFactor = _canvas.scaleFactor;
            var horizontalDeltaScroll = cellView.UnscaledDeltaMove.x / scaleFactor;
            var verticalDeltaScroll = cellView.UnscaledDeltaMove.y / scaleFactor;
            if (Mathf.Abs(horizontalDeltaScroll) > cellView.Transform.sizeDelta.x  * 0.05f)
            {
                _scrollCommandsProcessor.StartScroll(RowType.Horizontal, cellView.Model.HorizontalId,
                    cellView.Model.VerticalId);
                return;
            }
            if (Mathf.Abs(verticalDeltaScroll) > cellView.Transform.sizeDelta.y * 0.05f)
            {
                _scrollCommandsProcessor.StartScroll(RowType.Vertical, cellView.Model.HorizontalId,
                    cellView.Model.VerticalId);
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
        }
    }
}