using Minigames.Network_Injection.Data;

namespace Minigames.Network_Injection.Modificators
{
    public class EmptyModificator : INodeModificator
    {
        public NodeType NodeType => NodeType.Empty;
        
        public void OnNodeHacked()
        { }
    }
}