using System.Collections.Generic;
using UnityEngine;

namespace Minigames.NetworkInjection.Configs
{
    [CreateAssetMenu(fileName = "ENetworkSetupConfig", menuName = "Configs/Network Injection/E-Network Setup Config", order = 0)]
    public class ENetworkSetupConfig : ScriptableObject
    {
        [SerializeField] private List<ENetworkSetup> eNetworkSetups;
        public List<ENetworkSetup> ENetworkSetups => eNetworkSetups;
    }
}