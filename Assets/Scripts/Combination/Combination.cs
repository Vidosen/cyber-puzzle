using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Prototype.Scripts.Data;
using Signals;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Combinations
{
    public class Combination : MonoBehaviour, IDisposable
    {
        [SerializeField] private CombinationCell _combinationCellPrefab;
        [SerializeField] private Transform _codesHolder;
        [SerializeField] private TextMeshProUGUI _hpForCombinationText;
        [SerializeField] private Image _combinationStatusImg;
        [ColorUsage(true,true)]
        [SerializeField] private Color _incompleteCombination = Color.red;
        [ColorUsage(true,true)]
        [SerializeField] private Color _completeCombination = Color.green;

        public List<CombinationCell> CombinationCodes => _cells;

        public float HPForCombination { get; private set; }
        
        public Color HighlightColor => _highlightColor;
        public bool IsCombinationComplete { get; private set; }
        private Color _highlightColor;
        private List<CombinationCell> _cells = new List<CombinationCell>();
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        public void Initialize(LevelSettings.CodeCombination codeCombination)
        {
            _highlightColor = codeCombination.HighlightColor;
            HPForCombination = codeCombination.HPForCombination;
            _hpForCombinationText.text = HPForCombination.ToString("0");
            for (int i = 0; i < codeCombination.Count; i++)
            {
                var newCell = Instantiate(_combinationCellPrefab, _codesHolder);
                newCell.Initialize(codeCombination[i]);
                _cells.Add(newCell);
            }

            _cells.Select(cell => cell.CellHoverEnter).Merge().Subscribe(cell =>
                    MessageBroker.Default.Publish(new CombinationSignals.HoverCellEnter
                        {Cell = cell, Combination = this }))
                .AddTo(_compositeDisposable);
            
            _cells.Select(cell => cell.CellHoverExit).Merge().Subscribe(cell =>
                    MessageBroker.Default.Publish(new CombinationSignals.HoverCellExit()
                        {Cell = cell, Combination = this }))
                .AddTo(_compositeDisposable);
        }

        public void ChangeIsCombinationComplete(bool isComplete)
        {
            IsCombinationComplete = isComplete;
            _combinationStatusImg.color = IsCombinationComplete ? _completeCombination : _incompleteCombination;
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
            if (_cells != null)
                _cells.ForEach(c => c.Dispose());
            _cells = null;
            Destroy(gameObject);
        }
    }
}