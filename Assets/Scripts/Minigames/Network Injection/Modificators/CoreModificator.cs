using Minigames.Network_Injection.Data;

namespace Minigames.Network_Injection.Modificators
{
    public class CoreModificator : INodeModificator
    {
        public NodeType NodeType => NodeType.Core;
        
        public void OnNodeHacked()
        {
        }
    }
}