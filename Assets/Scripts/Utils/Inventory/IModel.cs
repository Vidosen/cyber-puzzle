using System;

namespace Utils.Inventory
{
    public interface IModel<TIndex> where TIndex : IComparable<TIndex>
    {
        TIndex Id { get; }
    }
}