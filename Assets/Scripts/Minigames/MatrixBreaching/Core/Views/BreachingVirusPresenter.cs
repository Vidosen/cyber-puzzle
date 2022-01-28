using Cysharp.Threading.Tasks;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Viruses.Models;
using Zenject;

namespace Minigames.MatrixBreaching.Core.Views
{
    public class BreachingVirusPresenter : MonoBehaviour
    {
        [SerializeField] private Slider _durabilitySlider;
        private MatrixBreachingService _matrixBreachingService;
        private VirusModel _attachedVirus;

        [Inject]
        public void Construct(MatrixBreachingService matrixBreachingService)
        {
            _matrixBreachingService = matrixBreachingService;
        }
        private async void Start()
        {
            if (_matrixBreachingService.Status != MatrixBreachingData.Status.Process)
                await _matrixBreachingService.WaitForProccessStatusAsync();
            
            _attachedVirus = _matrixBreachingService.AttachedVirus.Value;
            UpdateDurabilityView();
            _attachedVirus.CurrentDurability.Subscribe(_ => UpdateDurabilityView())
                .AddTo(this);
        }

        private void UpdateDurabilityView()
        {
            _durabilitySlider.value = _attachedVirus.MaxDurability > 0
                ? _attachedVirus.CurrentDurability.Value / _attachedVirus.MaxDurability
                : 0;
        }
    }
}
