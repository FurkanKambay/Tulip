using System;
using FK.Common;
using FK.Tulip.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FK.Tulip.Input
{
    [DefaultExecutionOrder(-10)]
    public sealed class PlayerBrain : CharacterBrain
    {
        public override event Action OnJump;
        public override event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Input - Basic")]
        [SerializeField, Required] InputActionReference point;
        [SerializeField, Required] InputActionReference move;
        [SerializeField, Required] InputActionReference jump;
        [SerializeField, Required] InputActionReference dash;
        [SerializeField, Required] InputActionReference use;
        [SerializeField, Required] InputActionReference aim;
        [SerializeField, Required] InputActionReference hook;

        public override Vector2 AimPointScreen { get; protected set; }
        public override Vector2? AimPosition { get; protected set; }
        public override float HorizontalMovement { get; protected set; }

        public override bool WantsToJump { get; protected set; }
        public override bool WantsToDash { get; protected set; }
        public override bool WantsToAttack { get; protected set; }
        public override bool WantsToTakeAim { get; protected set; }
        public override bool WantsToHook { get; protected set; }

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        private void Update()
        {
            if (!health || health.IsDead)
            {
                AimPosition = default;

                HorizontalMovement = default;
                WantsToDash = false;
                WantsToAttack = false;

                OnJumpReleased?.Invoke();
                return;
            }

            if (Time.timeScale > 0)
            {
                AimPointScreen = point.action.ReadValue<Vector2>();
                AimPosition = mainCamera.ScreenToWorldPoint(AimPointScreen);
            }

            HorizontalMovement = move.action.ReadValue<float>();

            if (jump.action.triggered)
                OnJump?.Invoke();
            else if (jump.action.WasReleasedThisFrame())
                OnJumpReleased?.Invoke();

            WantsToJump = jump.action.inProgress;
            WantsToDash = dash.action.inProgress;
            WantsToAttack = use.action.inProgress;
            WantsToTakeAim = aim.action.inProgress;
            WantsToHook = hook.action.triggered;
        }
    }
}
