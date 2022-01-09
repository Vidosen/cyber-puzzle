using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Views;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations.ViewProcessors
{
    public class HorizontalSwapViewProcessor : IInitializable, IDisposable
    {
        private readonly SwapCommandsProcessor _swapCommandsProcessor;
        private readonly GuardMatrixPresenter _matrixPresenter;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly CompositeDisposable _swapDisposable = new CompositeDisposable();
        private GuardMatrixExchangerView _exchanger;
        private List<ValueCellView> _cells = new List<ValueCellView>();
        private Canvas _canvas;

        public HorizontalSwapViewProcessor(SwapCommandsProcessor swapCommandsProcessor, GuardMatrixPresenter matrixPresenter)
        {
            _swapCommandsProcessor = swapCommandsProcessor;
            _matrixPresenter = matrixPresenter;
        }

        public void Initialize()
        {
            _canvas = _matrixPresenter.GetComponentInParent<Canvas>();
            
            _swapCommandsProcessor.IsExecutingCommand
                .Where(isExecuting => isExecuting && _swapCommandsProcessor.RowType == RowType.Horizontal)
                .Subscribe(_ => StartSwap()).AddTo(_compositeDisposable);
            _swapCommandsProcessor.IsExecutingCommand
                .Where(isExecuting => !isExecuting && _swapCommandsProcessor.RowType == RowType.Horizontal)
                .Subscribe(_ => EndSwap()).AddTo(_compositeDisposable);
        }

        private void EndSwap()
        {
            _swapDisposable.Clear();
            if (_exchanger != null)
            {
                if (_swapCommandsProcessor.IsSwapOccured)
                    _exchanger.ChangeRowIndex(_swapCommandsProcessor.AppliedRowIndex);
                
                _matrixPresenter.UpdateExchangerViewPos(_exchanger);
            }
            _cells.ForEach(cell=>_matrixPresenter.UpdateCellViewPos(cell));
            _cells.Clear();
        }

        private void StartSwap()
        {
            _cells.Clear();
            var horizontalId = _swapCommandsProcessor.ApplyingRowIndex;
            _cells.AddRange(_matrixPresenter.GetHorizontalCellViews(horizontalId));
            _exchanger = _matrixPresenter.GetHorizontalExchangerView(horizontalId);
            _exchanger.OnDragObservable
                .Subscribe(data => OnSwapProgress(data))
                .AddTo(_swapDisposable);
            OnSwapProgress(new PointerEventData(EventSystem.current));
            
            _matrixPresenter.CellViewsReplaced
                .Subscribe(
                    args =>
                    {
                        if (_cells.Contains(args.DisposedCellView))
                            _cells.Remove(args.DisposedCellView);
                        if (!_cells.Contains(args.NewCellView))
                            _cells.Add(args.NewCellView);
                    }).AddTo(_swapDisposable);
        }

        private void OnSwapProgress(PointerEventData eventData)
        {
            CheckSwapAblity();
            UpdateSwapPositions(eventData);
        }

        private void CheckSwapAblity()
        {
            var exchangerRect = _exchanger.Transform.rect;
            exchangerRect.position = _exchanger.Transform.anchoredPosition;
            var overlappedRows =
                _matrixPresenter.HorizontalRowViews
                    .Where(row =>
                    {
                        var rowRect = row.Transform.rect;
                        rowRect.position = row.Transform.anchoredPosition;
                        return row.Index != _exchanger.RowIndex && exchangerRect.Overlaps(rowRect);
                    })
                    .OrderBy(row => row.Index)
                    .ToList();

            if (overlappedRows.Count <= 0)
            {
                _swapCommandsProcessor.TryCancel();
                return;
            }

            var foundRow =
                overlappedRows.First();
            if (foundRow.Index != _swapCommandsProcessor.AppliedRowIndex)
            {
                _swapCommandsProcessor.ApplyTo(foundRow.Index);
            }
        }

        private void UpdateSwapPositions(PointerEventData eventData)
        {
            _exchanger.Transform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            foreach (var cellView in _cells)
            {
                if (cellView != null)
                    cellView.Transform.anchoredPosition = _exchanger.Transform.anchoredPosition + Vector2.right *
                    ((cellView.Transform.sizeDelta.x + _matrixPresenter.CellsOffset) *
                     (cellView.Model.HorizontalId + 1));
            }
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}