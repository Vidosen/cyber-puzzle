using System.Linq;
using Minigames.MatrixBreaching.Matrix.Models;
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
                var rowLogLine = row.Select(cell => cell as ValueCell).Where(cell => cell != null)
                    .Select(cell => (int)cell.Value + "\t").Aggregate((one, two) => one + two);
                Debug.Log(rowLogLine);
            }
            Debug.Log("== Matrix :: End ==");
        }
    }
}