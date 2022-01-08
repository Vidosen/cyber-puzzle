using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using UniRx;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class GuardMatrix : IDisposable
    {
        public Vector2Int Size { get; private set; }
        public bool IsInitialized { get; private set; }

        public IObservable<ReplaceCellsEventArgs> OnCellReplaced => _cellReplacedSubject;
        private readonly ICellProvider _cellProvider;
        private readonly List<ICell> _cells = new List<ICell>();
        private Subject<ReplaceCellsEventArgs> _cellReplacedSubject = new Subject<ReplaceCellsEventArgs>();


        public GuardMatrix(ICellProvider cellProvider)
        {
            _cellProvider = cellProvider;
        }

        public void Initialize(int horizontalSize, int verticalSize)
        {
            if (IsInitialized)
                Dispose();
            Size = new Vector2Int(horizontalSize, verticalSize);
            _cells.AddRange(_cellProvider.GetNewCells(horizontalSize, verticalSize));
            for (int y = 0; y < verticalSize; y++)
            for (int x = 0; x < horizontalSize; x++)
            {
                var cell = _cells[horizontalSize * y + x];
                cell.Move(x, y);
            }

            IsInitialized = true;
        }

        public IList<ICell> GetCells()
        {
            return _cells.ToList();
        }

        public IList<ICell> GetHorizontalCells(int verticalId)
        {
            return _cells.Where(cell => cell.VerticalId.Equals(verticalId)).OrderBy(cell=>cell.HorizontalId).ToList();
        }
        public IList<ICell> GetVerticalCells(int horizontalId)
        {
            return _cells.Where(cell => cell.HorizontalId.Equals(horizontalId)).OrderBy(cell=>cell.VerticalId).ToList();
        }
        public ICell GetCell(int horizontalId, int verticalId)
        {
            var foundCells = _cells.Where(cell =>
                cell.HorizontalId.Equals(horizontalId) && cell.VerticalId.Equals(verticalId)).ToList();
            if (foundCells.Count > 1)
                throw new InvalidOperationException();
            return foundCells.FirstOrDefault();
        }

        public ICell ReplaceCell(int horizontalId, int verticalId)
        {
            return ReplaceCell(GetCell(horizontalId, verticalId));
        }
        
        public ICell ReplaceCell(ICell cell)
        {
            if (!_cells.Contains(cell))
            {
                Debug.LogWarning(
                    $"Cell ({cell.HorizontalId}, {cell.VerticalId}) is already replaced");
                return null;
            }

            var newCell = _cellProvider.GetNewCell();
            newCell.Move(cell.HorizontalId, cell.VerticalId);
            
            _cellReplacedSubject.OnNext(new ReplaceCellsEventArgs(cell, newCell));
            _cells.Remove(cell);
            cell.Dispose();
            _cells.Add(newCell);
            return newCell;
        }

        public void Dispose()
        {
            IsInitialized = false;
            _cells.ForEach(cell=>cell.Dispose());
            _cells.Clear();
        }
    }
}
