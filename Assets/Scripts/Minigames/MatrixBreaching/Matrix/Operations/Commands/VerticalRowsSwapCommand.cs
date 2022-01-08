using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class VerticalRowsSwapCommand : BaseRowsSwapCommand
    {
        private readonly SignalBus _signalBus;
        private readonly GuardMatrix _contextMatrix;

        public VerticalRowsSwapCommand(SignalBus signalBus, GuardMatrix contextMatrix)
        {
            _signalBus = signalBus;
            _contextMatrix = contextMatrix;
        }
        public override void Execute()
        {
            if (_contextMatrix.Size.x <= ApplyingRowId || _contextMatrix.Size.x <= AppliedRowId)
                throw new InvalidOperationException();
            var firstRow = _contextMatrix.GetVerticalCells(ApplyingRowId);
            var secondRow = _contextMatrix.GetVerticalCells(AppliedRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(AppliedRowId, cell.VerticalId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(ApplyingRowId, cell.VerticalId);
            }
            _signalBus.AbstractFire(new MatrixOperationsSignals.SwapOperationOccured(RowType.Vertical, ApplyingRowId, AppliedRowId));
        }
    }
}