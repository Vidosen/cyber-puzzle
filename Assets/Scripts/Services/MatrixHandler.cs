using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Data;
using Prototype.Scripts.Matrix;
using Prototype.Scripts.Providers.Mono;
using UnityEngine;

public class MatrixHandler : Singleton<MatrixHandler>
{
    [SerializeField] private MatrixProvider _matrixProvider;
    private GameMatrix gameMatrix;
    public event Action MatrixChanged;

    private void Awake()
    {
#if DEBUG
        MatrixChanged += DebugMatrix;      
#endif
    }

    private void DebugMatrix()
    {
        Debug.Log("NEW MATRIX:\b");
        for (var i = 0; i < gameMatrix.RowsSize; i++)
        {
            var resultStr = "| ";
            for (var j = 0; j < gameMatrix.ColumnsSize; j++)
            {
                var pos = (j, i);
                resultStr += $"({pos.Item1},{pos.Item2})" + gameMatrix[j, i].Value;
                if (j == gameMatrix.ColumnsSize - 1)
                {
                    resultStr += " |\b\b";
                    continue;
                }

                resultStr += " | ";
            }

            Debug.Log(resultStr);
        }
    }

    public void InitMatrix(LevelSO level)
    {
        gameMatrix = _matrixProvider.CreateNew();
        gameMatrix.ThisTransform.localPosition = Vector2.zero;
        if (!gameMatrix.IsInitialized)
            gameMatrix.InitializeFromLevelSO(level);
        MatrixChanged?.Invoke();
    }

    public void DisposeMatrix()
    {
        gameMatrix?.Dispose();
        gameMatrix = null;
    }

    #region Highlight Methods

    public void DimAllMatrixCells(HighlightType type)
    {
        foreach (var matrixCell in gameMatrix.AllCells)
            matrixCell.DimCell(type);
    }

    public void HighlightAllMatchingMatrixCells(int compValue, Color highlightColor, HighlightType type)
    {
        foreach (var matrixCell in gameMatrix.AllCells.Where(cell => cell.Value == compValue))
            matrixCell.HighlightCell(highlightColor, type);
    }

    #endregion

    public void SwapRequest(BaseVector oneVector, BaseVector twoVector)
    {
        gameMatrix.SwapVectors(oneVector, twoVector);
        MatrixChanged?.Invoke();
    }

    public List<MatrixCell> FindBestMatrixCombination(List<ICell> combination)
    {
        var foundCombinations = new List<List<MatrixCell>>();
        var firstCell = combination.FirstOrDefault();
        if (firstCell == null)
            throw new Exception("'combination' parameter doesn't contain any CombinationCell!");
        var startCells = gameMatrix.AllCells.Where(mCell => mCell.Value == firstCell.Value);
        foreach (var cell in startCells)
        {
            var result = new List<MatrixCell>();
            FindMatrixCombination(cell, combination, 1, ref result);
            foundCombinations.Add(result);
        }

        return foundCombinations
            .OrderByDescending(c => c.Count)
            .FirstOrDefault();
    }

    private void FindMatrixCombination(MatrixCell cell, List<ICell> combination, int currentIndex,
        ref List<MatrixCell> resultCells)
    {
        if (resultCells == null) resultCells = new List<MatrixCell>();
        resultCells.Add(cell);
        if (currentIndex >= combination.Count)
            return;

        var pos = gameMatrix.IndexOf(cell);
        var i = pos.Item1;
        var j = pos.Item2;

        if (i + 1 < gameMatrix.ColumnsSize && gameMatrix[i + 1, j].Value == combination[currentIndex].Value)
        {
            FindMatrixCombination(gameMatrix[i + 1, j], combination, ++currentIndex, ref resultCells);
            return;
        }

        if (i - 1 >= 0 && gameMatrix[i - 1, j].Value == combination[currentIndex].Value)
        {
            FindMatrixCombination(gameMatrix[i - 1, j], combination, ++currentIndex, ref resultCells);
            return;
        }

        if (j + 1 < gameMatrix.RowsSize && gameMatrix[i, j + 1].Value == combination[currentIndex].Value)
        {
            FindMatrixCombination(gameMatrix[i, j + 1], combination, ++currentIndex, ref resultCells);
            return;
        }

        if (j - 1 >= 0 && gameMatrix[i, j - 1].Value == combination[currentIndex].Value)
        {
            FindMatrixCombination(gameMatrix[i, j - 1], combination, ++currentIndex, ref resultCells);
        }
    }
}