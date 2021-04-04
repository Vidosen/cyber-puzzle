using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace Prototype.Scripts.Utils.Editor
{
    [CustomEditor(typeof(LevelSO))]
    public class LevelSOEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var levelData = target as LevelSO;
            
            if (levelData == null)
                return;
            
            EditorGUILayout.Space(5f);
            GUILayout.Space(20f);
            GUILayout.Label("Level Preset Settings", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            levelData.LevelTimer = Mathf.Max(EditorGUILayout.FloatField("Time to finish level (ins sec.):", levelData.LevelTimer), 0f);
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            levelData.ColumnsCount =
                Mathf.Clamp(EditorGUILayout.IntField("Columns:", levelData.ColumnsCount), 3, 6);
            EditorGUILayout.Space(5f);
            levelData.RowsCount = 
                Mathf.Clamp(EditorGUILayout.IntField("Rows:", levelData.RowsCount), 3, 6);
            EditorGUILayout.EndVertical();
            GUILayout.Space(20f);

            ResizeMatrixContainer(levelData);

            int columnIndex;
            GUILayout.Label("Matrix:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            for (columnIndex = 0; columnIndex < levelData.ColumnsCount; columnIndex++)
            {
                EditorGUILayout.BeginVertical();
                int rowIndex;
                for (rowIndex = 0; rowIndex < levelData.RowsCount; rowIndex++)
                {
                    levelData.MatrixField[columnIndex][rowIndex] = EditorGUILayout.IntField(levelData.MatrixField[columnIndex][rowIndex]);
                    EditorGUILayout.Space(5f);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20f);
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Combinations:", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            if (levelData.CombinationsIsNull())
                levelData.CodeCombinations = new List<LevelSO.CodeCombination>();
            var combinationsCount = Mathf.Clamp(EditorGUILayout.IntField("Count: ",levelData.CodeCombinations.Count), 1,5);
            GUILayout.Space(5f);
            if (combinationsCount < levelData.CodeCombinations.Count)
                levelData.CodeCombinations.RemoveRange(combinationsCount,
                    levelData.CodeCombinations.Count - combinationsCount);
            else if(combinationsCount > levelData.CodeCombinations.Count)
                for (var i = 0; i < combinationsCount - levelData.CodeCombinations.Count; i++)
                    levelData.CodeCombinations.Add(
                        new LevelSO.CodeCombination());

            for (var i = 0; i < levelData.CodeCombinations.Count; i++)
            {
                var codeCombination = levelData.CodeCombinations[i];
                var codeSize = Mathf.Clamp(EditorGUILayout.IntField("Code size: ", codeCombination.Count), 2,8);
                
                if (codeSize < codeCombination.Count)
                    codeCombination.RemoveRange(codeSize, codeCombination.Count - codeSize);
                else if(codeSize > codeCombination.Count)
                    codeCombination.AddRange(new int[codeSize - codeCombination.Count]);
                
                EditorGUILayout.BeginHorizontal();
                int rowIndex;
                for (rowIndex = 0; rowIndex < codeCombination.Count; rowIndex++)
                {
                    codeCombination[rowIndex] = EditorGUILayout.IntField(codeCombination[rowIndex]);
                    EditorGUILayout.Space(5f);
                }
                EditorGUILayout.EndHorizontal();
                codeCombination.HighlightColor =
                    EditorGUILayout.ColorField(codeCombination.HighlightColor);
                EditorGUILayout.Space(10f);
            }

            EditorGUILayout.EndVertical();
            
            if (GUI.changed) {
                Undo.RecordObject(levelData, "Edit levels");
                EditorUtility.SetDirty(levelData);
            }

        }

        private static void ResizeMatrixContainer(LevelSO levelData)
        {
            if (levelData.MatrixIsNull())
            {
                levelData.MatrixField = new List<LevelSO.Row>();
                for (int column = 0; column < levelData.ColumnsCount; column++)
                {
                    levelData.MatrixField.Add(new LevelSO.Row());
                    levelData.MatrixField[column].AddRange(new int[levelData.RowsCount]);
                }
            }
            else
            {
                int column;
                for (column = 0; column < levelData.ColumnsCount; column++)
                {
                    if (column >= levelData.MatrixField.Count)
                    {
                        levelData.MatrixField.Add(new LevelSO.Row());
                        levelData.MatrixField[column].AddRange(new int[levelData.RowsCount]);
                    }

                    int row;
                    for (row = 0; row < levelData.RowsCount; row++)
                    {
                        if (row >= levelData.MatrixField[column].Count)
                            levelData.MatrixField[column].Add(0);
                    }

                    var rowsCount = levelData.MatrixField[column].Count;
                    var throwRows = rowsCount - row;
                    if (throwRows > 0)
                        levelData.MatrixField[column].RemoveRange(row, throwRows);
                }

                var columnsCount = levelData.MatrixField.Count;
                var throwColumns = columnsCount - column;
                if (throwColumns > 0)
                    levelData.MatrixField.RemoveRange(column, throwColumns);
            }
        }
    }
    
}
