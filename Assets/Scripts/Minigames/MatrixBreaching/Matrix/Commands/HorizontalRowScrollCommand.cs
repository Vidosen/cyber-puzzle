using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class HorizontalRowScrollCommand : BaseRowScrollCommand
    {
        private readonly GuardMatrix _contextMatrix;
        private readonly SignalBus _signalBus;

        public HorizontalRowScrollCommand(GuardMatrix contextMatrix, SignalBus signalBus)
        {
            _contextMatrix = contextMatrix;
            _signalBus = signalBus;
        }

        protected override void ScrollRow(bool isReversed)
        {
            if (_contextMatrix.Size.y <= RowId)
                throw new InvalidOperationException();
            var row = _contextMatrix.GetHorizontalCells(RowId);
            foreach (var cell in row)
            {
                var newRowId = Mathf.RoundToInt(Mathf.Repeat(cell.HorizontalId + ScrollDelta * (isReversed ? -1 : 1),
                    _contextMatrix.Size.x));
                cell.Move(newRowId, cell.VerticalId);
            }
            _signalBus.Fire(new MatrixOperationsSignals.ScrollOperationOccured(RowType.Horizontal, RowId));
        }
    }
}