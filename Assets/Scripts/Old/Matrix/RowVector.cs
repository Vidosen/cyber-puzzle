using UnityEngine;

namespace Matrix
{
    public class RowVector : BaseVector
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.up;
    }
}