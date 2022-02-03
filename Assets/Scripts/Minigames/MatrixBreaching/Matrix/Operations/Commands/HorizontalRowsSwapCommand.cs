using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Operations.Commands
{
    public class HorizontalRowsSwapCommand : BaseRowsSwapCommand
    {
        private readonly SignalBus _signalBus;
        private readonly GuardMatrix _contextMatrix;

        public HorizontalRowsSwapCommand(SignalBus signalBus, GuardMatrix contextMatrix)
        {
            _signalBus = signalBus;
            _contextMatrix = contextMatrix;
        }
        
        public override void Execute()
        {
            if (_contextMatrix.Size.y <= ApplyingRowId || _contextMatrix.Size.y <= AppliedRowId)
                throw new InvalidOperationException();
            
            var firstRow = _contextMatrix.GetHorizontalCells(ApplyingRowId);
            var secondRow = _contextMatrix.GetHorizontalCells(AppliedRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(cell.HorizontalId, AppliedRowId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(cell.HorizontalId, ApplyingRowId);
            }
            _signalBus.AbstractFire(new MatrixOperationsSignals.SwapOperationOccured(RowType.Horizontal, ApplyingRowId, AppliedRowId));
        }
    }
}