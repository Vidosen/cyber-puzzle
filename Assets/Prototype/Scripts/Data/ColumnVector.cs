using System.Numerics;
using Vector2 = UnityEngine.Vector2;

namespace Prototype.Scripts.Data
{
    public class ColumnVector : BaseVector
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.right;
    }
}