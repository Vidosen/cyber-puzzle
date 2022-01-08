using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Vulnerabilities.Models;
using Minigames.MatrixBreaching.Vulnerabilities.Services;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Vulnerabilities.Rules
{
    public class CheckVulnerabilitiesRule : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly VulnerabiltyInventory _vulnerabiltyInventory;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public CheckVulnerabilitiesRule(SignalBus signalBus, VulnerabiltyInventory vulnerabiltyInventory,
            VulnerabilityService vulnerabilityService)
        {
            _vulnerabiltyInventory = vulnerabiltyInventory;
            _vulnerabilityService = vulnerabilityService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.GetStream<MatrixOperationsSignals.IOperationSignal>().Subscribe(_ => CheckVulnerabilities())
                .AddTo(_compositeDisposable);
            _vulnerabiltyInventory.ModelAdded.Subscribe(addEvent => CheckVulnerabilities())
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
        }

        private void CheckVulnerabilities()
        {
            foreach (var model in _vulnerabiltyInventory)
                CheckVulnerability(model);
        }

        private void CheckVulnerability(VulnerabilityModel model)
        {
            IList<ValueCell> foundMatchedMatrixCells;
            if (model.IsInitialized)
                foundMatchedMatrixCells =
                    _vulnerabilityService.FindBestMatchForVulnerabilitySequence(model.VulnerabilitySequence);
            else
                foundMatchedMatrixCells = new List<ValueCell>();

            model.SetMatchedMatrixSequence(foundMatchedMatrixCells);
        }
    }
}