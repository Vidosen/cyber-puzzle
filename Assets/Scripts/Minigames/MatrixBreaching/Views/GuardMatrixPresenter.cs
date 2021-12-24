using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Models;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Views
{
    public class GuardMatrixPresenter : MonoBehaviour
    {
        [SerializeField] private Transform Holder;
        [SerializeField] private GuardMatrixRowView verticalGuardMatrixRowViewPrefab;
        [SerializeField] private GuardMatrixRowView horizontalGuardMatrixRowViewPrefab;
        
        [SerializeField] private GuardMatrixExchangerView verticalGuardMatrixExchangerViewPrefab;
        [SerializeField] private GuardMatrixExchangerView horizontalGuardMatrixExchangerViewPrefab;
        
        [SerializeField] private ValueCellView valueCellPrefab;
        
        [Space, SerializeField] private float Offset = 10f;
        [Space, SerializeField] private float SlotOffset = 20f;
        
        private RectTransform _transform;

        private List<GuardMatrixRowView> horizontalRowViews = new List<GuardMatrixRowView>();
        private List<GuardMatrixRowView> verticalRowViews = new List<GuardMatrixRowView>();
        
        private List<GuardMatrixExchangerView> horizontalExchangerViews = new List<GuardMatrixExchangerView>();
        private List<GuardMatrixExchangerView> verticalExchangerViews = new List<GuardMatrixExchangerView>();
        
        private List<ValueCellView> cellViews = new List<ValueCellView>();
        
        private GuardMatrix _guardMatrix;

        [Inject]
        private void Construct(GuardMatrix guardMatrix)
        {
            _guardMatrix = guardMatrix;
        }

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            InitializeFromModel(_guardMatrix);
        }
        public RectTransform Transform => _transform;

        

        public void InitializeFromModel(GuardMatrix guardMatrix)
        {
            var gridRatio = RecalculateGuradMatrixRect(guardMatrix.Size.x, guardMatrix.Size.y, transform.parent as RectTransform);
            
            InstantiateRows(guardMatrix, gridRatio);
            InstantiateExchangers(gridRatio);
            
            InstantiateCells(guardMatrix, gridRatio);

        }

        private void InstantiateCells(GuardMatrix guardMatrix, float gridRatio)
        {
            foreach (var cell in guardMatrix.GetCells())
            {
                var cellView = Instantiate(valueCellPrefab, Holder);
                cellView.Initialize(cell, gridRatio);
                
                cell.CellUpdated.Subscribe(_ => UpdateCellViewPos(cellView)).AddTo(cellView);
                UpdateCellViewPos(cellView);
                cellViews.Add(cellView);
            }
        }

        private void UpdateCellViewPos(ValueCellView cellView)
        {
            var model = cellView.Model;
            var horizontalView = horizontalRowViews.First(rowView => rowView.Index.Equals(model.HorizontalId));
            var verticalView = verticalRowViews.First(rowView => rowView.Index.Equals(model.VerticalId));
            
            var horizPos = horizontalView.Transform.anchoredPosition;
            var vertPos = verticalView.Transform.anchoredPosition;
            
            var targetPos = new Vector2(horizPos.x + horizontalView.Transform.rect.width / 2,
                vertPos.y - verticalView.Transform.rect.height / 2);
            
            cellView.Transform.anchoredPosition = targetPos;
        }

        private void InstantiateExchangers(float gridRatio)
        {
            foreach (var rowView in horizontalRowViews)
            {
                var exchangerView = Instantiate(horizontalGuardMatrixExchangerViewPrefab, Holder);
                exchangerView.Initialize(rowView, gridRatio);
                horizontalExchangerViews.Add(exchangerView);
            }
            
            foreach (var rowView in verticalRowViews)
            {
                var exchangerView = Instantiate(verticalGuardMatrixExchangerViewPrefab, Holder);
                exchangerView.Initialize(rowView, gridRatio);
                verticalExchangerViews.Add(exchangerView);
            }
        }

        private float RecalculateGuradMatrixRect(int columns, int rows, RectTransform holder)
        {
            var unscaledGridSize =
                RectTransformHelper.GetGridContainer(new Vector2(120f,120f), rows + 1, columns + 1,
                    Offset) + new Vector2(SlotOffset, SlotOffset);
            var gridRatio = RectTransformHelper.GetGridContainerRatio(holder, unscaledGridSize);
            Transform.sizeDelta = unscaledGridSize * gridRatio;
            return gridRatio;
        }
        private void InstantiateRows(GuardMatrix guardMatrix, float gridRatio)
        {
            for (int i = 0; i < guardMatrix.Size.x; i++)
            {
                var rowView = Instantiate(horizontalGuardMatrixRowViewPrefab, Holder);
                rowView.Initialize(i, gridRatio);
                rowView.Transform.localPosition =
                    RectTransformHelper.GetChildPositionContainer(_transform.rect, i + 1, 0, Offset) +
                    new Vector3(SlotOffset, 0, -5f);
                horizontalRowViews.Add(rowView);
            }

            for (int i = 0; i < guardMatrix.Size.y; i++)
            {
                var rowView = Instantiate(verticalGuardMatrixRowViewPrefab, Holder);
                rowView.Initialize(i, gridRatio);
                rowView.Transform.localPosition =
                    RectTransformHelper.GetChildPositionContainer(_transform.rect, 0, i + 1, Offset) +
                    new Vector3(0, -SlotOffset, -5f);
                verticalRowViews.Add(rowView);
            }
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}