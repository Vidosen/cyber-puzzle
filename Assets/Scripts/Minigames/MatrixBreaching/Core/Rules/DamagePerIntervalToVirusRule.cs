using System;
using Minigames.MatrixBreaching.Config;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Progress;
using UniRx;
using Viruses.Models;
using Zenject;

namespace Minigames.MatrixBreaching.Core.Rules
{
    public class DamagePerIntervalToVirusRule : IInitializable, IDisposable
    {
        private readonly MatrixBreachingGeneralConfig _matrixBreachingGeneralConfig;
        private readonly MatrixBreachingService _matrixBreachingService;
        private readonly MatrixBreachingModel _matrixBreachingModel;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private IDisposable _damageStream;

        public DamagePerIntervalToVirusRule(MatrixBreachingGeneralConfig matrixBreachingGeneralConfig,
            MatrixBreachingService matrixBreachingService, MatrixBreachingModel matrixBreachingModel)
        {
            _matrixBreachingGeneralConfig = matrixBreachingGeneralConfig;
            _matrixBreachingService = matrixBreachingService;
            _matrixBreachingModel = matrixBreachingModel;
        }

        public void Initialize()
        {
            _matrixBreachingService.StatusReactiveProperty.Where(status => status == MatrixBreachingData.Status.Process)
                .Subscribe(_ => StartDealDamageToVirus()).AddTo(_compositeDisposable);
            _matrixBreachingService.StatusReactiveProperty.Where(status => status != MatrixBreachingData.Status.Process)
                .Subscribe(_ => TryStopDealDamageToVirus()).AddTo(_compositeDisposable);
        }

        private void TryStopDealDamageToVirus()
        {
            _damageStream?.Dispose();
        }

        private void StartDealDamageToVirus()
        {
            var damageInterval = TimeSpan.FromSeconds(_matrixBreachingGeneralConfig.DamageToVirusInterval);
            
            _damageStream = Observable.Interval(damageInterval, Scheduler.MainThread)
                .Select(_ => _matrixBreachingService.AttachedVirus.Value)
                .Where(model => model != null)
                .Subscribe(model => DealDamage(model))
                .AddTo(_compositeDisposable);
        }

        private void DealDamage(VirusModel model)
        {
            model.DealDamage(_matrixBreachingGeneralConfig.DamageToVirusInterval *
                             _matrixBreachingModel.Settings.DamageToVirusPerSec);
        }

        public void Dispose()
        {
            _compositeDisposable.Clear();
        }
    }
}