using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
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
        
        private List<ValueCellView> _cells = new List<ValueCellView>();
        private CompositeDisposable _srollDisposable = new CompositeDisposable();
        private float _deltaCellScroll;
        private float _scrollThreshold;
        private ValueCellView _scrollingCell;

        public ScrollViewProcessor(GuardMatrixPresenter matrixPresenter, ScrollCommandsProcessor scrollCommandsProcessor)
        {
            _matrixPresenter = matrixPresenter;
            _scrollCommandsProcessor = scrollCommandsProcessor;
        }
        public void Initialize()
        {
            _canvas = _matrixPresenter.GetComponentInParent<Canvas>();

            _scrollCommandsProcessor.IsExecutingCommand.Where(isExecuting => isExecuting)
                .Subscribe(_ => OnStartedScroll()).AddTo(_compositeDisposable);
            _scrollCommandsProcessor.IsExecutingCommand.Where(isExecuting => !isExecuting)
                .Subscribe(_ => OnFinishedScroll()).AddTo(_compositeDisposable);
        }

        private void OnStartedScroll()
        {
            _scrollingCell = _matrixPresenter.GetCellView(_scrollCommandsProcessor.HorizontalIndex,
                _scrollCommandsProcessor.VerticalIndex);
            switch (_scrollCommandsProcessor.RowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    _cells.AddRange(_matrixPresenter.GetHorizontalCellViews(_scrollCommandsProcessor.VerticalIndex));
                    _scrollThreshold = _scrollingCell.Transform.sizeDelta.x * 0.4f;
                    break;
                case RowType.Vertical:
                    _cells.AddRange(_matrixPresenter.GetVerticalCellViews(_scrollCommandsProcessor.HorizontalIndex));
                    _scrollThreshold = _scrollingCell.Transform.sizeDelta.y * 0.4f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _scrollingCell.OnDragObservable.Subscribe(data => OnCellScroll(data)).AddTo(_srollDisposable);
            _matrixPresenter.CellViewsReplaced
                .Subscribe(
                    args =>
                    {
                        if (_cells.Contains(args.DisposedCellView))
                            _cells.Remove(args.DisposedCellView);
                        if (!_cells.Contains(args.NewCellView))
                            _cells.Add(args.NewCellView);
                    }).AddTo(_srollDisposable);
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
                    _cells.ForEach(cell => cell.Transform.anchoredPosition += Vector2.right * eventData.delta.x / _canvas.scaleFactor);
                    break;
                case RowType.Vertical:
                    _cells.ForEach(cell => cell.Transform.anchoredPosition += Vector2.up * eventData.delta.y / _canvas.scaleFactor);
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
            _cells.ForEach(cell=>_matrixPresenter.UpdateCellViewPos(cell));
            _srollDisposable.Clear();
            _cells.Clear();
        }
        

        public void Dispose()
        {
            _compositeDisposable.Clear();
        }
    }
}