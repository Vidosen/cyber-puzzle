using Minigames.NetworkInjection.Data;

namespace Minigames.NetworkInjection.Modificators
{
    public class FirewallModificator : INodeModificator
    {

        public NodeType NodeType => NodeType.Firewall;
        
        public void OnNodeHacked()
        {
        }
    }
}