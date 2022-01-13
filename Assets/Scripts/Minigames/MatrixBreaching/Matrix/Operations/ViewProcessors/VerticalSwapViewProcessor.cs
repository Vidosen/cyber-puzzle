using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Matrix.Views.Cells;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations.ViewProcessors
{
    public class VerticalSwapViewProcessor : IInitializable, IDisposable
    {
        private readonly SwapCommandsProcessor _swapCommandsProcessor;
        private readonly GuardMatrixPresenter _matrixPresenter;
        private readonly GuardMatrix _matrix;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly CompositeDisposable _swapDisposable = new CompositeDisposable();
        private GuardMatrixExchangerView _exchanger;
        private List<BaseCellView> _cells = new List<BaseCellView>();
        private Canvas _canvas;

        public VerticalSwapViewProcessor(SwapCommandsProcessor swapCommandsProcessor, GuardMatrixPresenter matrixPresenter, GuardMatrix matrix)
        {
            _swapCommandsProcessor = swapCommandsProcessor;
            _matrixPresenter = matrixPresenter;
            _matrix = matrix;
        }

        public void Initialize()
        {
            _canvas = _matrixPresenter.GetComponentInParent<Canvas>();
            
           _swapCommandsProcessor.IsExecutingCommand
                .Where(isExecuting => isExecuting && _swapCommandsProcessor.RowType == RowType.Vertical)
                .Subscribe(_ => StartSwap()).AddTo(_compositeDisposable);
           _swapCommandsProcessor.IsExecutingCommand
               .Where(isExecuting => !isExecuting && _swapCommandsProcessor.RowType == RowType.Vertical)
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
            _cells.AddRange(_matrixPresenter.GetVerticalCellViews(horizontalId));
            _exchanger = _matrixPresenter.GetVerticalExchangerView(horizontalId);
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
                _matrixPresenter.VerticalRowViews
                    .Where(row =>
                    {
                        var rowRect = row.Transform.rect;
                        rowRect.position = row.Transform.anchoredPosition;
                        return exchangerRect.Overlaps(rowRect);
                    })
                    .Where(row=> row.Index != _exchanger.RowIndex)
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
                    cellView.Transform.anchoredPosition = _exchanger.Transform.anchoredPosition + Vector2.down *
                    ((cellView.Transform.sizeDelta.y + _matrixPresenter.CellsOffset) *
                     (cellView.Model.VerticalId + 1));
            }
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}