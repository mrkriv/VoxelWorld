using OpenTK;

namespace GameCore.Services.Extensions
{
    public static class VectorExtensions
    {
        public static string ToStringShort(this Vector3 vec)
        {
            return $"{vec.X:0.0} {vec.Y:0.0} {vec.Z:0.0}";
        }
    }
}