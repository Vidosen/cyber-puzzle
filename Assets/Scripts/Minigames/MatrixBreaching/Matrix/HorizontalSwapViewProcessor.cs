using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Views;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix
{
    public class HorizontalSwapViewProcessor : IInitializable, IDisposable
    {
        private readonly SwapCommandsProcessor _swapCommandsProcessor;
        private readonly GuardMatrixPresenter _guardMatrixPresenter;
        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private IDisposable _swapProgressStream;
        private GuardMatrixExchangerView _exchanger;
        private List<ValueCellView> _cells = new List<ValueCellView>();
        private Canvas _canvas;

        public HorizontalSwapViewProcessor(SwapCommandsProcessor swapCommandsProcessor, GuardMatrixPresenter guardMatrixPresenter)
        {
            _swapCommandsProcessor = swapCommandsProcessor;
            _guardMatrixPresenter = guardMatrixPresenter;
        }

        public void Initialize()
        {
            _canvas = _guardMatrixPresenter.GetComponentInParent<Canvas>();
            
            _swapCommandsProcessor.IsExecutingCommand
                .Where(isExecuting => isExecuting && _swapCommandsProcessor.RowType == RowType.Horizontal)
                .Subscribe(_ => StartSwap()).AddTo(_compositeDisposable);
            _swapCommandsProcessor.IsExecutingCommand
                .Where(isExecuting => !isExecuting && _swapCommandsProcessor.RowType == RowType.Horizontal)
                .Subscribe(_ => EndSwap()).AddTo(_compositeDisposable);
        }

        private void EndSwap()
        {
            _swapProgressStream?.Dispose();
            if (_exchanger != null)
            {
                if (_swapCommandsProcessor.IsSwapOccured)
                    _exchanger.ChangeRowIndex(_swapCommandsProcessor.AppliedRowIndex);
                
                _guardMatrixPresenter.UpdateExchangerViewPos(_exchanger);
            }
            _cells.ForEach(_guardMatrixPresenter.UpdateCellViewPos);
            _cells.Clear();
        }

        private void StartSwap()
        {
            var horizontalId = _swapCommandsProcessor.ApplyingRowIndex;
            _cells.AddRange(_guardMatrixPresenter.GetHorizontalCellViews(horizontalId));
            _exchanger = _guardMatrixPresenter.GetHorizontalExchangerView(horizontalId);
            _swapProgressStream = _exchanger.OnDragObservable
                .Subscribe(data => OnSwapProgress(data));
            OnSwapProgress(new PointerEventData(EventSystem.current));
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
                _guardMatrixPresenter.HorizontalRowViews
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
                cellView.Transform.anchoredPosition = _exchanger.Transform.anchoredPosition + Vector2.right *
                    ((cellView.Transform.sizeDelta.x + _guardMatrixPresenter.CellsOffset) *
                     (cellView.Model.HorizontalId + 1));
            }
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}