using System.Collections.Generic;
using Data;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomEditor(typeof(LevelSettings))]
    public class LevelSettingsEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var levelData = target as LevelSettings;
            
            if (levelData == null)
                return;
            
            EditorGUILayout.Space(5f);
            GUILayout.Space(20f);
            GUILayout.Label("Level Preset Settings", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            levelData.LevelTimer = Mathf.Max(EditorGUILayout.FloatField("Timer (in sec.):", levelData.LevelTimer), 0f);
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
            GUILayout.Label("Defined Combinations:", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            if (levelData.CombinationsIsNull())
                levelData.CodeCombinations = new List<LevelSettings.CodeCombination>();
            var combinationsCount = Mathf.Clamp(EditorGUILayout.IntField("Count: ",levelData.CodeCombinations.Count), 0,5);
            GUILayout.Space(5f);
            if (combinationsCount < levelData.CodeCombinations.Count)
                levelData.CodeCombinations.RemoveRange(combinationsCount,
                    levelData.CodeCombinations.Count - combinationsCount);
            else if(combinationsCount > levelData.CodeCombinations.Count)
                for (var i = 0; i < combinationsCount - levelData.CodeCombinations.Count; i++)
                    levelData.CodeCombinations.Add(
                        new LevelSettings.CodeCombination());

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

        private static void ResizeMatrixContainer(LevelSettings levelData)
        {
            if (levelData.MatrixIsNull())
            {
                levelData.MatrixField = new List<LevelSettings.Row>();
                for (int column = 0; column < levelData.ColumnsCount; column++)
                {
                    levelData.MatrixField.Add(new LevelSettings.Row());
                    for (int i = 0; i < levelData.RowsCount; i++)
                        levelData.MatrixField[column].Add(-1);
                }
            }
            else
            {
                int column;
                for (column = 0; column < levelData.ColumnsCount; column++)
                {
                    if (column >= levelData.MatrixField.Count)
                    {
                        levelData.MatrixField.Add(new LevelSettings.Row());
                        for (int i = 0; i < levelData.RowsCount; i++)
                            levelData.MatrixField[column].Add(-1);
                    }

                    int row;
                    for (row = 0; row < levelData.RowsCount; row++)
                    {
                        if (row >= levelData.MatrixField[column].Count)
                            levelData.MatrixField[column].Add(-1);
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
