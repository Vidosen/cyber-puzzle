using Minigames.MatrixBreaching.Matrix.Commands;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix
{
    public class SwapCommandsProcessor
    {
        private readonly GuardMatrix _guardMatrix;
        private readonly IMatrixCommand.Factory _commandFactory;
        private readonly SignalBus _signalBus;
        public IReadOnlyReactiveProperty<bool> IsExecutingCommand => _isExecutingCommand.ToReadOnlyReactiveProperty();
        public RowType RowType { get; private set; }
        public int ApplyingRowIndex { get; private set; }
        public int AppliedRowIndex { get; private set; }

        public bool IsSwapOccured => AppliedRowIndex != -1 && ApplyingRowIndex != AppliedRowIndex;
        private ReactiveProperty<bool> _isExecutingCommand = new ReactiveProperty<bool>();
        
        private IMatrixCommand _lastMatrixCommand;

        public SwapCommandsProcessor(GuardMatrix guardMatrix, IMatrixCommand.Factory commandFactory, SignalBus signalBus)
        {
            _guardMatrix = guardMatrix;
            _commandFactory = commandFactory;
            _signalBus = signalBus;
        }
        
        public void StartSwap(RowType rowType, int index)
        {
            if (_isExecutingCommand.Value)
            {
                Debug.LogWarning("Swipe is already being executed at the moment!");
                return;
            }
            RowType = rowType;
            AppliedRowIndex = ApplyingRowIndex = index;
            _isExecutingCommand.Value = true;
        }
        
        public void ApplyTo(int secondIndex)
        {
            if (!IsExecutingCommand.Value)
            {
                Debug.LogError("Swap is not being executed at the moment!");
                return;
            }
            TryCancel();
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
                Debug.LogError($"Swap is being executed with {ApplyingRowIndex} index," +
                               $" but tried to finish with {index} index!");
                return;
            }
            if (!IsSwapOccured)
            {
                Debug.Log("Swap cancelled");
            }
            else
            {
                _guardMatrix.Log();
                _signalBus.Fire<MatrixOperationsSignals.OperationApplied>();
            }
            
            _isExecutingCommand.Value = false;
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