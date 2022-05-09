using Minigames.NetworkInjection.Data;

namespace Minigames.NetworkInjection.Modificators
{
    public class UpgradeModificator : INodeModificator
    {
        public NodeType NodeType => NodeType.Upgrade;
        public float Value { get; }
        public  UpgradeType UpgradeType { get; }
        public UpgradeModificator(UpgradeType upgradeType, float upgradeValue)
        {
            UpgradeType = upgradeType;
            Value = upgradeValue;
        }

        public void OnNodeHacked()
        {
            
        }
    }
}