using Vector2 = UnityEngine.Vector2;

namespace Matrix
{
    public class ColumnVector : BaseVector
    {
        public override Vector2 SnapDirection { get; protected set; } = Vector2.right;
    }
}