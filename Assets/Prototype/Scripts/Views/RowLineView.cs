using UnityEngine;

namespace Prototype.Scripts
{
    class RowLineView : BaseLineView
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.up;
    }
}