using System;
using UnityEngine;

namespace FK.Tulip.Input
{
    [DefaultExecutionOrder(-10)]
    public abstract class CharacterBrain : MonoBehaviour
    {
        public virtual event Action OnJump;
        public virtual event Action OnJumpReleased;

        public virtual float HorizontalMovement { get; protected set; }
        public virtual float VerticalMovement { get; protected set; }
        public virtual bool WantsToJump { get; protected set; }

        public virtual Vector2 AimPointScreen { get; protected set; }
        public virtual Vector2? AimPosition { get; protected set; }
        public virtual bool WantsToAttack { get; protected set; }
        public virtual bool WantsToTakeAim { get; protected set; }

        public virtual bool WantsToDash { get; protected set; }
        public virtual bool WantsToHook { get; protected set; }
    }
}
