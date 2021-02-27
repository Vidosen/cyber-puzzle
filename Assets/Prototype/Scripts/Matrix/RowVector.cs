using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class RowVector : BaseVector
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.up;
    }
}