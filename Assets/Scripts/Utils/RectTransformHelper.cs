using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class RectTransformHelper
    {
        public static Vector2 GetGridContainer(Vector2 childSize, int rows, int columns, float childrenOffset)
        {
            return new Vector2(columns * childSize.y + (columns + 1) * childrenOffset,
                rows * childSize.x + (rows + 1) * childrenOffset);
        }
        
        public static float GetGridContainerRatio(RectTransform holder, Vector2 gridSize, float maxRatio = Single.PositiveInfinity)
        {
            var holderBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(holder);
            return Mathf.Min(holderBounds.size.x / gridSize.x, holderBounds.size.y /  gridSize.y, maxRatio);
        }

        public static Vector3 GetChildPositionContainer(Rect childRect, int xIndex, int yIndex, float childrenOffset)
        {
            Vector3 resultPosition = Vector3.zero;
            resultPosition.x = childrenOffset * (xIndex + 1) + childRect.width * xIndex + childRect.width * 0.5f;
            resultPosition.y = -childrenOffset * (yIndex + 1) - childRect.height * yIndex - childRect.height * 0.5f;
            return resultPosition;
        }

        public static Color AverageColor(this List<Color> colorList)
        {
            float h, s, v;
            float resH = 0, resS = 0, resV = 0;
            foreach (var color in colorList)
            {
                Color.RGBToHSV(color,out h, out s,out v);
                resH += h;
                resS += s;
                resV += v;
            }

            var count = colorList.Count;
            return Color.HSVToRGB(resH / count, resS / count, resV / count);
        }
        
        public static void MultiplyScale(this MonoBehaviour obj, float scale)
        {
            var _transform = obj.GetComponent<RectTransform>();
            if (scale > 0 && _transform != null)
            {
                _transform.sizeDelta = _transform.sizeDelta * scale;   
            }
        }
    }
}
