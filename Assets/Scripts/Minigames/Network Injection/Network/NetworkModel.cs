
using System;
using UniRx;
using UnityEngine;

namespace Minigames.Network_Injection.Network
{
    public class NetworkModel
    {
        public float CummulativeInjectMultiplier { get; private set; }
        public float CummulativeMatrixBreachMultiplier { get; private set; }
        public class Node
        {
            public byte[] AccessCode { get; }
            public IObservable<Unit> ProgressUpdated => _hackPointer.AsUnitObservable();
            public bool IsHacked => _hackPointer.Value >= AccessCode.Length;
            public float HackProgress => (float)_hackPointer.Value / AccessCode.Length;

            private ReactiveProperty<int> _hackPointer = new ReactiveProperty<int>();
            public void AddBreachingProgress(float progress)
            {
                var hackedCodes = Mathf.FloorToInt(AccessCode.Length * progress);
                _hackPointer.Value = Mathf.Min(_hackPointer.Value + hackedCodes, AccessCode.Length);
            }

            public bool GuessCode(byte gussedCode)
            {
                if (IsHacked)
                {
                    Debug.LogError("Node already hacked!");
                    return false;
                }

                if (AccessCode[_hackPointer.Value] == gussedCode)
                {
                    _hackPointer.Value++;
                    return true;
                }
                return false;
            }
            
            public Node(byte[] accessCode)
            {
                AccessCode = accessCode;
            }
        }

        public class Connection
        {
            public Node Node1 { get; }
            public Node Node2 { get; }
            
            private readonly int _hashCode;
            
            public Connection(Node node1, Node node2)
            {
                Node1 = node1;
                Node2 = node2;
                _hashCode = unchecked(node1.GetHashCode() + node2.GetHashCode());
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
