using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Utils
{
    public static class CoreExtensions
    {
        public static int GetEnumSize<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetNames(typeof(TEnum)).Length;
        }
        public static TEnum GetRandomEnum<TEnum>(this Random random) where TEnum : struct, Enum
        {
            var enumIndex = random.Next(0, CoreExtensions.GetEnumSize<TEnum>());
            return (TEnum) Enum.GetValues(typeof(TEnum)).GetValue(enumIndex);
        }
        
        public static TItem GetRandomItem<TItem>(this Random random, Dictionary<TItem, int> weights)
        {
            var sumWeights = weights.Values.Sum();
            var compareValue = random.Next(0, sumWeights);
            foreach (var pair in weights)
            {
                if (compareValue < pair.Value)
                {
                    return pair.Key;
                }
                else
                {
                    compareValue -= pair.Value;
                }
            }
            return default;
        }
        public static IList<TConcreteCell> GetCells<TConcreteCell>(this List<ICell> cells) where TConcreteCell : class, ICell
        {
            return cells.Select(cell => cell as TConcreteCell).Where(cell => cell != null).ToList();
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