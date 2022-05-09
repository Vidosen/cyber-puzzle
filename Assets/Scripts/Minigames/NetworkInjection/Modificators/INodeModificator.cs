using Minigames.NetworkInjection.Data;

namespace Minigames.NetworkInjection.Modificators
{
    public interface INodeModificator
    {
        NodeType NodeType { get; }
        void OnNodeHacked();
    }
}