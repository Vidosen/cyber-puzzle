using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class SwapVerticalRowsCommand : IMatrixCommand
    {
        public int FirstRowId { get; }
        public int SecondRowId { get; }
        private readonly ProtectMatrix _contextMatrix;

        public SwapVerticalRowsCommand(ProtectMatrix contextMatrix, int firstRowId, int secondRowId)
        {
            FirstRowId = firstRowId;
            SecondRowId = secondRowId;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            if (_contextMatrix.Size.y <= FirstRowId || _contextMatrix.Size.y <= SecondRowId)
                throw new InvalidOperationException();
            var firstRow = _contextMatrix.GetVerticalCells(FirstRowId);
            var secondRow = _contextMatrix.GetVerticalCells(SecondRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(cell.HorizontalId, SecondRowId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(cell.HorizontalId, FirstRowId);
            }
        }

        public void Cancel()
        {
            Execute();
        }
    }
}