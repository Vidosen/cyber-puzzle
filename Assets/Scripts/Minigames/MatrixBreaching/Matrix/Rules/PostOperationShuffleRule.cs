using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Matrix.Views;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Minigames.MatrixBreaching.Matrix.Rules
{
    public class PostOperationShuffleRule : IInitializable, IReqiureRandomSeed
    {
        private readonly SignalBus _signalBus;
        private readonly GuardMatrix _guardMatrix;
        private readonly GuardMatrixPresenter _guardMatrixPresenter;
        private Random _random;

        public PostOperationShuffleRule(SignalBus signalBus, GuardMatrix guardMatrix, GuardMatrixPresenter guardMatrixPresenter)
        {
            _signalBus = signalBus;
            _guardMatrix = guardMatrix;
            _guardMatrixPresenter = guardMatrixPresenter;
        }
        public void Initialize()
        {
            _signalBus.Subscribe<MatrixOperationsSignals.OperationApplied>(ApplyShuffleForInvolvedCells);
        }

        private void ApplyShuffleForInvolvedCells(MatrixOperationsSignals.OperationApplied signal)
        {
            var shuffleCells = signal.InvolvedCells.Where(cell => cell.CellType == CellType.Shuffle)
                .Select(cell => (ShuffleCell)cell);
            
            foreach (var shuffleCell in shuffleCells)
                ApplyShuffle(shuffleCell);
        }
        public void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }
        private async void ApplyShuffle(ShuffleCell shuffleCell)
        {
            var shuffleTargetCells = new[]
            {
                _guardMatrix.GetCell(shuffleCell.HorizontalId, shuffleCell.VerticalId + 1),
                _guardMatrix.GetCell(shuffleCell.HorizontalId + 1, shuffleCell.VerticalId + 1),
                _guardMatrix.GetCell(shuffleCell.HorizontalId + 1, shuffleCell.VerticalId),
                _guardMatrix.GetCell(shuffleCell.HorizontalId + 1, shuffleCell.VerticalId - 1),
                _guardMatrix.GetCell(shuffleCell.HorizontalId, shuffleCell.VerticalId - 1),
                _guardMatrix.GetCell(shuffleCell.HorizontalId - 1, shuffleCell.VerticalId - 1),
                _guardMatrix.GetCell(shuffleCell.HorizontalId - 1, shuffleCell.VerticalId),
                _guardMatrix.GetCell(shuffleCell.HorizontalId - 1, shuffleCell.VerticalId + 1)
            }.Where(cell => cell != null && cell is ValueCell)
                .OrderBy(_ => _random.Next())
                .ToList();
            var shuffleQueue = new Queue<ICell>(shuffleTargetCells);
            if (shuffleQueue.Count > 0)
            {
                ICell currentCell = shuffleQueue.Dequeue();
                var firstPos = new Vector2Int(currentCell.HorizontalId, currentCell.VerticalId);
                while (shuffleQueue.Count > 0)
                {
                    var next = shuffleQueue.Dequeue();
                    currentCell.Move(next.HorizontalId, next.VerticalId);
                    currentCell = next;

                    if (shuffleQueue.Count == 0)
                        next.Move(firstPos.x, firstPos.y);
                }

                var taskList = new List<UniTask>();
                foreach (var targetCell in shuffleTargetCells)
                {
                    var cellView = _guardMatrixPresenter.GetCellView(targetCell.HorizontalId, targetCell.VerticalId);
                    taskList.Add(_guardMatrixPresenter.UpdateCellViewPosAsync(cellView, shuffleCell.HorizontalId, shuffleCell.VerticalId));
                }
                await UniTask.WhenAll(taskList);
                taskList.Clear();
                foreach (var targetCell in shuffleTargetCells)
                {
                    var cellView = _guardMatrixPresenter.GetCellView(targetCell.HorizontalId, targetCell.VerticalId);
                    taskList.Add(_guardMatrixPresenter.UpdateCellViewPosAsync(cellView));
                }
                await UniTask.WhenAll(taskList);
            }
            _guardMatrix.ReplaceCell(shuffleCell);
            _signalBus.AbstractFire(new MatrixOperationsSignals.PostOperationOccured(OperationType.Shuffle));
        }
    }
}