using System;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using UnityEngine;

namespace Utils
{
    public static class LogExtensions
    {
        public static void Log(this GuardMatrix guardMatrix)
        {
            Debug.Log("== Matrix :: Start ==");
            for (int y = 0; y < guardMatrix.Size.y; y++)
            {
                var row = guardMatrix.GetHorizontalCells(y);
                var rowLogLine = row
                    .Select(cell => GetCellLog(cell) + "\t").Aggregate((one, two) => one + two);
                Debug.Log(rowLogLine);
            }
            Debug.Log("== Matrix :: End ==");
        }

        private static string GetCellLog(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Value:
                case CellType.Glitch:
                    var valueCell = (ValueCell)cell;
                    return ((int)valueCell.Value).ToString();
                case CellType.Shuffle:
                    return "-S-";
                case CellType.Lock:
                    return "-L-";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}