using Minigames.Network_Injection.Data;

namespace Minigames.Network_Injection.Modificators
{
    public class FirewallModificator : INodeModificator
    {

        public NodeType NodeType => NodeType.Firewall;
        
        public void OnNodeHacked()
        {
        }
    }
}