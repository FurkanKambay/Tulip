using JetBrains.Annotations;
using UnityEngine;

namespace FK.Common.Extensions
{
    [PublicAPI]
    public static class VectorExtensions
    {
        public static Vector3 WithZ(this Vector2 self, float z) =>
            new(self.x, self.y, z);

        public static Vector2 With(this Vector2 self, float? x = null, float? y = null) =>
            new(x ?? self.x, y ?? self.y);

        public static Vector3 With(this Vector3 self, float? x = null, float? y = null, float? z = null) =>
            new(x ?? self.x, y ?? self.y, z ?? self.z);

        public static Vector3Int WithZ(this Vector2Int self, int z) =>
            new(self.x, self.y, z);

        public static Vector2Int With(this Vector2Int self, int? x = null, int? y = null) =>
            new(x ?? self.x, y ?? self.y);
    }
}
