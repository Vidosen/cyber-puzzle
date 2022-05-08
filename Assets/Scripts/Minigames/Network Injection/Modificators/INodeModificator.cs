using Minigames.Network_Injection.Data;

namespace Minigames.Network_Injection.Modificators
{
    public interface INodeModificator
    {
        NodeType NodeType { get; }
        void OnNodeHacked();
    }
}