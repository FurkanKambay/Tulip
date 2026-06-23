using System;
using Furkan.Common;
using Tulip.Combat;
using Tulip.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    [DefaultExecutionOrder(-10)]
    public sealed class PlayerBrain : MonoBehaviour, IPlayerBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

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

        public Vector2 AimPointScreen { get; private set; }
        public Vector2? AimPosition { get; private set; }
        public float HorizontalMovement { get; private set; }

        public bool WantsToJump { get; private set; }
        public bool WantsToDash { get; private set; }
        public bool WantsToAttack { get; private set; }
        public bool WantsToTakeAim { get; private set; }
        public bool WantsToHook { get; private set; }

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
