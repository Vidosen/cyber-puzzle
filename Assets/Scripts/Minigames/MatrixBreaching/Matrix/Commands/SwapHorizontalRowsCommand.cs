using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class SwapHorizontalRowsCommand : IMatrixCommand
    {
        public int FirstRowId { get; }
        public int SecondRowId { get; }
        private readonly ProtectMatrix _contextMatrix;

        public SwapHorizontalRowsCommand(ProtectMatrix contextMatrix, int firstRowId, int secondRowId)
        {
            FirstRowId = firstRowId;
            SecondRowId = secondRowId;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            if (_contextMatrix.Size.x <= FirstRowId || _contextMatrix.Size.x <= SecondRowId)
                throw new InvalidOperationException();
            
            var firstRow = _contextMatrix.GetHorizontalCells(FirstRowId);
            var secondRow = _contextMatrix.GetHorizontalCells(SecondRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(SecondRowId, cell.VerticalId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(FirstRowId, cell.VerticalId);
            }
        }

        public void Cancel()
        {
            Execute();
        }
    }
}