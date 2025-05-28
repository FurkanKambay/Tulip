using JetBrains.Annotations;
using UnityEngine;

namespace Furkan.Common
{
    [PublicAPI]
    public static class VectorMathExtensions
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

        /// Exponential decay function from Freya Holmér.
        public static float ExpDecay(this float from, float to, float decay, float deltaTime) =>
            to + ((from - to) * Mathf.Exp(-decay * deltaTime));

        /// Exponential decay function from Freya Holmér.
        public static Vector2 ExpDecay(this Vector2 from, Vector2 to, float decay, float deltaTime) =>
            to + ((from - to) * Mathf.Exp(-decay * deltaTime));

        /// Exponential decay function from Freya Holmér.
        public static Vector3 ExpDecay(this Vector3 from, Vector3 to, float decay, float deltaTime) =>
            to + ((from - to) * Mathf.Exp(-decay * deltaTime));
    }
}
