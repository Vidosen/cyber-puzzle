using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Prototype.Scripts.Data;
using UniRx;
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

        public List<CombinationCell> CombinationCodes => _cells;
        
        public IObservable<(CombinationCell, Combination)> AnyCellHoverEnter { get; private set; }
        public IObservable<(CombinationCell, Combination)> AnyCellHoverExit { get; private set; }
        public Color HighlightColor => _highlightColor;
        public bool IsCombinationComplete { get; private set; }
        private Color _highlightColor;
        private List<CombinationCell> _cells = new List<CombinationCell>();
        public void Initialize(LevelSettings.CodeCombination codeCombination)
        {
            _highlightColor = codeCombination.HighlightColor;
            for (int i = 0; i < codeCombination.Count; i++)
            {
                var newCell = Instantiate(_combinationCellPrefab, _codesHolder);
                newCell.Initialize(codeCombination[i]);
                _cells.Add(newCell);
            }
            AnyCellHoverEnter = Observable.Merge(_cells.Select(cell=>cell.CellHoverEnter)).Select(cell=>(cell, this));
            AnyCellHoverExit = Observable.Merge(_cells.Select(cell=>cell.CellHoverExit)).Select(cell=>(cell, this));
        }

        public void ChangeIsCombinationComplete(bool isComplete)
        {
            IsCombinationComplete = isComplete;
            _combinationStatusImg.color = IsCombinationComplete ? _completeCombination : _incompleteCombination;
        }

        public void Dispose()
        {
            if (_cells != null)
                _cells.ForEach(c => c.Dispose());
            _cells = null;
            Destroy(gameObject);
        }
    }
}