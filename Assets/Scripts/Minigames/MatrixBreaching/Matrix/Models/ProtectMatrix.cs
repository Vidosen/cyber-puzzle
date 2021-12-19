using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using UniRx;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class ProtectMatrix : IDisposable
    {
        public Vector2 Size { get; private set; }
        public bool IsInitialized { get; private set; }

        public IObservable<ICell> OnCellAdded => _cellAddedSubject;
        public IObservable<ICell> OnCellRemoved => _cellRemovedSubject;

        private readonly ICellProvider _cellProvider;
        private readonly List<ICell> _cells = new List<ICell>();
        private Subject<ICell> _cellAddedSubject = new Subject<ICell>();
        private Subject<ICell> _cellRemovedSubject = new Subject<ICell>();


        public ProtectMatrix(ICellProvider cellProvider)
        {
            _cellProvider = cellProvider;
        }

        public void Initialize(int horizontalSize, int verticalSize)
        {
            if (IsInitialized)
                Dispose();
            Size = new Vector2(horizontalSize, verticalSize);
            _cells.AddRange(_cellProvider.GetNewCells(horizontalSize * verticalSize));
            for (int x = 0; x < horizontalSize; x++)
            for (int y = 0; y < verticalSize; y++)
            {
                var cell = _cells[verticalSize * x + y];
                cell.Move(x, y);
                _cellAddedSubject.OnNext(cell);
            }

            IsInitialized = true;
        }

        public IEnumerable<ICell> GetCells()
        {
            return _cells.AsEnumerable();
        }

        public IEnumerable<ICell> GetHorizontalCells(int horizontalId)
        {
            return _cells.Where(cell => cell.HorizontalId.Equals(horizontalId)).AsEnumerable();
        }
        public IEnumerable<ICell> GetVerticalCells(int verticalId)
        {
            return _cells.Where(cell => cell.VerticalId.Equals(verticalId)).AsEnumerable();
        }
        public ICell GetCell(int horizontalId, int verticalId)
        {
            var foundCells = _cells.Where(cell =>
                cell.HorizontalId.Equals(horizontalId) && cell.VerticalId.Equals(verticalId)).ToList();
            if (foundCells.Count != 1)
                throw new InvalidOperationException();
            return foundCells.First();
        }

        public ICell ReplaceCell(int horizontalId, int verticalId)
        {
            return ReplaceCell(GetCell(horizontalId, verticalId));
        }
        
        public ICell ReplaceCell(ICell cell)
        {
            if (!_cells.Contains(cell))
                throw new InvalidOperationException();

            var newCell = _cellProvider.GetNewCell();
            newCell.Move(cell.HorizontalId, cell.VerticalId);
            
            _cells.Remove(cell);
            _cellRemovedSubject.OnNext(cell);
            cell.Dispose();
            
            _cells.Add(newCell);
            _cellAddedSubject.OnNext(newCell);
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
