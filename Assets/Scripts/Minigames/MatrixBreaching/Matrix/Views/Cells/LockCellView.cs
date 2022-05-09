using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames.MatrixBreaching.Matrix.Views.Cells
{
    public class LockCellView : BaseCellView<LockCell>
    {
        [SerializeField] private RectTransform _topChainImg;
        [SerializeField] private RectTransform _rightChainImg;
        [SerializeField] private RectTransform _bottmChainImg;
        [SerializeField] private RectTransform _leftChainImg;
        public override void Initialize(ICell cellModel, bool animateShow)
        {
            base.Initialize(cellModel, animateShow);
            if (animateShow)
                Transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutQuad);
        }

        public void PlayLockAnimation(float duration, Vector2 cellOffset, Vector2Int matrixSize)
        {
            var cellSize = Transform.sizeDelta;
            var targetTopWidth = _concrecteModel.VerticalId * (cellOffset.y + cellSize.y);
            var targetBottomWidth = (matrixSize.y - _concrecteModel.VerticalId - 1) * (cellOffset.y + cellSize.y);
            var targetLeftWidth = _concrecteModel.HorizontalId * (cellOffset.x + cellSize.x);
            var targetRightWidth = (matrixSize.x - _concrecteModel.HorizontalId - 1) * (cellOffset.x + cellSize.x);
            DOTween.Sequence()
                .Join(_topChainImg.DOSizeDelta(new Vector2(targetTopWidth, _topChainImg.sizeDelta.y), duration))
                .Join(_bottmChainImg.DOSizeDelta(new Vector2(targetBottomWidth, _bottmChainImg.sizeDelta.y), duration))
                .Join(_leftChainImg.DOSizeDelta(new Vector2(targetLeftWidth, _leftChainImg.sizeDelta.y), duration))
                .Join(_rightChainImg.DOSizeDelta(new Vector2(targetRightWidth, _rightChainImg.sizeDelta.y), duration));
        }
        public async UniTask PlayUnlockAnimation(float duration)
        {
            var unlockSequence = DOTween.Sequence()
                .Join(_topChainImg.DOSizeDelta(new Vector2(0, _topChainImg.sizeDelta.y), duration))
                .Join(_bottmChainImg.DOSizeDelta(new Vector2(0, _bottmChainImg.sizeDelta.y), duration))
                .Join(_leftChainImg.DOSizeDelta(new Vector2(0, _leftChainImg.sizeDelta.y), duration))
                .Join(_rightChainImg.DOSizeDelta(new Vector2(0, _rightChainImg.sizeDelta.y), duration));
            await unlockSequence.AsyncWaitForKill();
        }

        private void InvertPivotX(RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0, rectTransform.pivot.y);
            rectTransform.anchoredPosition -= new Vector2(rectTransform.sizeDelta.x, 0);
        }
    }
}