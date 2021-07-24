using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Prototype.Scripts.Combinations;
using Prototype.Scripts.Data;
using Prototype.Scripts.Providers.Mono;
using Signals;
using UniRx;
using UnityEngine;

namespace Services
{
    public class CombinationsHandler : MonoBehaviour
    {
        [SerializeField] private CombinationProvider _combinationProvider;
        [SerializeField] private MatrixHandler _matrixHandler;
        [SerializeField] private ProgressHandler _progressHandler;
        private List<Combination> _combinations = new List<Combination>();

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private GeneralGameplaySettings _defaultlevelSettings;
        private Dictionary<int, int> _combinationDictionary = new Dictionary<int, int>();
        public Dictionary<int, int> CombinationValuesMap => _combinationDictionary;
        private void Awake()
        {
            _defaultlevelSettings = Resources.Load<GeneralGameplaySettings>("Settings/GeneralLevelSettings");
        }

        public void InitCombinations(LevelSettings level)
        {
            for (int i = 0; i < _defaultlevelSettings.CombinationsCount; i++)
            {
                var combination = _combinationProvider.CreateNew();
                var hasPreparedCombination = i < level.CodeCombinations.Count;
                var combinationArray =
                    hasPreparedCombination ? level.CodeCombinations[i] : GenerateDynamicCombination(i);
                AddToMap(combinationArray);
                combination.Initialize(combinationArray);
                _combinations.Add(combination);
            }
            MessageBroker.Default.Receive<CombinationSignals.HoverCellEnter>()
                .Subscribe(o => OnCellHoverEnter(o.Cell, o.Combination))
                .AddTo(_compositeDisposable);
            
            MessageBroker.Default.Receive<CombinationSignals.HoverCellExit>()
                .Subscribe(o => OnCellHoverExit(o.Cell, o.Combination))
                .AddTo(_compositeDisposable);
            
        }

        private void AddToMap(LevelSettings.CodeCombination combinationArray)
        {
            foreach (var value in combinationArray.Combination)
            {
                if (!_combinationDictionary.ContainsKey(value))
                    _combinationDictionary.Add(value, 0);

                _combinationDictionary[value]++;
            }
        }

        private LevelSettings.CodeCombination GenerateDynamicCombination(int complexity)
        {
            var percent = 0.05f * (complexity + 1);
            var HPRange = _progressHandler.GoalProgress * percent;
            var offset = 0.2f * HPRange;
            var HPCount = ((int) UnityEngine.Random.Range(HPRange - offset, HPRange + offset) / 10) * 10;
            var numOfCells = complexity + 2;
            var combination = new LevelSettings.CodeCombination();
            var intComb = _matrixHandler.GenerateJointCombination(numOfCells);
            combination.AddRange(intComb);
            combination.HPForCombination = HPCount;
            return combination;
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

        public void ResetCombinations()
        {
            _combinations.ForEach(c => c.Dispose());
            _combinations.Clear();
            _combinationDictionary.Clear();
            _compositeDisposable.Clear();
        }


        public void UpdateCombinations()
        {
            bool needToUpdateAgain = false;
            var indexes = new List<int>();
            for (int i = 0; i < _combinations.Count; i++)
            {
                if (CheckAndUpdateCombination(_combinations[i], 2))
                {
                    needToUpdateAgain = true;
                    indexes.Add(i);
                }
            }

            if (needToUpdateAgain)
            {
                foreach (var i in indexes)
                {
                    RemoveFromMap(_combinations[i].CombinationCodes);
                    _combinations[i].Dispose();
                    
                    var newCombination = _combinationProvider.CreateNew();
                    var combinationArray = GenerateDynamicCombination(i);
                    AddToMap(combinationArray);
                    newCombination.Initialize(combinationArray);
                    _combinations[i] = newCombination;   
                }
                UpdateCombinations();
            }
        }

        private void RemoveFromMap(List<CombinationCell> combinationCodes)
        {
            foreach (var value in combinationCodes)
            {
                _combinationDictionary[value.Value]--;
            }
        }

        private bool CheckAndUpdateCombination(Combination combination, int minCount)
        {
            var foundCodes =
                _matrixHandler.FindBestMatrixCombination(combination.CombinationCodes.Select(code => code as ICell)
                    .ToList());
            
            
            //Visual highlight & dim
            combination.CombinationCodes.ForEach(code=>code.DimCell(HighlightType.CombinationSequence));
            if (foundCodes == null)
                return false;

            // if (foundCodes.Count >= minCount)
            // {
            //     for (int i = 0; i < Mathf.Min(foundCodes.Count, combination.CombinationCodes.Count); i++)
            //     {
            //         foundCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
            //         combination.CombinationCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
            //     }
            // }
            combination.ChangeIsCombinationComplete(foundCodes.Count == combination.CombinationCodes.Count);
            if (foundCodes.Count == combination.CombinationCodes.Count)
            {
                foundCodes.ForEach(cell =>
                {
                    _matrixHandler.ReplaceCell(cell);
                    //cell.DimCell(HighlightType.CombinationSequence);
                });
                _progressHandler.AddProgress(combination);
                return true;
            }
            return false;
        }
    }
}