using System;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Matrix.Views.Cells;
using UniRx;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class PostOperationLockRule : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GuardMatrix _guardMatrix;
        private readonly GuardMatrixPresenter _guardMatrixPresenter;

        public PostOperationLockRule(SignalBus signalBus, GuardMatrix guardMatrix, GuardMatrixPresenter guardMatrixPresenter)
        {
            _signalBus = signalBus;
            _guardMatrix = guardMatrix;
            _guardMatrixPresenter = guardMatrixPresenter;
        }
        public void Initialize()
        {
            _signalBus.Subscribe<MatrixOperationsSignals.OperationApplied>(ApplyLockForInvolvedCells);
        }

        private void ApplyLockForInvolvedCells(MatrixOperationsSignals.OperationApplied signal)
        {
            var lockCells = signal.InvolvedCells.Where(cell => cell.CellType == CellType.Lock)
                .Select(cell => (LockCell)cell);
            
            foreach (var lockCell in lockCells)
            {
                lockCell.ApplyLock(5f);
                var lockCellView = _guardMatrixPresenter.GetCellView(lockCell.HorizontalId, lockCell.VerticalId) as LockCellView;
                if (lockCellView == null)
                {
                    throw new InvalidOperationException(
                        $"Couldn't play Lock animation for cell {lockCell} because view wasn't found!");
                }
                lockCellView.PlayLockAnimation(0.25f,
                    new Vector2(_guardMatrixPresenter.CellsOffset, _guardMatrixPresenter.CellsOffset),
                    _guardMatrix.Size);
                lockCell.IsLocked.Where(isLocked => !isLocked)
                    .Subscribe(_=>ReplaceUnlockedCell(lockCellView))
                    .AddTo(lockCell.CellDisposable);
            }
        }

        private async void ReplaceUnlockedCell(LockCellView lockCellView)
        {
            await lockCellView.PlayUnlockAnimation(0.25f);
            _guardMatrix.ReplaceCell(lockCellView.Model);
            _signalBus.AbstractFire(new MatrixOperationsSignals.PostOperationOccured(OperationType.Lock));
        }
    }
}