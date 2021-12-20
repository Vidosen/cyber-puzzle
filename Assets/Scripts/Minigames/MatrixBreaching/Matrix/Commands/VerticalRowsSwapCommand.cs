using System;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class VerticalRowsSwapCommand : IMatrixCommand
    {
        public int FirstHorizRowId { get; }
        public int SecondHorizRowId { get; }
        private readonly ProtectMatrix _contextMatrix;

        public VerticalRowsSwapCommand(ProtectMatrix contextMatrix, int firstHorizRowId, int secondHorizRowId)
        {
            FirstHorizRowId = firstHorizRowId;
            SecondHorizRowId = secondHorizRowId;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            if (_contextMatrix.Size.x <= FirstHorizRowId || _contextMatrix.Size.x <= SecondHorizRowId)
                throw new InvalidOperationException();
            var firstRow = _contextMatrix.GetVerticalCells(FirstHorizRowId);
            var secondRow = _contextMatrix.GetVerticalCells(SecondHorizRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(SecondHorizRowId, cell.VerticalId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(FirstHorizRowId, cell.VerticalId);
            }
        }

        public void Cancel()
        {
            Execute();
        }
    }
}