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
        public static TEnum GetRandomEnum<TEnum>(Random random) where TEnum : struct, Enum
        {
            var enumIndex = random.Next(0, CoreExtensions.GetEnumSize<TEnum>());
            return (TEnum) Enum.Parse(typeof(TEnum), Enum.GetNames(typeof(TEnum))[enumIndex]);
        }
        public static TEnum GetRandomEnum<TEnum>() where TEnum : struct, Enum
        {
            return GetRandomEnum<TEnum>(new Random());
        }

        public static string ToTextString(this CellValueType cellValueType)
        {
            return ((int)cellValueType).ToString();
        }
    }
}