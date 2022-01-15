using System;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using TMPro;
using UniRx;
using UnityEngine;
using Utils;

namespace Minigames.MatrixBreaching.Matrix.Views.Cells
{
    public class GlitchCellView : BaseCellView<GlitchCell>
    {
        [SerializeField] private TextMeshProUGUI _valueText;
        private Sequence _glitchSequence;

        public override void Initialize(ICell cellModel, bool animateShow)
        {
            base.Initialize(cellModel, animateShow);
            if (animateShow)
                Transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutQuad);

            _concrecteModel.ValueChanged.Subscribe(newValue =>
            {
                if (Transform == null)
                    return;
                UpdateValueText(newValue);
                PlayGlitchAnimation();
            }).AddTo(this);
            UpdateValueText(_concrecteModel.Value);
        }
        private void UpdateValueText(CellValueType value)
        {
            _valueText.text = value.ToTextString();
        }

        private void PlayGlitchAnimation()
        {
            StopGlitchAnimation();
            _glitchSequence = DOTween.Sequence()
                .Append(Transform.DORotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.FastBeyond360))
                .Join(DOTween.Sequence()
                    .Append(Transform.DOScale(1.2f, 0.25f).From(1f))
                    .Append(Transform.DOScale(1, 0.25f)));
        }

        private void StopGlitchAnimation()
        {
            if (_glitchSequence.IsActive())
                _glitchSequence.Kill();
        }

        private void OnDestroy()
        {
            StopGlitchAnimation();
        }
    }
}