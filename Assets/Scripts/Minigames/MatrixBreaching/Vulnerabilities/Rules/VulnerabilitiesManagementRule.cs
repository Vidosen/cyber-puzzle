using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Vulnerabilities.Services;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Vulnerabilities.Rules
{
    public class VulnerabilitiesManagementRule : IInitializable
    {
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly GuardMatrix _guardMatrix;
        private readonly MatrixBreachingService _matrixBreachingService;
        private VulnerabiltyInventory _vulnerabiltyInventory;
        private SignalBus _signalBus;

        private List<OperationType> _allowedInstantOperations =
            new List<OperationType> { OperationType.Glitch, OperationType.Lock, OperationType.Shuffle };

        public VulnerabilitiesManagementRule(VulnerabilityService vulnerabilityService,
            VulnerabiltyInventory vulnerabiltyInventory, SignalBus signalBus, GuardMatrix guardMatrix, MatrixBreachingService matrixBreachingService)
        {
            _signalBus = signalBus;
            _vulnerabiltyInventory = vulnerabiltyInventory;
            _vulnerabilityService = vulnerabilityService;
            _guardMatrix = guardMatrix;
            _matrixBreachingService = matrixBreachingService;
        }
        public async void Initialize()
        {
            if (!_guardMatrix.IsInitialized)
                await UniTask.WaitUntil(() => _guardMatrix.IsInitialized);
            
            _vulnerabiltyInventory.ModelRemoved
                .Where(_=>_matrixBreachingService.Status == MatrixBreachingData.Status.Process)
                .Subscribe(removeEvent =>
            {
                _vulnerabilityService.CreateNewVulnerability(removeEvent.Value.SequenceSize);
                RemoveCompletedVulnerabilities();
            });
            _signalBus.GetStream<MatrixOperationsSignals.IOperationSignal>().Where(signal=> _allowedInstantOperations.Contains(signal.OperationType))
                .AsUnitObservable()
                .Merge(_signalBus.GetStream<MatrixOperationsSignals.OperationApplied>().AsUnitObservable())
                .Subscribe(_=>RemoveCompletedVulnerabilities());
        }

        private async void RemoveCompletedVulnerabilities()
        {
            if (_vulnerabilityService.FindCompletedVulnerabilities(out var foundVulnerabilites))
            {
                await UniTask.Delay(800);
                foreach (var valueCell in foundVulnerabilites.SelectMany(model => model.MatchedSequence).Distinct())
                {
                    _guardMatrix.ReplaceCell(valueCell);
                }
                foreach (var model in foundVulnerabilites)
                {
                    _vulnerabiltyInventory.Remove(model);
                }
            }
        }
    }
}