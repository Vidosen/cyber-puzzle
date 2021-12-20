using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class HorizontalRowScrollCommand : IMatrixCommand
    {
        public int VertRowId { get; }
        public int ScrollDelta { get; }
        private readonly ProtectMatrix _contextMatrix;

        public HorizontalRowScrollCommand(ProtectMatrix contextMatrix, int vertRowId, int scrollDelta)
        {
            VertRowId = vertRowId;
            ScrollDelta = scrollDelta;
            _contextMatrix = contextMatrix;
        }
        public void Execute()
        {
            ScrollRow(false);
        }

        private void ScrollRow(bool isReversed)
        {
            if (_contextMatrix.Size.y <= VertRowId)
                throw new InvalidOperationException();
            var row = _contextMatrix.GetHorizontalCells(VertRowId);
            foreach (var cell in row)
            {
                var newRowId = Mathf.RoundToInt(Mathf.Repeat(cell.HorizontalId + ScrollDelta * (isReversed ? -1 : 1),
                    _contextMatrix.Size.x));
                cell.Move(newRowId, cell.VerticalId);
            }
        }

        public void Cancel()
        {
            ScrollRow(true);
        }
    }
}