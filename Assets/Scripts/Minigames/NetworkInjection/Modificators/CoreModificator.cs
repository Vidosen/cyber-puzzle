using Minigames.NetworkInjection.Data;

namespace Minigames.NetworkInjection.Modificators
{
    public class CoreModificator : INodeModificator
    {
        public NodeType NodeType => NodeType.Core;
        
        public void OnNodeHacked()
        {
        }
    }
}