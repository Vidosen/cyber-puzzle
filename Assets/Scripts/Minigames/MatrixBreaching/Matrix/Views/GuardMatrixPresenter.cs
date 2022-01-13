﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Minigames.MatrixBreaching.Config;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Matrix.Views.Cells;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Views
{
    public class GuardMatrixPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform Holder;
        
        [Space]
        [SerializeField, Min(0)] private float _offset = 10f;
        [SerializeField, Min(0)] private float _rowsOffset = 20f;
        public float CellsOffset => _offset;
        public float RowsOffset => _rowsOffset;

        public List<GuardMatrixRowView> HorizontalRowViews => _horizontalRowViews.ToList();
        public List<GuardMatrixRowView> VerticalRowViews => _verticalRowViews.ToList();
        public List<GuardMatrixExchangerView> HorizontalExchangerViews => _horizontalExchangerViews.ToList();
        public List<GuardMatrixExchangerView> VerticalExchangerViews => _verticalExchangerViews.ToList();
        public List<BaseCellView> CellViews => _cellViews.ToList();
        public IObservable<ReplaceCellViewsEventArgs> CellViewsReplaced => _cellViewsReplacedSubject; 

        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;

        private Dictionary<BaseCellView, Tweener> _cellViewTweeners = new Dictionary<BaseCellView, Tweener>();
        private Dictionary<GuardMatrixExchangerView, Tweener> _exchangerViewTweeners = new Dictionary<GuardMatrixExchangerView, Tweener>();

        private RectTransform _transform;

        private List<GuardMatrixRowView> _horizontalRowViews = new List<GuardMatrixRowView>();
        private List<GuardMatrixRowView> _verticalRowViews = new List<GuardMatrixRowView>();
        
        private List<GuardMatrixExchangerView> _horizontalExchangerViews = new List<GuardMatrixExchangerView>();
        private List<GuardMatrixExchangerView> _verticalExchangerViews = new List<GuardMatrixExchangerView>();
        
        private List<BaseCellView> _cellViews = new List<BaseCellView>();
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private GuardMatrix _guardMatrix;
        private DiContainer _container;
        private float _gridRatio;
        private SignalBus _signalBus;
        private MatrixBreachingViewConfig _viewConfig;
        private ReactiveProperty<bool> _isInitialized = new ReactiveProperty<bool>();
        private Subject<ReplaceCellViewsEventArgs> _cellViewsReplacedSubject = new Subject<ReplaceCellViewsEventArgs>();

        [Inject]
        private void Construct(GuardMatrix guardMatrix, MatrixBreachingViewConfig viewConfig, DiContainer container,
            SignalBus signalBus)
        {
            _viewConfig = viewConfig;
            _signalBus = signalBus;
            _container = container;
            _guardMatrix = guardMatrix;
        }

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        private IEnumerator Start()
        {
            yield return null;
            InitializeFromModel(_guardMatrix);
        }
        public RectTransform Transform => _transform;

        

        public void InitializeFromModel(GuardMatrix guardMatrix)
        {
            _compositeDisposable.Clear();
            RecalculateGuradMatrixRect(guardMatrix.Size.x, guardMatrix.Size.y, _transform.parent as RectTransform);
            
            InstantiateRows(guardMatrix);
            InstantiateExchangers();
            
            InstantiateCells(guardMatrix);
            _signalBus.GetStream<MatrixOperationsSignals.SwapOperationOccured>()
                .Subscribe(signal => OnSwapOccured(signal))
                .AddTo(_compositeDisposable);
            _signalBus.GetStream<MatrixOperationsSignals.ScrollOperationOccured>()
                .Subscribe(signal => OnScrollOccured(signal))
                .AddTo(_compositeDisposable);
            _isInitialized.Value = true;
        }

        private void OnScrollOccured(MatrixOperationsSignals.ScrollOperationOccured signal)
        {
            var cellViews = new List<BaseCellView>();
            switch (signal.RowType)
            {
                case RowType.None:
                    return;
                case RowType.Horizontal:
                    cellViews.AddRange( GetHorizontalCellViews(signal.RowIndex));
                    break;
                case RowType.Vertical:
                    cellViews.AddRange( GetVerticalCellViews(signal.RowIndex));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            cellViews.ForEach(cellView=> UpdateCellViewPos(cellView));
        }

        private void OnSwapOccured(MatrixOperationsSignals.SwapOperationOccured signal)
        {
            var cellViews = new List<BaseCellView>();
            GuardMatrixExchangerView applyingExchangerView;
            GuardMatrixExchangerView appliedExchangerView;
            switch (signal.RowType)
            {
                case RowType.None:
                    return;
                case RowType.Horizontal:
                    applyingExchangerView = GetHorizontalExchangerView(signal.ApplyingRowIndex, true);
                    appliedExchangerView = GetHorizontalExchangerView(signal.AppliedRowIndex, true);
                    break;
                case RowType.Vertical:
                    applyingExchangerView = GetVerticalExchangerView(signal.ApplyingRowIndex, true);
                    appliedExchangerView = GetVerticalExchangerView(signal.AppliedRowIndex, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var isApplyingExchangerStatic = applyingExchangerView != null;
            if (isApplyingExchangerStatic)
            {
                applyingExchangerView.ChangeRowIndex(signal.AppliedRowIndex);
                UpdateExchangerViewPos(applyingExchangerView);

            }
            var isAppliedExchangerStatic = appliedExchangerView != null;
            if (isAppliedExchangerStatic)
            {
                appliedExchangerView.ChangeRowIndex(signal.ApplyingRowIndex);
                UpdateExchangerViewPos(appliedExchangerView);
            }
            
            switch (signal.RowType)
            {
                case RowType.None:
                    return;
                case RowType.Horizontal:
                    if (isApplyingExchangerStatic)
                        cellViews.AddRange( GetHorizontalCellViews(signal.AppliedRowIndex));
                    if (isAppliedExchangerStatic)
                        cellViews.AddRange( GetHorizontalCellViews(signal.ApplyingRowIndex));
                    break;
                case RowType.Vertical:
                    if (isApplyingExchangerStatic)
                        cellViews.AddRange( GetVerticalCellViews(signal.AppliedRowIndex));
                    if (isAppliedExchangerStatic)
                        cellViews.AddRange( GetVerticalCellViews(signal.ApplyingRowIndex));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            cellViews.ForEach(cellView=> UpdateCellViewPos(cellView));
        }

        public GuardMatrixExchangerView GetHorizontalExchangerView(int verticalId, bool excludeIsMoving = false)
        {
            return _horizontalExchangerViews.FirstOrDefault(exchangerView =>
                exchangerView.RowIndex == verticalId &&
                (!excludeIsMoving || !exchangerView.IsMoving));
        }
        public GuardMatrixExchangerView GetVerticalExchangerView(int horizontalId, bool excludeIsMoving = false)
        {
            return _verticalExchangerViews.FirstOrDefault(exchangerView =>
                exchangerView.RowIndex == horizontalId &&
                (!excludeIsMoving || !exchangerView.IsMoving));
        }
        public GuardMatrixRowView GetHorizontalRowView(int verticalId)
        {
            return _horizontalRowViews.First(row => row.Index.Equals(verticalId));
        }
        public GuardMatrixRowView GetVerticalRowView(int horizontalId)
        {
            return _verticalRowViews.First(row => row.Index.Equals(horizontalId));
        }
        
        public IList<BaseCellView> GetHorizontalCellViews(int verticalId)
        {
            return _cellViews.Where(cell => cell.Model.VerticalId.Equals(verticalId))
                .OrderBy(cell => cell.Model.HorizontalId).ToList();
        }
        public IList<BaseCellView> GetVerticalCellViews(int horizontalId)
        {
            return _cellViews.Where(cell => cell.Model.HorizontalId.Equals(horizontalId))
                .OrderBy(cell => cell.Model.VerticalId).ToList();
        }
        
        public BaseCellView GetCellView(int horizontalId, int verticalId)
        {
            return _cellViews.First(cell => cell.Model.HorizontalId == horizontalId && cell.Model.VerticalId == verticalId);
        }

        private void InstantiateCells(GuardMatrix guardMatrix)
        {
            foreach (var cell in guardMatrix.GetCells())
            {
                InstantiateCell(cell, false);
            }

            guardMatrix.OnCellReplaced.Subscribe(args => ReplaceCellViews(args))
                .AddTo(_compositeDisposable);
        }

        private async void ReplaceCellViews(ReplaceCellsEventArgs args)
        {
            var disposeCellView = await DisposeCell(args.DisposedCell);
            if (args.NewCell.VerticalId == -1 || args.NewCell.VerticalId == -1)
            {
                Debug.LogWarning($"View for {args.NewCell} wasn't created because it probably already has been disposed!");
                return;
            }
            var newCellView = InstantiateCell(args.NewCell, true);
            _cellViewsReplacedSubject.OnNext(new ReplaceCellViewsEventArgs(disposeCellView, newCellView));
        }

        private async UniTask<BaseCellView> DisposeCell(ICell cell)
        {
            var foundView = _cellViews.FirstOrDefault(view => view.Model == cell);
            if (foundView == null)
                return null;
            await foundView.HideAnimation();
            Destroy(foundView.gameObject);
            _cellViews.Remove(foundView);
            return foundView;
        }
        private BaseCellView InstantiateCell(ICell cell, bool animate)
        {
            var cellView = _container.InstantiatePrefabForComponent<BaseCellView>(_viewConfig.GetCellViewTemplate(cell.CellType), Holder);
            cellView.Initialize(cell, animate);
            cellView.Rescale(_gridRatio);

            UpdateCellViewPos(cellView, false);
            _cellViews.Add(cellView);
            return cellView;
        }

        public void UpdateCellViewPos(BaseCellView cellView, bool animate = true)
        {
            var model = cellView.Model;
            var horizontalView = _horizontalRowViews.FirstOrDefault(rowView => rowView.Index == model.VerticalId);
            var verticalView = _verticalRowViews.FirstOrDefault(rowView => rowView.Index == model.HorizontalId);

            if (horizontalView == null || verticalView == null)
            {
                Debug.LogError($"{cellView.Model} is not valid!");
                return;
            }
            var horizRowPos = horizontalView.Transform.anchoredPosition;
            var vertRowPos = verticalView.Transform.anchoredPosition;
            var endPos = new Vector2(vertRowPos.x, horizRowPos.y);
            if (animate && _viewConfig.SwapMoveDuration > 0)
            {
                if (_cellViewTweeners.TryGetValue(cellView, out var tweeer) && tweeer.IsActive())
                    tweeer.Kill();
                
                _cellViewTweeners[cellView] = cellView.Transform
                    .DOAnchorPos(endPos, _viewConfig.SwapMoveDuration)
                    .SetEase(_viewConfig.SwapFlatMoveEase);
            }
            else
                cellView.Transform.anchoredPosition = endPos;
        }

        public void UpdateExchangerViewPos(GuardMatrixExchangerView exchangerView, bool animate = true)
        {
            GuardMatrixRowView rowView;
            switch (exchangerView.RowType)
            {
                case RowType.None:
                    return;
                case RowType.Horizontal:
                    rowView = GetHorizontalRowView(exchangerView.RowIndex);
                    break;
                case RowType.Vertical:
                    rowView = GetVerticalRowView(exchangerView.RowIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var endPos = rowView.Transform.anchoredPosition;
            if (animate && _viewConfig.SwapMoveDuration > 0)
            {
                if (_exchangerViewTweeners.TryGetValue(exchangerView, out var tweeer) && tweeer.IsActive())
                    tweeer.Kill();
                _exchangerViewTweeners[exchangerView] = exchangerView.Transform
                    .DOAnchorPos(endPos, _viewConfig.SwapMoveDuration)
                    .SetEase(_viewConfig.SwapFlatMoveEase);
            }
            else
                exchangerView.Transform.anchoredPosition = endPos;
        }

        private void InstantiateExchangers()
        {
            foreach (var rowView in _horizontalRowViews)
            {
                var exchangerView = _container.InstantiatePrefabForComponent<GuardMatrixExchangerView>(_viewConfig.HorizontalExchangerViewTemplate, Holder);
                exchangerView.Initialize(rowView.Index);
                exchangerView.Rescale(_gridRatio);
                UpdateExchangerViewPos(exchangerView, false);
                _horizontalExchangerViews.Add(exchangerView);
            }
            
            foreach (var rowView in _verticalRowViews)
            {
                var exchangerView =  _container.InstantiatePrefabForComponent<GuardMatrixExchangerView>(_viewConfig.VerticalExchangerViewTemplate, Holder);
                exchangerView.Initialize(rowView.Index);
                exchangerView.Rescale(_gridRatio);
                UpdateExchangerViewPos(exchangerView, false);
                _verticalExchangerViews.Add(exchangerView);
            }
        }

        private void RecalculateGuradMatrixRect(int columns, int rows, RectTransform holder)
        {
            var unscaledGridSize =
                RectTransformHelper.GetGridContainer(_viewConfig.GenericCellSize, rows + 1, columns + 1,
                    _offset) + new Vector2(_rowsOffset, _rowsOffset);
            _gridRatio = RectTransformHelper.GetGridContainerRatio(holder, unscaledGridSize);
            Transform.sizeDelta = unscaledGridSize * _gridRatio;
        }
        private void InstantiateRows(GuardMatrix guardMatrix)
        {
            for (int i = 0; i < guardMatrix.Size.x; i++)
            {
                var rowView = _container.InstantiatePrefabForComponent<GuardMatrixRowView>(_viewConfig.VerticalRowViewTemplate, Holder);
                rowView.Initialize(i);
                rowView.Rescale(_gridRatio);
                rowView.Transform.anchoredPosition3D =
                    RectTransformHelper.GetChildPositionContainer(rowView.Transform.rect, i + 1, 0, _offset);
                _verticalRowViews.Add(rowView);
            }

            for (int i = 0; i < guardMatrix.Size.y; i++)
            {
                var rowView = _container.InstantiatePrefabForComponent<GuardMatrixRowView>(_viewConfig.HorizontalRowViewTemplate, Holder);
                rowView.Initialize(i);
                rowView.Rescale(_gridRatio);
                rowView.Transform.anchoredPosition3D =
                    RectTransformHelper.GetChildPositionContainer(rowView.Transform.rect, 0, i + 1, _offset);
                _horizontalRowViews.Add(rowView);
            }
        }

        public bool TryGetCellMoveAnimationTweener(BaseCellView cellView, out Tweener tweerner)
        {
            return _cellViewTweeners.TryGetValue(cellView, out tweerner);
        }

        public void OnDestroy()
        {
            _isInitialized.Value = false;
            _isInitialized.Dispose();
            _compositeDisposable.Clear();
        }
    }
}