using System;
using Minigames.MatrixBreaching.Matrix.Data;

namespace Utils
{
    public static class CoreExtensions
    {
        public static int GetEnumSize<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetNames(typeof(TEnum)).Length;
        }

        public static string ToTextString(this CellValueType cellValueType)
        {
            return ((int)cellValueType).ToString();
        }
    }
}