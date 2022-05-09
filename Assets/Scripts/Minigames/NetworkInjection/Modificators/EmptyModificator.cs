using Minigames.NetworkInjection.Data;

namespace Minigames.NetworkInjection.Modificators
{
    public class EmptyModificator : INodeModificator
    {
        public NodeType NodeType => NodeType.Empty;
        
        public void OnNodeHacked()
        { }
    }
}