using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using TMPro;
using UnityEngine;
using Utils;

namespace Minigames.MatrixBreaching.Matrix.Views.Cells
{
    public class ValueCellView : BaseCellView<ValueCell>
    {
        [SerializeField] private TextMeshProUGUI _valueText;

        public override void Initialize(ICell cellModel, bool animateShow)
        {
            base.Initialize(cellModel, animateShow);
            if (animateShow)
                Transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutQuad);
            _valueText.text = _concrecteModel.Value.ToTextString();
        }
    }
}