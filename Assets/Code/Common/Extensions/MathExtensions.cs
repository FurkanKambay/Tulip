using JetBrains.Annotations;
using UnityEngine;

namespace Furkan.Common.Extensions
{
    [PublicAPI]
    public static class MathExtensions
    {
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
