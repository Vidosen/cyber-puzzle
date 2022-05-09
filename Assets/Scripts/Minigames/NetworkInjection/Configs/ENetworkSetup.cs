using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.NetworkInjection.Data;
using UnityEngine;

namespace Minigames.NetworkInjection.Configs
{
    [Serializable]
    public class ENetworkNodeData
    {
        public int DataId;
        public NodeType NodeType;
        public UpgradeType UpgradeType;
        public float UpgradeValue;
        public byte[] AccessCode;
    }
    
    [Serializable]
    public class ENetworkConnectionData
    {
        public int Node1_DataId;
        public int Node2_DataId;
        public float InjectTarget;
    }
    
    [CreateAssetMenu(fileName = "ENetworkSetup", menuName = "New E-Network Preset", order = 0)]
    public class ENetworkSetup : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private List<ENetworkNodeData> nodesData;
        [SerializeField] private List<ENetworkConnectionData> connectionsData;
        
        public string Id => id;
        public List<ENetworkNodeData> NodesData => nodesData.ToList();
        public List<ENetworkConnectionData> ConnectionsData => connectionsData.ToList();
    }
}