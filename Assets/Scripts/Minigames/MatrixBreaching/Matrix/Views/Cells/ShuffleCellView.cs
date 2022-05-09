using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using UnityEngine;

namespace Minigames.MatrixBreaching.Matrix.Views.Cells
{
    public class ShuffleCellView : BaseCellView<ShuffleCell>
    {
        public override void Initialize(ICell cellModel, bool animateShow)
        {
            base.Initialize(cellModel, animateShow);
            if (animateShow)
                Transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutQuad);
        }
    }
}