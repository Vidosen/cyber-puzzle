using System;
using System.Collections.Generic;
using Prototype.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Combinations
{
    public class Combination : MonoBehaviour, IDisposable
    {
        [SerializeField] private CombinationCell _combinationCellPrefab;
        [SerializeField] private Transform _codesHolder;
        [SerializeField] private Image _combinationStatusImg;
        [SerializeField] private Color _incompleteCombination = Color.red;
        [SerializeField] private Color _completeCombination = Color.green;

        public List<CombinationCell> CombinationCodes => cells;
        public Color HighlightColor => _highlightColor;
        public bool IsCombinationComplete { get; private set; }
        private Color _highlightColor;
        private List<CombinationCell> cells = new List<CombinationCell>();
        public void Initialize(LevelSO.CodeCombination codeCombination)
        {
            _highlightColor = codeCombination.HighlightColor;
            for (int i = 0; i < codeCombination.Count; i++)
            {
                var newCell = Instantiate(_combinationCellPrefab, _codesHolder);
                newCell.Initialize(codeCombination[i], this);
                cells.Add(newCell);
            }
        }

        public void ChangeIsCombinationComplete(bool isComplete)
        {
            IsCombinationComplete = isComplete;
            _combinationStatusImg.color = IsCombinationComplete ? _completeCombination : _incompleteCombination;
        }

        public void Dispose()
        {
            if (cells != null)
                cells.ForEach(c => c.Dispose());
            cells = null;
            Destroy(gameObject);
        }
    }
}