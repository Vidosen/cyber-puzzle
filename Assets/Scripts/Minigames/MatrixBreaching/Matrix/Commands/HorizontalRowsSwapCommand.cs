using System;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class HorizontalRowsSwapCommand : IMatrixCommand
    {
        public int FirstVertRowId { get; }
        public int SecondVertRowId { get; }
        private readonly ProtectMatrix _contextMatrix;

        public HorizontalRowsSwapCommand(ProtectMatrix contextMatrix, int firstVertRowId, int secondVertRowId)
        {
            FirstVertRowId = firstVertRowId;
            SecondVertRowId = secondVertRowId;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            if (_contextMatrix.Size.y <= FirstVertRowId || _contextMatrix.Size.y <= SecondVertRowId)
                throw new InvalidOperationException();
            
            var firstRow = _contextMatrix.GetHorizontalCells(FirstVertRowId);
            var secondRow = _contextMatrix.GetHorizontalCells(SecondVertRowId);
            foreach (var cell in firstRow)
            {
                cell.Move(cell.HorizontalId, SecondVertRowId);
            }
            foreach (var cell in secondRow)
            {
                cell.Move(cell.HorizontalId, FirstVertRowId);
            }
        }

        public void Cancel()
        {
            Execute();
        }
    }
}