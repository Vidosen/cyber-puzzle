using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.Network_Injection.Configs;
using Minigames.Network_Injection.Data;
using Minigames.Network_Injection.Modificators;
using UnityEditor;
using UnityEngine;
using Utils.Inventory;
using Zenject;

namespace Minigames.Network_Injection.Network
{
    public class ENetworkInventory : BaseInventory<ENetworkModel, string>, IInitializable
    {
        private readonly ENetworkFactory _eNetworkFactory;
        private readonly ENetworkSetupConfig _eNetworkSetupConfig;

        public ENetworkInventory(ENetworkFactory eNetworkFactory, ENetworkSetupConfig eNetworkSetupConfig)
        {
            _eNetworkFactory = eNetworkFactory;
            _eNetworkSetupConfig = eNetworkSetupConfig;
        }
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public ENetworkModel CreateModelFromSetup(string configId)
        {
            var foundSetup = _eNetworkSetupConfig.ENetworkSetups.FirstOrDefault(setup => setup.Id == configId);
            if (foundSetup == null)
            {
                Debug.LogError($"Didn't find E-Network Setup with ID '{configId}'!");
                return null;
            }

            var nodes = BuildNodes(foundSetup.NodesData);
            var connections = BuildConnections(nodes, foundSetup.ConnectionsData);
            
            var modelId = configId + '_' + GUID.Generate().ToString();
            var model = _eNetworkFactory.Create(modelId, nodes.Values.ToList(), connections);
            _models.Add(model);
            return model;
        }

        private List<ENetworkModel.Connection> BuildConnections(Dictionary<int, ENetworkModel.Node> foundSetupNodesData,
            List<ENetworkConnectionData> foundSetupConnectionsData)
        {
            var connectionList = new List<ENetworkModel.Connection>();
            foreach (var connectionData in foundSetupConnectionsData)
            {
                if (foundSetupNodesData.TryGetValue(connectionData.Node1_DataId, out var node1) &&
                    foundSetupNodesData.TryGetValue(connectionData.Node2_DataId, out var node2))
                    connectionList.Add(new ENetworkModel.Connection(node1, node2, connectionData.InjectTarget));
            }
            return connectionList;
        }

        private Dictionary<int, ENetworkModel.Node> BuildNodes(List<ENetworkNodeData> foundSetupNodesData)
        {
            Dictionary<int, ENetworkModel.Node> dictionary = new Dictionary<int, ENetworkModel.Node>();
            foreach (var nodeData in foundSetupNodesData)
            {
                INodeModificator modificator;
                switch (nodeData.NodeType)
                {
                    case NodeType.Upgrade:
                        modificator = new UpgradeModificator(nodeData.UpgradeType, nodeData.UpgradeValue);
                        break;
                    case NodeType.Firewall:
                        modificator = new FirewallModificator();
                        break;
                    case NodeType.Core:
                        modificator = new CoreModificator();
                        break; 
                    default:
                        modificator = new EmptyModificator();
                        break;
                }
                dictionary.Add(nodeData.DataId, new ENetworkModel.Node(modificator, nodeData.AccessCode));
            }
            return dictionary;
        }

    }
}