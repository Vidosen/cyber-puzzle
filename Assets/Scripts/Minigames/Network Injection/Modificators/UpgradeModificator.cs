using Minigames.Network_Injection.Data;
using Minigames.Network_Injection.Network;

namespace Minigames.Network_Injection.Modificators
{
    public class InjectUpgradeModificator : INodeModificator
    {
        private readonly NetworkModel _networkModel;
        public float Value { get; }

        public InjectUpgradeModificator(float upgradeValue, NetworkModel networkModel)
        {
            _networkModel = networkModel;
            Value = upgradeValue;
        }
        public NodeType NodeType => NodeType.Upgrade;
        
        public void OnNodeHacked()
        {
            _networkModel.AddInjectMultiplier(Value);
        }
    }
    
    public class BreachUpgradeModificator : INodeModificator
    {
        private readonly NetworkModel _networkModel;
        public float Value { get; }

        public BreachUpgradeModificator(float upgradeValue, NetworkModel networkModel)
        {
            _networkModel = networkModel;
            Value = upgradeValue;
        }
        public NodeType NodeType => NodeType.Upgrade;
        
        public void OnNodeHacked()
        {
            _networkModel.AddBreachMultiplier(Value);
        }
    }
    
    public class FirewallModificator : INodeModificator
    {
        private readonly NetworkModel _networkModel;
        public float Value => 0;

        public FirewallModificator(NetworkModel networkModel)
        {
            _networkModel = networkModel;
        }

        public NodeType NodeType => NodeType.Firewall;
        
        public void OnNodeHacked()
        {
        }
    }

    public interface INodeModificator
    {
        float Value { get; }
        NodeType NodeType { get; }
        void OnNodeHacked();
    }
}