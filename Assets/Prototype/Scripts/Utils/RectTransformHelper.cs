using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Utils
{
    public static class RectTransformHelper
    {
        public static Vector2 GetGridContainer(Rect childRect, int rows, int columns, float childrenOffset)
        {
            return new Vector2(columns * childRect.height + (columns + 1) * childrenOffset,
                rows * childRect.width + (rows + 1) * childrenOffset);
        }

        public static Vector2 GetChildPositionContainer(Rect childRect, int xIndex, int yIndex, float childrenOffset)
        {
            Vector2 resultPosition = Vector2.zero;
            resultPosition.x = childrenOffset * (xIndex + 1) + childRect.width * xIndex;
            resultPosition.y = -childrenOffset * (yIndex + 1) - childRect.height * yIndex;
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
        
    }
}
