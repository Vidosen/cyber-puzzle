using UnityEngine;

namespace Prototype.Scripts.Utils
{
    public class GridCreator : MonoBehaviour
    {
        public RectTransform RowsSlotPrefab;
        public RectTransform ColumnsSlotPrefab;
        public RectTransform CellSlotPrefab;

        public float Offset;

        public int Rows;
        public int Columns;
        
        public void ClearObjectChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    var child = transform.GetChild(i);
                    DestroyImmediate(child.gameObject);
                }

                Debug.Log($"{transform.name} is cleared!");
        }

        public void ApplyGridCreatorModifications()
        {
            ClearObjectChildren();

            ((RectTransform) transform).sizeDelta =
                RectTransformHelper.GetGridContainer(CellSlotPrefab.rect, Rows, Columns, Offset);

            for (int i = 1; i < Columns; i++)
            {
                var child = Instantiate(RowsSlotPrefab, transform, false);
                child.localPosition = RectTransformHelper.GetChildPsotionContainer(child.rect, 0, i, Offset);
            }
            for (int i = 1; i < Rows; i++)
            {
                var child = Instantiate(ColumnsSlotPrefab, transform, false);
                child.localPosition = RectTransformHelper.GetChildPsotionContainer(child.rect, i, 0, Offset);
            }
            
            for (int row = 1; row < Rows; row++)
            {
                for (int column = 1; column < Columns; column++)
                {
                    var child = Instantiate(CellSlotPrefab, transform, false);
                    child.localPosition = RectTransformHelper.GetChildPsotionContainer(child.rect, row, column, Offset);
                }
            }
            
        }
    }
}
