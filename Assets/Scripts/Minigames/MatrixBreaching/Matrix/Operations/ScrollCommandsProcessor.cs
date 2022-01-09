using System;
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
    public class ScrollCommandsProcessor
    {
        private readonly GuardMatrix _guardMatrix;
        private readonly IMatrixCommand.Factory _commandFactory;
        private readonly SignalBus _signalBus;
        public IReadOnlyReactiveProperty<bool> IsExecutingCommand => _isExecutingCommand.ToReadOnlyReactiveProperty();
        public RowType RowType { get; private set; }
        public int VerticalIndex { get; private set; }
        public int HorizontalIndex { get; private set; }
        public int ScrollDelta { get; private set; }

        public bool IsScrollOccured => VerticalIndex != -1 && Mathf.Abs(ScrollDelta) > 0;
        private ReactiveProperty<bool> _isExecutingCommand = new ReactiveProperty<bool>();

        public ScrollCommandsProcessor(GuardMatrix guardMatrix, IMatrixCommand.Factory commandFactory, SignalBus signalBus)
        {
            _guardMatrix = guardMatrix;
            _commandFactory = commandFactory;
            _signalBus = signalBus;
        }
        
        public void StartScroll(RowType rowType, int cellHorizontalIndex, int cellVerticalIndex)
        {
            if (_isExecutingCommand.Value)
            {
                Debug.LogWarning("Scroll is already being executed at the moment!");
                return;
            }
            RowType = rowType;
            HorizontalIndex = cellHorizontalIndex;
            VerticalIndex = cellVerticalIndex;
            _isExecutingCommand.Value = true;
        }
        
        public void ApplyScroll(int scrollDelta)
        {
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
                Debug.LogError($"Scroll is being executed with ({HorizontalIndex}, {VerticalIndex}) indices," +
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
                _signalBus.Fire<MatrixOperationsSignals.OperationApplied>();
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