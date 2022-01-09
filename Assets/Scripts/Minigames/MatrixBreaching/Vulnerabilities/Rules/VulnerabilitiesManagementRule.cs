using System.Linq;
using Cysharp.Threading.Tasks;
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
        private VulnerabiltyInventory _vulnerabiltyInventory;
        private SignalBus _signalBus;

        public VulnerabilitiesManagementRule(VulnerabilityService vulnerabilityService,
            VulnerabiltyInventory vulnerabiltyInventory, SignalBus signalBus, GuardMatrix guardMatrix)
        {
            _signalBus = signalBus;
            _vulnerabiltyInventory = vulnerabiltyInventory;
            _vulnerabilityService = vulnerabilityService;
            _guardMatrix = guardMatrix;
        }
        public async void Initialize()
        {
            if (!_guardMatrix.IsInitialized)
                await UniTask.WaitUntil(() => _guardMatrix.IsInitialized);
            
            _vulnerabiltyInventory.ModelRemoved.Subscribe(removeEvent =>
            {
                _vulnerabilityService.CreateNewVulnerability(removeEvent.Value.SequenceSize);
                RemoveCompletedVulnerabilities();
            });
            _signalBus.Subscribe<MatrixOperationsSignals.OperationApplied>(RemoveCompletedVulnerabilities);
            _vulnerabilityService.CreateNewVulnerability(3);
            _vulnerabilityService.CreateNewVulnerability(4);
            _vulnerabilityService.CreateNewVulnerability(5);
            
            
        }

        private void RemoveCompletedVulnerabilities()
        {
            if (_vulnerabilityService.FindCompletedVulnerabilities(out var foundVulnerabilites))
            {
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