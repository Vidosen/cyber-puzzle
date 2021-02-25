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

        public int VerticalSize => rowVectors.Length;
        public int HorizonalSize => columnVectors.Length;
        public RowVector[] Rows => rowVectors;
        public ColumnVector[] Columns => columnVectors;
        public RowSlot[] RowSlots => rowSlots;
        public ColumnSlot[] ColumnSlots => columnSlots;
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;

        public bool MatrixInitialized => rowVectors != null && columnVectors != null;
        
        public void InitializeFromLevelSO(LevelSO level)
        {
            RecalculateMatrixRect(level.ColumnsCount, level.RowsCount);
            GenerateVectors(level.ColumnsCount, level.RowsCount);
            SetupVectors();
            
            GenerateCells();
            for (var x = 0; x < level.ColumnsCount; x++)
            for (int y = 0; y < level.RowsCount; y++)
                this[x, y].Value = level.MatrixField[x][y];
        }
        
        private void GenerateVectors(int columns, int rows)
        {
            rowVectors = new RowVector[rows];
            columnVectors = new ColumnVector[columns];
            
            rowSlots = new RowSlot[rows];
            columnSlots = new ColumnSlot[columns];

            for (int i = 0; i < rows; i++)
            {
                rowSlots[i] = Instantiate(rowSlotPrefab, transform, true);
                rowVectors[i] = Instantiate(rowVectorPrefab, rowSlots[i].ThisTransform, true);
                rowVectors[i].Initialize(columns, i);
                rowSlots[i].Vector = rowVectors[i];
            }

            for (int i = 0; i < columns; i++)
            {
                columnSlots[i] = Instantiate(columnSlotPrefab, transform, true);
                columnVectors[i] = Instantiate(columnVectorPrefab, columnSlots[i].ThisTransform, true);
                columnVectors[i].Initialize(columns, i);
                columnSlots[i].Vector = columnVectors[i];
            }
            
        }

        private void GenerateCells()
        {
            for (int row = 0; row < VerticalSize; row++)
            {
                for (int column = 0; column < HorizonalSize; column++)
                {
                    var newCell = Instantiate(cellPrefab, transform, true);
                    newCell.Initialize(columnVectors[column], rowVectors[row]);
                    rowVectors[row][column] = newCell;
                    columnVectors[column][row] = newCell;
                }
            }
        }

        public void RecalculateMatrixRect()
        {
            ThisTransform.sizeDelta =
                RectTransformHelper.GetGridContainer(cellPrefab.ThisTransform.rect, VerticalSize + 1, HorizonalSize + 1, Offset);
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
                _transform.localPosition = RectTransformHelper.GetChildPsotionContainer(_transform.rect, 0, i + 1, Offset);
            }
            for (var i = 0; i < ColumnSlots.Length; i++)
            {
                var _transform = ColumnSlots[i].ThisTransform;
                _transform.localPosition = RectTransformHelper.GetChildPsotionContainer(_transform.rect, i + 1, 0, Offset);
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
            if (rowIndex >= VerticalSize)
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