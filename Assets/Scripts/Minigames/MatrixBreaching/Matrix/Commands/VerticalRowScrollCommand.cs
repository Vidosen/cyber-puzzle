using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public class VerticalRowScrollCommand : IMatrixCommand
    {
        public int HorizRowId { get; private set; }
        public int ScrollDelta { get; private set; }
        private readonly GuardMatrix _contextMatrix;

        public VerticalRowScrollCommand(GuardMatrix contextMatrix)
        {
            _contextMatrix = contextMatrix;
        }

        public void Initialize(int horizRowId, int scrollDelta)
        {
            HorizRowId = horizRowId;
            ScrollDelta = scrollDelta;
        }
        public void Execute()
        {
            ScrollRow(false);
        }

        private void ScrollRow(bool isReversed)
        {
            if (_contextMatrix.Size.x <= HorizRowId)
                throw new InvalidOperationException();
            var row = _contextMatrix.GetVerticalCells(HorizRowId);
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