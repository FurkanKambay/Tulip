using UnityEngine;

namespace Tulip.Data
{
    public interface IWielderBrain
    {
        public Vector2? AimPosition { get; }
        public bool WantsToAttack { get; }
        public bool WantsToTakeAim { get; }
        public bool WantsToHook { get; }
    }
}
