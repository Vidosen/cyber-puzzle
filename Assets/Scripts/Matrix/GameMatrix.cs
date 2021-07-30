﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Matrix
{
    public class GameMatrix : MonoBehaviour, IDisposable
    {
        #region Private Fields
        [SerializeField] private Transform Holder;
        [SerializeField, Space] private ColumnSlot columnSlotPrefab;
        [SerializeField] private RowSlot rowSlotPrefab;
        [SerializeField] private ColumnVector columnVectorPrefab;
        [SerializeField] private RowVector rowVectorPrefab;
        [SerializeField] private MatrixCell matrixCellPrefab;
        
        [Space, SerializeField] private float Offset = 10f;
        [Space, SerializeField] private float SlotOffset = 20f;
        
        private RectTransform _thisTransform;
        private RowVector[] rowVectors;
        private ColumnVector[] columnVectors;
        
        private RowSlot[] rowSlots;
        private ColumnSlot[] columnSlots;
        
        private Dictionary<int, int> _matrixDictionary = new Dictionary<int, int>();
        #endregion

        public Dictionary<int, int> MatrixValuesMap => _matrixDictionary;
        public int RowsSize => rowVectors.Length;
        public int ColumnsSize => columnVectors.Length;
        public RowVector[] Rows => rowVectors;
        public ColumnVector[] Columns => columnVectors;
        public RowSlot[] RowSlots => rowSlots;
        public ColumnSlot[] ColumnSlots => columnSlots;

        public MatrixCell[] AllCells =>
            Rows.Select(r => r.Cells)
                .Aggregate((one, second) => one.Concat(second).ToArray());
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;

        public bool IsInitialized => rowVectors != null && columnVectors != null;
        

        public void InitializeFromLevelSO(LevelSettings level, RectTransform holder)
        {
            var gridRatio = RecalculateMatrixRect(level.ColumnsCount, level.RowsCount, holder);
            GenerateVectors(level.ColumnsCount, level.RowsCount, gridRatio);
            SetupSlots();
            
            GenerateCells(gridRatio);
            SetupCells(level);
            
        }
        
        #region Internal Initialize
        private float RecalculateMatrixRect(int columns, int rows, RectTransform holder)
        {
            var unscaledGridSize =
                RectTransformHelper.GetGridContainer(matrixCellPrefab.ThisTransform.rect, rows + 1, columns + 1,
                    Offset) + new Vector2(SlotOffset, SlotOffset);
            var gridRatio = RectTransformHelper.GetGridContainerRatio(holder, unscaledGridSize);
            ThisTransform.sizeDelta = unscaledGridSize * gridRatio;
            return gridRatio;
        }
        private void GenerateVectors(int columns, int rows, float gridRatio)
        {
            GenerateVector(columns, rows, gridRatio, columnSlotPrefab, columnVectorPrefab, out columnSlots, out columnVectors);
            GenerateVector(rows, columns, gridRatio, rowSlotPrefab, rowVectorPrefab, out rowSlots, out rowVectors);
        }

        private void GenerateVector<TSlot, TVector>(int size, int cellsCount, float gridRatio, TSlot slotPrefab, TVector vectorPrefab,
            out TSlot[] slots, out TVector[] vectors)
            where TVector : BaseVector
            where TSlot : BaseSlot<TVector>
        {
            slots = new TSlot[size];
            vectors = new TVector[size];
            for (int i = 0; i < size; i++)
            {
                slots[i] = Instantiate(slotPrefab, Holder);
                vectors[i] = Instantiate(vectorPrefab, Holder);
                
                slots[i].Initialize(vectors[i]);
                slots[i].name += $" {i + 1}";
                slots[i].MultiplyScale(gridRatio);
                
                vectors[i].Initialize(cellsCount, i, this);
                vectors[i].name += $" {i + 1}";
                vectors[i].MultiplyScale(gridRatio);
            }
        }
        

        private void GenerateCells(float gridRatio)
        {
            for (int row = 0; row < RowsSize; row++)
            {
                for (int column = 0; column < ColumnsSize; column++)
                {
                    var newCell = Instantiate(matrixCellPrefab, Holder);
                    newCell.Initialize(this, columnVectors[column], rowVectors[row]);
                    rowVectors[row][column] = newCell;
                    columnVectors[column][row] = newCell;
                    
                    newCell.name += $" C{column + 1}-R{row + 1}";
                    newCell.MultiplyScale(gridRatio);
                }
            }
        }

        private void SetupSlots()
        {
            for (var i = 0; i < RowSlots.Length; i++)
            {
                var _transform = RowSlots[i].ThisTransform;
                _transform.localPosition =
                    RectTransformHelper.GetChildPositionContainer(_transform.rect, 0, i + 1, Offset) +
                    new Vector3(0, -SlotOffset, -5f);
                RowSlots[i].SnapVector();
            }
            for (var i = 0; i < ColumnSlots.Length; i++)
            {
                var _transform = ColumnSlots[i].ThisTransform;
                _transform.localPosition =
                    RectTransformHelper.GetChildPositionContainer(_transform.rect, i + 1, 0, Offset) +
                    new Vector3(SlotOffset, 0, -5f);
                ColumnSlots[i].SnapVector();
            }
            
        }

        private void SetupCells(LevelSettings level)
        {
            for (var x = 0; x < level.ColumnsCount; x++)
            for (int y = 0; y < level.RowsCount; y++)
            {
                var cell = this[x, y];
                if (level.MatrixField[x][y] == -1)
                    cell.Value = ChooseRandomValue();
                else
                    cell.Value = level.MatrixField[x][y];
                if (!_matrixDictionary.ContainsKey(cell.Value))
                    _matrixDictionary.Add(cell.Value, 0);

                cell.SnapCell();
                _matrixDictionary[cell.Value]++;
            }

        }
        #endregion
        
        public void ChangeCell(MatrixCell cell, int newValue = -1)
        {
            _matrixDictionary[cell.Value]--;
            if (newValue == -1)
                cell.Value = ChooseRandomValue();
            else
                cell.Value = newValue;

            if (!_matrixDictionary.ContainsKey(cell.Value))
                _matrixDictionary.Add(cell.Value, 0);
            
            _matrixDictionary[cell.Value]++;
        }

        private int ChooseRandomValue()
        {
            return Random.Range(0, 10);
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


        public MatrixCell this[int columnIndex, int rowIndex] => GetCellByRowColumnIndices(columnIndex, rowIndex);

        public (int x, int y) IndexOf(MatrixCell cell)
        {
            for (int i = 0; i < Rows.Length; i++)
            for (int j = 0; j < Columns.Length; j++)
                if (Rows[i][j] == cell)
                    return (j, i);

            return (-1, -1);
        }
        public MatrixCell GetCellByRowColumnIndices(int columnIndex, int rowIndex)
        {
            if (rowIndex >= RowsSize)
            {
                Debug.LogError("[Data] GameMatrix.GetCellByRowColumnIndices: rowIndex is out of range");
                return default;
            }
            return rowVectors[rowIndex][columnIndex];
        }

        public void SwapVectors(BaseVector vectorOne, BaseVector vectorTwo)
        {
            var vecOneType = vectorOne.GetType();
            var vecTwoType = vectorTwo.GetType();
            if (vecOneType != vecTwoType)
            {
                Debug.LogError("TVector types don't match!");
                return;
            }
            // var tmp = vectorOne;
            // vectorOne = vectorTwo;
            // vectorTwo = tmp;
            if (vectorOne is RowVector rowOne && vectorTwo is RowVector rowTwo)
            {
                var rowVectorList = rowVectors.ToList();
                var rOneIndex = rowVectorList.IndexOf(rowOne);
                var rTwoIndex = rowVectorList.IndexOf(rowTwo);
                rowVectors[rOneIndex] = rowSlots[rOneIndex].Vector = rowTwo;
                rowVectors[rTwoIndex] = rowSlots[rTwoIndex].Vector = rowOne;
                
                foreach (var columnVector in columnVectors)
                {
                    var tmp = columnVector.Cells[rOneIndex];
                    columnVector.Cells[rOneIndex] = columnVector.Cells[rTwoIndex];
                    columnVector.Cells[rTwoIndex] = tmp;
                }
            }
            else if (vectorOne is ColumnVector columnOne && vectorTwo is ColumnVector columnTwo)
            {
                var columnVectorList = columnVectors.ToList();
                var cOneIndex = columnVectorList.IndexOf(columnOne);
                var cTwoIndex = columnVectorList.IndexOf(columnTwo);
                columnVectors[cOneIndex] = columnSlots[cOneIndex].Vector = columnTwo;
                columnVectors[cTwoIndex] = columnSlots[cTwoIndex].Vector = columnOne;
                foreach (var rowVector in rowVectors)
                {
                    var tmp = rowVector.Cells[cOneIndex];
                    rowVector.Cells[cOneIndex] = rowVector.Cells[cTwoIndex];
                    rowVector.Cells[cTwoIndex] = tmp;
                }
            }
        }

        public RowSlot FindRowSlotByVector(RowVector vector)
        {
            return rowSlots.FirstOrDefault(slot => slot.Vector == vector);
        }
        public ColumnSlot FindColumnSlotByVector(ColumnVector vector)
        {
            return columnSlots.FirstOrDefault(slot => slot.Vector == vector);
        }

        public void Dispose()
        {
            _matrixDictionary.Clear();
            Destroy(gameObject);
        }
    }
}