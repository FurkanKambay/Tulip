using JetBrains.Annotations;
using UnityEngine;

namespace Furkan.Common
{
    [PublicAPI]
    public static class LayerExtensions
    {
        public static bool Includes(this LayerMask mask, Transform transform) =>
            mask.Includes(transform.gameObject);

        public static bool Includes(this LayerMask mask, GameObject gameObject) =>
            (mask.value & (1 << gameObject.layer)) != 0;
    }
}
