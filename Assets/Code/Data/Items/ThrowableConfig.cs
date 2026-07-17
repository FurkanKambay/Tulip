using System;
using UnityEngine;

namespace FK.Tulip.Data.Items
{
    [Serializable]
    public struct ThrowableConfig
    {
        [Tooltip("Throw cooldown in seconds.")]
        [Min(0)] public float cooldown;

        [Min(0)] public float strength;
        [Min(0)] public float chargeSpeed;
    }
}
