using System.Globalization;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Progress;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minigames.MatrixBreaching
{
    public class BreachingProgressView : MonoBehaviour
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _targetProgressText;
        private MatrixBreachingModel _model;
        private MatrixBreachingService _matrixBreachingService;

        [Inject]
        private void Construct(MatrixBreachingModel model, MatrixBreachingService matrixBreachingService)
        {
            _matrixBreachingService = matrixBreachingService;
            _model = model;
        }

        private async void Start()
        {
            if (_matrixBreachingService.Status != MatrixBreachingData.Status.Process)
                await _matrixBreachingService.WaitForProccessStatusAsync();
            _targetProgressText.text = _model.TargetProgress.ToString(CultureInfo.InvariantCulture);
            OnProgressChanged();
            _model.ProgressChanged.Subscribe(_ => OnProgressChanged()).AddTo(this);
        }

        private void OnProgressChanged()
        {
            _progressSlider.value = _model.CurrentRelativeProgress;
        }
    }
}