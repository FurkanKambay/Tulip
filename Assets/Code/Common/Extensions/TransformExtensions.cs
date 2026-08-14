using JetBrains.Annotations;
using UnityEngine;

namespace FK.Common.Extensions
{
    [PublicAPI]
    public static class TransformExtensions
    {
        public static void SetLocalPositionAndAngle(this Transform self, Vector2 position, float targetAngle) =>
            self.SetLocalPositionAndRotation(position, Quaternion.Euler(0, 0, targetAngle));

        public static void SetPositionAndAngle(this Transform self, Vector2 position, float targetAngle) =>
            self.SetPositionAndRotation(position, Quaternion.Euler(0, 0, targetAngle));

        public static void SetLocalAngle(this Transform self, float targetAngle) =>
            self.localRotation = Quaternion.Euler(0, 0, targetAngle);

        public static void SetAngle(this Transform self, float targetAngle) =>
            self.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
