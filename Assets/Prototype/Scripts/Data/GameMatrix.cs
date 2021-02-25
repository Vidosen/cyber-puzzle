using System;
using System.Linq;
using Prototype.Scripts.Utils;
using Prototype.Scripts.Views;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class GameMatrix : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ColumnSlot columnSlotPrefab;
        [SerializeField] private RowSlot rowSlotPrefab;
        [SerializeField] private ColumnVector columnVectorPrefab;
        [SerializeField] private RowVector rowVectorPrefab;
        [SerializeField] private Cell cellPrefab;
        
        [Space, SerializeField] private float Offset = 20f;
        
        private RectTransform _thisTransform;
        private RowVector[] rowVectors;
        private ColumnVector[] columnVectors;
        
        private RowSlot[] rowSlots;
        private ColumnSlot[] columnSlots;
        
        #endregion

        public int RowsSize => rowVectors.Length;
        public int ColumnsSize => columnVectors.Length;
        public RowVector[] Rows => rowVectors;
        public ColumnVector[] Columns => columnVectors;
        public RowSlot[] RowSlots => rowSlots;
        public ColumnSlot[] ColumnSlots => columnSlots;
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;

        public bool IsInitialized => rowVectors != null && columnVectors != null;
        

        public void InitializeFromLevelSO(LevelSO level)
        {
            RecalculateMatrixRect(level.ColumnsCount, level.RowsCount);
            GenerateVectors(level.ColumnsCount, level.RowsCount);
            SetupVectors();
            
            GenerateCells();
            SetupCells(level);
            
        }
        
        private void GenerateVectors(int columns, int rows)
        {
            GenerateVector(columns, rows, columnSlotPrefab, columnVectorPrefab, out columnSlots, out columnVectors);
            GenerateVector(rows, columns, rowSlotPrefab, rowVectorPrefab, out rowSlots, out rowVectors);
        }

        private void GenerateVector<TSlot, TVector>(int size, int cellsCount, TSlot slotPrefab, TVector vectorPrefab,
            out TSlot[] slots, out TVector[] vectors)
            where TVector : BaseVector
            where TSlot : BaseSlot<TVector>
        {
            slots = new TSlot[size];
            vectors = new TVector[size];
            for (int i = 0; i < size; i++)
            {
                slots[i] = Instantiate(slotPrefab, transform);
                vectors[i] = Instantiate(vectorPrefab, slots[i].ThisTransform);
                
                slots[i].Initialize(vectors[i]);
                vectors[i].Initialize(cellsCount, i);
            }
        }
        

        private void GenerateCells()
        {
            for (int row = 0; row < RowsSize; row++)
            {
                for (int column = 0; column < ColumnsSize; column++)
                {
                    var newCell = Instantiate(cellPrefab, transform);
                    newCell.Initialize(columnVectors[column], rowVectors[row]);
                    rowVectors[row][column] = newCell;
                    columnVectors[column][row] = newCell;
                }
            }
        }

        public void RecalculateMatrixRect()
        {
            ThisTransform.sizeDelta =
                RectTransformHelper.GetGridContainer(cellPrefab.ThisTransform.rect, RowsSize + 1, ColumnsSize + 1, Offset);
        }
        public void RecalculateMatrixRect(int columns, int rows)
        {
            ThisTransform.sizeDelta =
                RectTransformHelper.GetGridContainer(cellPrefab.ThisTransform.rect, rows + 1, columns + 1, Offset);
        }
        
        private void SetupVectors()
        {
            for (var i = 0; i < RowSlots.Length; i++)
            {
                var _transform = RowSlots[i].ThisTransform;
                _transform.localPosition = RectTransformHelper.GetChildPositionContainer(_transform.rect, 0, i + 1, Offset);
            }
            for (var i = 0; i < ColumnSlots.Length; i++)
            {
                var _transform = ColumnSlots[i].ThisTransform;
                _transform.localPosition = RectTransformHelper.GetChildPositionContainer(_transform.rect, i + 1, 0, Offset);
            }
        }

        private void SetupCells(LevelSO level)
        {
            for (var x = 0; x < level.ColumnsCount; x++)
            for (int y = 0; y < level.RowsCount; y++)
            {
                this[x, y].Value = level.MatrixField[x][y];
            }

        }

        private void OnDestroy()
        {
            if (rowVectors != null)
                foreach (var rowVector in rowVectors?.Where(v => v != null))
                    Destroy(rowVector.gameObject);
            if (columnVectors != null)
                foreach (var columnVector in columnVectors?.Where(v => v != null))
                    Destroy(columnVector.gameObject);
            if (rowSlots != null)
                foreach (var vectorSlot in rowSlots?.Where(v => v != null))
                    Destroy(vectorSlot.gameObject);
            if (columnSlots != null)
                foreach (var vectorSlot in columnSlots?.Where(v => v != null))
                    Destroy(vectorSlot.gameObject);
        }


        public Cell this[int columnIndex, int rowIndex] => GetCellByRowColumnIndices(columnIndex, rowIndex);

        public Cell GetCellByRowColumnIndices(int columnIndex, int rowIndex)
        {
            if (rowIndex >= RowsSize)
            {
                Debug.LogError("[Data] GameMatrix.GetCellByRowColumnIndices: rowIndex is out of range");
                return default;
            }
            return rowVectors[rowIndex][columnIndex];
        }

        public void SwapRows(int firstIndex, int secondIndex)
        {
            SwapVectors(rowVectors, firstIndex, secondIndex);
        }
        public void SwapColumns(int firstIndex, int secondIndex)
        {
            SwapVectors(columnVectors, firstIndex, secondIndex);
        }

        private void SwapVectors<TVector>(TVector[] vectorsArray, int firstIndex, int secondIndex) where TVector : BaseVector
        {
            if (Math.Max(firstIndex, secondIndex) >= vectorsArray.Length)
            {
                Debug.LogError("[Data] GameMatrix.SwapVectors: some index is out of range");
                return;
            }
            var temp = vectorsArray[firstIndex];
            vectorsArray[firstIndex] = vectorsArray[secondIndex];
            vectorsArray[secondIndex] = temp;
        }
    }
}