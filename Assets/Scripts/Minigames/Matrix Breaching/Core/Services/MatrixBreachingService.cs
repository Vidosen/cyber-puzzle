using System;
using Cysharp.Threading.Tasks;
using Minigames.MatrixBreaching.Core.Data;
using UniRx;
using Viruses.Models;

namespace Minigames.MatrixBreaching.Core.Services
{
    public class MatrixBreachingService
    {
        public IReadOnlyReactiveProperty<VirusModel> AttachedVirus => _attachedVirus;
        public MatrixBreachingData.Status Status => _statusReactivePropertyProperty.Value;
        public IReadOnlyReactiveProperty<MatrixBreachingData.Status> StatusReactiveProperty => _statusReactivePropertyProperty;
        private ReactiveProperty<MatrixBreachingData.Status> _statusReactivePropertyProperty =
            new ReactiveProperty<MatrixBreachingData.Status>(MatrixBreachingData.Status.Await);
        
        private MatrixMinigameFacade _matrixMinigameFacade;
        private ReactiveProperty<VirusModel> _attachedVirus = new ReactiveProperty<VirusModel>();

        public async UniTask<MatrixBreachingData.MinigameResult> StartMinigame(VirusModel model, MatrixBreachingData.Settings settings)
        {
            _attachedVirus.Value = model;
            if (_matrixMinigameFacade == null)
            {
                await UniTask.WaitUntil(() => _matrixMinigameFacade != null);
            }
            _matrixMinigameFacade.SetSeed(settings.Id.GetHashCode());
            _matrixMinigameFacade.InitializeMinigame(settings);
            _statusReactivePropertyProperty.Value = MatrixBreachingData.Status.Process;

            var succeed = await FinishedMinigameAsObservable().ToUniTask(true);
            var progress = _matrixMinigameFacade.GetProgress();
            _statusReactivePropertyProperty.Value = MatrixBreachingData.Status.Await;
            _attachedVirus.Value = null;
            return new MatrixBreachingData.MinigameResult(progress, succeed);
        }

        public void BindController(MatrixMinigameFacade matrixMinigameFacade)
        {
            _matrixMinigameFacade = matrixMinigameFacade;
        }

        public IObservable<bool> FinishedMinigameAsObservable()
        {
            return _attachedVirus.Value.OutOfDurabilityObservable.Select(_ => false)
                .Merge(_matrixMinigameFacade.FilledProgressAsObservable().Select(_ => true));
        }

        public async UniTask WaitForProccessStatusAsync()
        {
            await StatusReactiveProperty.Where(status => status == MatrixBreachingData.Status.Process)
                .ToUniTask(true);
        }
    }
}
