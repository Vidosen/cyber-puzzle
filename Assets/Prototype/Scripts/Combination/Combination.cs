using System;
using System.Collections.Generic;
using Prototype.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Prototype.Scripts.Views
{
    public class Combination : MonoBehaviour, IDisposable
    {
        [SerializeField] private CombinationCell _combinationCellPrefab;

        public Color HighlightColor => _highlightColor;
        private Color _highlightColor;
        private List<CombinationCell> cells = new List<CombinationCell>();
        public void Initialize(LevelSO.CodeCombination codeCombination)
        {
            _highlightColor = codeCombination.HighlightColor;
            for (int i = 0; i < codeCombination.Count; i++)
            {
                var newCell = Instantiate(_combinationCellPrefab, transform);
                newCell.Initialize(codeCombination[i], this);
                cells.Add(newCell);
            }
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}