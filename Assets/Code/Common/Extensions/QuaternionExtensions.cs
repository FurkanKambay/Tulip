using JetBrains.Annotations;
using UnityEngine;

namespace FK.Common.Extensions
{
    [PublicAPI]
    public static class QuaternionExtensions
    {
        public static float ToAngle(this Vector2 direction) =>
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        public static Quaternion ToQuaternion2D(this Vector2 direction, Vector3 axis) =>
            Quaternion.AngleAxis(direction.ToAngle(), axis);

        public static Quaternion ToQuaternion2D(this Vector2 direction) =>
            Quaternion.AngleAxis(direction.ToAngle(), Vector3.forward);

        public static void LookAt(this Transform transform, Vector2 targetPoint) =>
            transform.rotation = ToQuaternion2D(targetPoint - (Vector2)transform.position);
    }
}
