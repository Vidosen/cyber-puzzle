using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Combinations;
using Prototype.Scripts.Data;
using Prototype.Scripts.Providers.Mono;
using UnityEngine;

namespace Prototype.Scripts.Services
{
    public class CombinationsHandler : Singleton<CombinationsHandler>
    {
        [SerializeField] private CombinationProvider _combinationProvider;
        [SerializeField] private MatrixHandler _matrixHandler;
        private List<Combination> _combinations = new List<Combination>();

        public bool AllCombinationsCompleted => _combinations.All(c => c.IsCombinationComplete);
        
        public void InitCombinations(LevelSO level)
        {
            foreach (var codeCombination in level.CodeCombinations)
            {
                var combination = _combinationProvider.CreateNew();
                combination.Initialize(codeCombination);
                _combinations.Add(combination);
            }
        }

        public void DisposeCombinations()
        {
            _combinations?.ForEach(c => c.Dispose());
            _combinations?.Clear();
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