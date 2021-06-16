using Prototype.Scripts.Data;
using UnityEngine;

namespace Prototype.Scripts.Matrix
{
    public class RowVector : BaseVector
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.up;
    }
}