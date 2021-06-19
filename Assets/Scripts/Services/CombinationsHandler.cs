using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Prototype.Scripts.Combinations;
using Prototype.Scripts.Data;
using Prototype.Scripts.Providers.Mono;
using UniRx;
using UnityEngine;

namespace Services
{
    public class CombinationsHandler : MonoBehaviour
    {
        [SerializeField] private CombinationProvider _combinationProvider;
        [SerializeField] private MatrixHandler _matrixHandler;
        private List<Combination> _combinations = new List<Combination>();

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public bool AllCombinationsCompleted => _combinations.All(c => c.IsCombinationComplete);
        
        public void InitCombinations(LevelSettings level)
        {
            foreach (var codeCombination in level.CodeCombinations)
            {
                var combination = _combinationProvider.CreateNew();
                combination.Initialize(codeCombination);
                _combinations.Add(combination);
            }

            _combinations.Select(combination => combination.AnyCellHoverEnter).Concat()
                .Subscribe(o => OnCellHoverEnter(o.Item1, o.Item2))
                .AddTo(_compositeDisposable);
            _combinations.Select(combination => combination.AnyCellHoverExit).Concat()
                .Subscribe(o => OnCellHoverExit(o.Item1, o.Item2))
                .AddTo(_compositeDisposable);
        }

        private void OnCellHoverEnter(CombinationCell cell, Combination combination)
        {
            cell.HighlightCell(combination.HighlightColor, HighlightType.HoverHint);
            _matrixHandler.HighlightAllMatchingMatrixCells(cell.Value, combination.HighlightColor, HighlightType.HoverHint);
        }
        
        private void OnCellHoverExit(CombinationCell cell, Combination combination)
        {
            cell.DimCell(HighlightType.HoverHint);
            _matrixHandler.DimAllMatrixCells(HighlightType.HoverHint);
        }

        public void DisposeCombinations()
        {
            _combinations?.ForEach(c => c.Dispose());
            _combinations?.Clear();
            _compositeDisposable.Clear();
        }
        #region Highlight Methods
        public void DimAllCombinationCells(HighlightType type)
        {
            foreach (var matrixCell in _combinations.SelectMany(s=>s.CombinationCodes))
                matrixCell.DimCell(type);
        }
        #endregion

        public void UpdateCombinations()
        {
            foreach (var combination in _combinations)
                CheckAndUpdateCombination(combination, 2);
        }
        private void CheckAndUpdateCombination(Combination combination, int minCount)
        {
            var foundCodes =
                _matrixHandler.FindBestMatrixCombination(combination.CombinationCodes.Select(code => code as ICell)
                    .ToList());
            if (foundCodes.Count >= minCount)
            {
                for (int i = 0; i < Mathf.Min(foundCodes.Count, combination.CombinationCodes.Count); i++)
                {
                    foundCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
                    combination.CombinationCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
                }
            }
            combination.ChangeIsCombinationComplete(foundCodes.Count == combination.CombinationCodes.Count);
        }
    }
}