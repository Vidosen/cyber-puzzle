using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Core;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Vulnerabilities.Services;
using UniRx;
using UnityEngine;
using Viruses.Models;
using Zenject;

namespace Minigames.MatrixBreaching
{
    public class MatrixMinigameFacade : MonoBehaviour
    {
        private MatrixBreachingService _matrixBreachingService;
        private GuardMatrix _guardMatrix;
        private List<IReqiureRandomSeed> _seedDependentObjects;
        private MatrixBreachingModel _matrixBreachingModel;
        private VulnerabilityService _vulnerabilityService;

        [Inject]
        protected void Construct(MatrixBreachingService matrixBreachingService, GuardMatrix guardMatrix,
            List<IReqiureRandomSeed> seedDependent, MatrixBreachingModel matrixBreachingModel,
            VulnerabilityService vulnerabilityService)
        {
            _vulnerabilityService = vulnerabilityService;
            _matrixBreachingModel = matrixBreachingModel;
            _seedDependentObjects = seedDependent;
            _guardMatrix = guardMatrix;
            _matrixBreachingService = matrixBreachingService;
        }
        private void Start()
        {
            _matrixBreachingService.BindController(this);
        }
        
        public void InitializeMinigame(MatrixBreachingData.Settings settings)
        {
            _matrixBreachingModel.Reset();
            _matrixBreachingModel.Initialize(settings);
            
            SetSeed(settings.Id.GetHashCode());
            InitializeMatrix(settings.MatrixSize);
            InitializeVulnerabilites(settings.MaxConcurrentVulnerabilities);
        }
        private void InitializeMatrix(Vector2Int matrixSize)
        {
            _guardMatrix.Initialize(matrixSize.x, matrixSize.y);
        }
        
        public void SetSeed(int seed)
        {
            _seedDependentObjects.ForEach(obj => obj.SetRandomSeed(seed));
        }


        private void InitializeVulnerabilites(int concurrentVulnerabilities)
        {
            var maxVulnerabilitySize = Mathf.Max(_guardMatrix.Size.x, _guardMatrix.Size.y) - 1;
            var minVulnerabilitySize = Mathf.Max(2, maxVulnerabilitySize - concurrentVulnerabilities);
            for (int size = minVulnerabilitySize; size <= maxVulnerabilitySize; size++)
            {
                _vulnerabilityService.CreateNewVulnerability(size);
            }
        }

        public IObservable<Unit> FilledProgressAsObservable()
        {
            return _matrixBreachingModel.ProgressChanged
                .Where(_ => _matrixBreachingModel.CurrentProgress >= _matrixBreachingModel.TargetProgress)
                .AsUnitObservable();
        }

        public float GetProgress()
        {
            return _matrixBreachingModel.CurrentRelativeProgress;
        }
    }
}