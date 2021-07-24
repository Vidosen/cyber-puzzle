using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Data
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Settings/Create Level")]
    public class LevelSettings : ScriptableObject
    {
        public float LevelTimer = 30f;
        
        [Range(3, 6)] public int RowsCount = 3;
        [Range(3, 6)] public int ColumnsCount = 3;

        public List<Row> MatrixField;

        public List<CodeCombination> CodeCombinations;
        
        [Serializable]
        public class CodeCombination
        {
            public Color HighlightColor;
            public int HPForCombination;
            public List<int> Combination;
            public CodeCombination(){
                
                Combination = new List<int>();
                Random random = new Random();
                HighlightColor = Color.HSVToRGB((float)random.NextDouble(), 1, 1);
            }
            public int Count => Combination?.Count ?? 0;
            public int this[int key]
            {
                get => Combination[key];
                set => Combination[key] = value;
            }
            public void AddRange(params int[] range) => Combination.AddRange(range);
            public void RemoveRange(int index, int count) => Combination.RemoveRange(index, count);

        }
        
        [Serializable]
        public class Row
        {
            public List<int> Elements;

            public int this[int key]
            {
                get => Elements[key];
                set => Elements[key] = value;
            }

            public Row()
            {
                Elements = new List<int>();
            }

            public int Count => Elements?.Count ?? 0;
            public void AddRange(params int[] range) => Elements.AddRange(range);
            public void Add(int value) => Elements.Add(value);
            public void RemoveRange(int index, int count) => Elements.RemoveRange(index, count);
        }

        public bool MatrixIsNull()
        {
            return MatrixField == null;
        }
        public bool CombinationsIsNull()
        {
            return CodeCombinations == null;
        }

        public int MatrixColumnsCount()
        {
            return MatrixField?.Count ?? 0;
        }

        public int MatrixRowsCount()
        {
            return MatrixField?.FirstOrDefault()?.Count ?? 0;
        }
    }
}
