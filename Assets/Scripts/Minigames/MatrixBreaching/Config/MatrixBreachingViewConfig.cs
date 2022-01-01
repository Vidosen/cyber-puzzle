using DG.Tweening;
using Minigames.MatrixBreaching.Views;
using UnityEngine;

namespace Minigames.MatrixBreaching.Config
{
    [CreateAssetMenu(fileName = "MatrixBreachingViewConfig", menuName = "Configs/Matrix Breaching View", order = 0)]
    public class MatrixBreachingViewConfig : ScriptableObject
    {
        [Header("Templates")]
        [SerializeField] private GuardMatrixRowView _verticalRowViewTemplate;
        [SerializeField] private GuardMatrixRowView _horizontalRowViewTemplate;
        
        [SerializeField] private GuardMatrixExchangerView _verticalExchangerViewTemplate;
        [SerializeField] private GuardMatrixExchangerView _horizontalExchangerViewTemplate;
        
        [SerializeField] private ValueCellView _valueCellViewTemplate;
        [Header("Animation")]
        [SerializeField] private Ease _swapFlatMoveEase;
        [SerializeField] private float _swapMoveDuration;

        public GuardMatrixRowView VerticalRowViewTemplate => _verticalRowViewTemplate;
        public GuardMatrixRowView HorizontalRowViewTemplate => _horizontalRowViewTemplate;
        public GuardMatrixExchangerView VerticalExchangerViewTemplate => _verticalExchangerViewTemplate;
        public GuardMatrixExchangerView HorizontalExchangerViewTemplate => _horizontalExchangerViewTemplate;
        public ValueCellView ValueCellViewViewTemplate => _valueCellViewTemplate;
        public Ease SwapFlatMoveEase => _swapFlatMoveEase;
        public float SwapMoveDuration => _swapMoveDuration;
    }
}