using System;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
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

        [Header("Input - Misc")]
        [SerializeField, Required] InputActionReference hotbarScroll;
        [SerializeField, Required] InputActionReference hotbar;

        public Vector2 AimPointScreen { get; private set; }
        public Vector2? AimPosition { get; private set; }
        public float HorizontalMovement { get; private set; }

        public bool WantsToJump { get; private set; }
        public bool WantsToDash { get; private set; }
        public bool WantsToUse { get; private set; }
        public bool WantsToThrow { get; private set; }
        public bool WantsToHook { get; private set; }

        public int HotbarSelectionDelta { get; private set; }
        public int? HotbarSelectionIndex { get; private set; }

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        private void Update()
        {
            if (!health || health.IsDead)
            {
                AimPosition = default;

                HorizontalMovement = default;
                WantsToDash = false;
                WantsToUse = false;

                HotbarSelectionDelta = 0;
                HotbarSelectionIndex = null;

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
            WantsToUse = use.action.inProgress;
            WantsToThrow = aim.action.inProgress;
            WantsToHook = hook.action.triggered;

            HotbarSelectionDelta = Math.Sign(hotbarScroll.action.ReadValue<float>());
            HotbarSelectionIndex = !hotbar.action.inProgress ? null : (int)hotbar.action.ReadValue<float>();
        }
    }
}
