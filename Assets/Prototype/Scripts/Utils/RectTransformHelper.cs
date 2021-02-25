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
        
    }
}
