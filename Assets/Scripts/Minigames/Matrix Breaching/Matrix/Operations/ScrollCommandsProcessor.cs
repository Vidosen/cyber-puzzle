using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Minigames.MatrixBreaching.Matrix.Operations.Commands;
using Minigames.MatrixBreaching.Matrix.Signals;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations
{
    public class ScrollCommandsProcessor
    {
        private readonly GuardMatrix _guardMatrix;
        private readonly IMatrixCommand.Factory _commandFactory;
        private readonly SignalBus _signalBus;
        private readonly MatrixBreachingService _matrixBreachingService;
        public IReadOnlyReactiveProperty<bool> IsExecutingCommand => _isExecutingCommand.ToReadOnlyReactiveProperty();
        public RowType RowType { get; private set; }
        public int VerticalIndex { get; private set; }
        public int HorizontalIndex { get; private set; }
        public int ScrollDelta { get; private set; }

        public bool IsScrollOccured => VerticalIndex != -1 && Mathf.Abs(ScrollDelta) > 0;
        private ReactiveProperty<bool> _isExecutingCommand = new ReactiveProperty<bool>();

        public ScrollCommandsProcessor(GuardMatrix guardMatrix, IMatrixCommand.Factory commandFactory,
            SignalBus signalBus, MatrixBreachingService matrixBreachingService)
        {
            _guardMatrix = guardMatrix;
            _commandFactory = commandFactory;
            _signalBus = signalBus;
            _matrixBreachingService = matrixBreachingService;
        }
        
        public void StartScroll(RowType rowType, int cellHorizontalIndex, int cellVerticalIndex)
        {
            if (_matrixBreachingService.StatusReactiveProperty.Value != MatrixBreachingData.Status.Process)
            {
                Debug.LogWarning("Minigame isn't in progress!");
                return;
            }
            if (_isExecutingCommand.Value)
            {
                Debug.LogWarning("Scroll is already being executed at the moment!");
                return;
            }
            if (HasLockedCells(rowType, new Vector2Int(cellHorizontalIndex, cellVerticalIndex)))
                return;
            RowType = rowType;
            HorizontalIndex = cellHorizontalIndex;
            VerticalIndex = cellVerticalIndex;
            _isExecutingCommand.Value = true;
        }
        private bool HasLockedCells(RowType rowType, Vector2Int pos)
        {
            switch (rowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    return _guardMatrix.GetHorizontalCells(pos.y) .Any(cell => cell is LockCell lockCell && lockCell.IsLocked.Value);
                case RowType.Vertical:
                    return _guardMatrix.GetVerticalCells(pos.x).Any(cell => cell is LockCell lockCell && lockCell.IsLocked.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null);
            }
        }

        public void ApplyScroll(int scrollDelta)
        {
            if (_matrixBreachingService.StatusReactiveProperty.Value != MatrixBreachingData.Status.Process)
            {
                Debug.LogWarning("Minigame isn't in progress!");
                return;
            }
            if (!IsExecutingCommand.Value)
            {
                Debug.LogError("Scroll is not being executed at the moment!");
                return;
            }
            
            var command = GetScrollCommand(scrollDelta);
            command.Execute();
            ScrollDelta += scrollDelta;

            Debug.Log($"Executed preview scroll index {VerticalIndex} with delta {scrollDelta}");
        }
        

        private BaseRowScrollCommand GetScrollCommand(int scrollDelta)
        {
            var command = _commandFactory.Create(OperationType.Scroll, RowType) as BaseRowScrollCommand;
            switch (RowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    command.Initialize(VerticalIndex, scrollDelta);
                    break;
                case RowType.Vertical:
                    command.Initialize(HorizontalIndex, scrollDelta);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return command;
        }

        public void FinishScroll(int cellHorizontalIndex, int cellVerticalIndex)
        {
            if (HorizontalIndex != cellHorizontalIndex && VerticalIndex != cellVerticalIndex)
            {
                Debug.LogWarning($"Scroll is being executed with ({HorizontalIndex}, {VerticalIndex}) indices," +
                               $" but tried to finish with ({cellHorizontalIndex}, {cellVerticalIndex}) indices!");
                return;
            }
            _isExecutingCommand.Value = false;
            if (!IsScrollOccured)
            {
                Debug.Log("Swap cancelled");
            }
            else
            {
                _guardMatrix.Log();
                IList<ICell> appliedCells;
                switch (RowType)
                {
                    case RowType.None:
                        throw new InvalidOperationException();
                    case RowType.Horizontal:
                        appliedCells = _guardMatrix.GetHorizontalCells(VerticalIndex);
                        break;
                    case RowType.Vertical:
                        appliedCells = _guardMatrix.GetVerticalCells(HorizontalIndex);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _signalBus.Fire(new MatrixOperationsSignals.OperationApplied(appliedCells.ToArray()));
            }
            ResetState();
        }

        private void ResetState()
        {
            ScrollDelta = 0;
            VerticalIndex = -1;
            HorizontalIndex = -1;
            RowType = RowType.None;
        }
    }
}