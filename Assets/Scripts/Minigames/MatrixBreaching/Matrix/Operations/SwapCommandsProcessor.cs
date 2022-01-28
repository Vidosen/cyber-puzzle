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
    public class SwapCommandsProcessor
    {
        private readonly GuardMatrix _guardMatrix;
        private readonly IMatrixCommand.Factory _commandFactory;
        private readonly SignalBus _signalBus;
        private readonly MatrixBreachingService _matrixBreachingService;
        public IReadOnlyReactiveProperty<bool> IsExecutingCommand => _isExecutingCommand.ToReadOnlyReactiveProperty();
        public RowType RowType { get; private set; }
        public int ApplyingRowIndex { get; private set; }
        public int AppliedRowIndex { get; private set; }

        public bool IsSwapOccured => AppliedRowIndex != -1 && ApplyingRowIndex != AppliedRowIndex;
        private ReactiveProperty<bool> _isExecutingCommand = new ReactiveProperty<bool>();
        
        private IMatrixCommand _lastMatrixCommand;

        public SwapCommandsProcessor(GuardMatrix guardMatrix, IMatrixCommand.Factory commandFactory,
            SignalBus signalBus, MatrixBreachingService matrixBreachingService)
        {
            _guardMatrix = guardMatrix;
            _commandFactory = commandFactory;
            _signalBus = signalBus;
            _matrixBreachingService = matrixBreachingService;
        }
        
        public void StartSwap(RowType rowType, int index)
        {
            if (_matrixBreachingService.StatusReactiveProperty.Value != MatrixBreachingData.Status.Process)
            {
                Debug.LogWarning("Minigame isn't in progress!");
                return;
            }
            
            if (_isExecutingCommand.Value)
            {
                Debug.LogWarning("Swipe is already being executed at the moment!");
                return;
            }

            if (HasLockedCells(rowType, index))
                return;
            
            RowType = rowType;
            AppliedRowIndex = ApplyingRowIndex = index;
            _isExecutingCommand.Value = true;
        }

        private bool HasLockedCells(RowType rowType, int index)
        {
            switch (rowType)
            {
                case RowType.None:
                    throw new InvalidOperationException();
                case RowType.Horizontal:
                    return _guardMatrix.GetHorizontalCells(index) .Any(cell => cell is LockCell lockCell && lockCell.IsLocked.Value);
                case RowType.Vertical:
                        return _guardMatrix.GetVerticalCells(index).Any(cell => cell is LockCell lockCell && lockCell.IsLocked.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null);
            }
        }

        public void ApplyTo(int secondIndex)
        {
            if (_matrixBreachingService.StatusReactiveProperty.Value != MatrixBreachingData.Status.Process)
            {
                Debug.LogWarning("Minigame isn't in progress!");
                return;
            }
            if (!IsExecutingCommand.Value)
            {
                Debug.LogError("Swap is not being executed at the moment!");
                return;
            }
            TryCancel();
            if (HasLockedCells(RowType, secondIndex))
                return;

            var command = GetSwapCommand(secondIndex);
            command.Execute();

            _lastMatrixCommand = command;
            AppliedRowIndex = secondIndex;
            
            Debug.Log($"Executed preview swap index {ApplyingRowIndex} with {AppliedRowIndex}");
        }

        public void TryCancel()
        {
            if (_lastMatrixCommand != null)
            {
                _lastMatrixCommand.Cancel();
                Debug.Log($"Reversed preview swap index {ApplyingRowIndex} with {AppliedRowIndex}");
                _lastMatrixCommand = null;
            }
            AppliedRowIndex = -1;
        }

        private IMatrixCommand GetSwapCommand(int secondIndex)
        {
            var command = _commandFactory.Create(OperationType.Swap, RowType) as BaseRowsSwapCommand;
            command.Initialize(ApplyingRowIndex, secondIndex);
            return command;
        }

        public void FinishSwap(int index)
        {
            if (ApplyingRowIndex != index)
            {
                Debug.LogWarning($"Swap is being executed with {ApplyingRowIndex} index," +
                               $" but tried to finish with {index} index!");
                return;
            }
            _isExecutingCommand.Value = false;
            if (!IsSwapOccured)
            {
                Debug.Log("Swap cancelled");
            }
            else
            {
                _guardMatrix.Log();
                IList<ICell> applyingCells;
                IList<ICell> appliedCells;
                switch (RowType)
                {
                    case RowType.None:
                        throw new InvalidOperationException();
                    case RowType.Horizontal:
                        applyingCells = _guardMatrix.GetHorizontalCells(ApplyingRowIndex);
                        appliedCells = _guardMatrix.GetHorizontalCells(AppliedRowIndex);
                        break;
                    case RowType.Vertical:
                        applyingCells = _guardMatrix.GetVerticalCells(ApplyingRowIndex);
                        appliedCells = _guardMatrix.GetVerticalCells(AppliedRowIndex);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _signalBus.Fire(new MatrixOperationsSignals.OperationApplied(applyingCells.Concat(appliedCells).ToArray()));
            }
            ResetState();
            
        }
        private void ResetState()
        {
            _lastMatrixCommand = null;
            ApplyingRowIndex = -1;
            RowType = RowType.None;
        }
    }
}