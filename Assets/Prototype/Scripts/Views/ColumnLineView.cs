using Prototype.Scripts.Views;
using UnityEngine;

namespace Prototype.Scripts
{
    class ColumnLineView : BaseLineView
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.right;
    }
}