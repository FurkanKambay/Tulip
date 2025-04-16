using UnityEngine;

namespace Tulip.Data
{
    public interface IWielderBrain
    {
        public Vector2? AimPosition { get; }
        public bool WantsToUse { get; }
        public bool WantsToAim { get; }
        public bool WantsToHook { get; }
    }
}
