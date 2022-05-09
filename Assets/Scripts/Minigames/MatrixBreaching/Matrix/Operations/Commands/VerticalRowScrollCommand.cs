using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations.Commands
{
    public class VerticalRowScrollCommand : BaseRowScrollCommand
    {
        private readonly GuardMatrix _contextMatrix;
        private readonly SignalBus _signalBus;

        public VerticalRowScrollCommand(GuardMatrix contextMatrix, SignalBus signalBus)
        {
            _contextMatrix = contextMatrix;
            _signalBus = signalBus;
        }

        protected override void ScrollRow(bool isReversed)
        {
            if (_contextMatrix.Size.x <= RowId)
                throw new InvalidOperationException();
            var row = _contextMatrix.GetVerticalCells(RowId);
            foreach (var cell in row)
            {
                var newRowId = Mathf.RoundToInt(Mathf.Repeat(cell.VerticalId + ScrollDelta * (isReversed ? -1 : 1),
                    _contextMatrix.Size.y));
                cell.Move(cell.HorizontalId, newRowId);
            }
            _signalBus.AbstractFire(new MatrixOperationsSignals.ScrollOperationOccured(RowType.Vertical, RowId));
        }
    }
}