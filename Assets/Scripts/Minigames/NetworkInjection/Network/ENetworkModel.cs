using System;
using System.Collections.Generic;
using Minigames.NetworkInjection.Modificators;
using UniRx;
using UnityEngine;
using Utils.Inventory;
using Zenject;

namespace Minigames.NetworkInjection.Network
{
    public class ENetworkFactory : PlaceholderFactory<string, List<ENetworkModel.Node>, List<ENetworkModel.Connection> ,ENetworkModel>
    { }

    public class ENetworkModel : IModel<string>
    {
        public string Id { get; private set; }
        public float CummulativeInjectMultiplier { get; private set; }
        public float CummulativeMatrixBreachMultiplier { get; private set; }
        public IReadOnlyList<Node> Nodes => _nodes;
        public IReadOnlyCollection<Connection> Connections => _connections;
        private readonly List<Node> _nodes;
        private readonly HashSet<Connection> _connections;

        public ENetworkModel(string id, List<Node> nodes, List<Connection> connections)
        {
            Id = id;
            _nodes = nodes;
            _connections = new HashSet<Connection>(connections);
        }

        public class Node
        {
            public byte[] AccessCode { get; }
            public IObservable<float> ProgressUpdated => _progressSubject;
            public bool IsHacked => _hackPointer >= AccessCode.Length;
            public float HackProgress => (float) _hackPointer / AccessCode.Length;
            
            public INodeModificator Modificator { get; }

            private int _hackPointer;
            private Subject<float> _progressSubject = new Subject<float>();
            public void AddBreachingProgress(float progress)
            {
                var hackedCodes = Mathf.FloorToInt(AccessCode.Length * progress);
                _hackPointer = Mathf.Min(_hackPointer + hackedCodes, AccessCode.Length);
                _progressSubject.OnNext(HackProgress);
            }

            public bool GuessCode(byte gussedCode)
            {
                if (IsHacked)
                {
                    Debug.LogError("Node already hacked!");
                    return false;
                }

                if (AccessCode[_hackPointer] == gussedCode)
                {
                    _hackPointer++;
                    _progressSubject.OnNext(HackProgress);
                    return true;
                }
                return false;
            }
            
            public Node(INodeModificator modificator, byte[] accessCode)
            {
                Modificator = modificator;
                AccessCode = accessCode;
            }
        }

        public class Connection
        {
            public Node Node1 { get; }
            public Node Node2 { get; }

            public IObservable<float> ProgressUpdated => _progressSubject;

            public float InjectProgress => InjectTarget > 0 ? InjectCurrent / InjectTarget : 0;
            public bool IsInjected => InjectCurrent >= InjectTarget;
            public float InjectTarget { get; }
            public float InjectCurrent { get; private set; }

            private readonly int _hashCode;
            private Subject<float> _progressSubject = new Subject<float>();

            public Connection(Node node1, Node node2, float injectTarget)
            {
                Node1 = node1;
                Node2 = node2;
                InjectTarget = injectTarget;
                _hashCode = unchecked(node1.GetHashCode() + node2.GetHashCode());
            }

            public void InjectBytes(float injectedBytes)
            {
                InjectCurrent += injectedBytes;
                _progressSubject.OnNext(InjectProgress);
            }
            
            public bool ContainsNode(Node node)
            {
                return Node1 == node || Node2 == node;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != this.GetType())
                    return false;

                if (obj is Connection connection)
                    return this.Node1 == connection.Node1 &&
                           this.Node2 == connection.Node2;
                return false;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }

        public void AddInjectMultiplier(float upgradeValue)
        {
            CummulativeInjectMultiplier += upgradeValue;
        }
        public void AddBreachMultiplier(float upgradeValue)
        {
            CummulativeMatrixBreachMultiplier += upgradeValue;
        }
    }
}
