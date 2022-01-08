using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Vulnerabilities.Services;
using Zenject;

namespace Minigames.MatrixBreaching.Vulnerabilities.Rules
{
    public class CheckVulnerabilitiesRule : IInitializable
    {
        private SignalBus _signalBus;
        private VulnerabiltyInventory _vulnerabiltyInventory;
        private readonly VulnerabilityService _vulnerabilityService;

        public CheckVulnerabilitiesRule(SignalBus signalBus, VulnerabiltyInventory vulnerabiltyInventory, VulnerabilityService vulnerabilityService)
        {
            _vulnerabiltyInventory = vulnerabiltyInventory;
            _vulnerabilityService = vulnerabilityService;
            _signalBus = signalBus;
        }
        public void Initialize()
        {
            _signalBus.Subscribe<MatrixOperationsSignals.IOperationSignal>(CheckVulnerabilities);
        }

        private void CheckVulnerabilities()
        {
            foreach (var model in _vulnerabiltyInventory)
            {
                IList<ValueCell> foundMatchedMatrixCells = _vulnerabilityService.FindBestMatchForVulnerability(model);
                model.SetMatchedMatrixSequence(foundMatchedMatrixCells);
            }
        }
    }
}