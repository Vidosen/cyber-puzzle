using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class ScrollVerticalRowCommand : IMatrixCommand
    {
        public int RowId { get; }
        public int ScrollDelta { get; }
        private readonly ProtectMatrix _contextMatrix;

        public ScrollVerticalRowCommand(ProtectMatrix contextMatrix, int rowId, int scrollDelta)
        {
            RowId = rowId;
            ScrollDelta = scrollDelta;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            ScrollRow(false);
        }

        private void ScrollRow(bool isReversed)
        {
            if (_contextMatrix.Size.y <= RowId)
                throw new InvalidOperationException();
            var row = _contextMatrix.GetVerticalCells(RowId);
            foreach (var cell in row)
            {
                var newRowId = Mathf.RoundToInt(Mathf.Repeat(cell.VerticalId + ScrollDelta * (isReversed ? -1 : 1),
                    _contextMatrix.Size.y));
                cell.Move(cell.HorizontalId, newRowId);
            }
        }

        public void Cancel()
        {
            ScrollRow(true);
        }
    }
}