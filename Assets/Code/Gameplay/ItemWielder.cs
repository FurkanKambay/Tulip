using System;
using Furkan.Common;
using Furkan.Common.Extensions;
using Tulip.Combat;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Input;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class ItemWielder : MonoBehaviour
    {
        public delegate void ItemReadyEvent(ItemAsset item);
        public delegate void ItemSwingEvent(ItemAsset item, Vector3 aimPoint);

        public event ItemReadyEvent OnReady;
        public event ItemSwingEvent OnSwingStart;
        public event ItemSwingEvent OnSwingPerform;
        public event ItemSwingEvent OnThrowPerform;

        /// <summary>
        /// Vector from the item pivot to the mouse world point (not normalized).
        /// </summary>
        public Vector2 AimVector => aimVector;

        /// <summary>
        /// Aiming the item to throw, as opposed to for a melee swing.
        /// </summary>
        public bool IsThrowMode { get; private set; }

        [Header("Brain")]
        [SerializeField, Required] CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SpriteRenderer itemRenderer;

        [Header("Config")]
        [SerializeField] ItemAsset equippedItem;

        // cached references
        private Transform itemPivot;
        private Transform itemVisual;

        // state
        private float timeSinceLastUse;
        private ItemSwingState swingState;
        private Vector2 aimVector;
        private float throwChargeAmount;

        // state: phase (motion)
        private bool wantsToSwapItems;
        private int phaseIndex;
        private MotionState motion;

        private Vector3 AimPointWorld => itemPivot.position + (Vector3)aimVector;

#region Unity Lifecycle
        private void Awake()
        {
            itemVisual = itemRenderer.transform;
            itemPivot = itemVisual.parent;
        }

        private void OnEnable()
        {
            UpdateItemSprite();

            health.OnDie += HandleDie;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;
        }

        private void Start() => RefreshItem();

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            if (GameManager.CurrentState == GameState.Paused)
                return;

            TickSwingState();
        }
#endregion

        private void TickSwingState()
        {
            if (equippedItem.IsNot(out UsableAsset usableAsset))
                return;

            ItemSwingConfig swingConfig = usableAsset.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            // Free to melee swing OR start Throw Mode
            if (swingState == ItemSwingState.Ready)
            {
                RotateItemTowardsMouse();

                // we can use Throw Mode since we're not melee swinging
                float targetChargeAmount = throwChargeAmount + (Time.deltaTime * usableAsset.ThrowChargeSpeed);
                throwChargeAmount = IsThrowMode ? Mathf.Clamp01(targetChargeAmount) : 0;
                IsThrowMode = brain.WantsToTakeAim;
            }
            else if (!phase.preventAim)
            {
                // prevent Throw Mode since we're mid-melee-swing
                throwChargeAmount = 0;
                IsThrowMode = false;

                RotateItemTowardsMouse();
            }

            // Throw the item
            if (IsThrowMode && brain.WantsToAttack && timeSinceLastUse > usableAsset.ThrowCooldown)
            {
                OnThrowPerform?.Invoke(equippedItem, AimPointWorld);
                timeSinceLastUse = 0f;
            }

            // Early exit before the swing logic while on Throw Mode
            if (IsThrowMode)
                return;

            // We're not aiming, so we can do the swing logic now
            bool wantsToSwing = brain.WantsToAttack && !wantsToSwapItems;

            switch (swingState)
            {
                case ItemSwingState.Ready:
                    if (wantsToSwing && timeSinceLastUse > usableAsset.Cooldown)
                    {
                        SwitchState(ItemSwingState.Swinging);
                        timeSinceLastUse = 0f;
                    }

                    break;
                case ItemSwingState.Swinging:
                    // cancel the swing if needed
                    if (phase.isCancelable && !wantsToSwing)
                    {
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // proceed normally (not interrupting the motion)
                    TickMotionLerp();

                    // we're still Lerping, so we skip to the next tick
                    if (!IsMotionDone())
                        break;

                    // we reached the target angle. move to next phase or reset after final phase

                    // if no phases, hit and reset swing
                    if (swingConfig.Phases.Length == 0)
                    {
                        OnSwingPerform?.Invoke(equippedItem, AimPointWorld);
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // hit if we need to before checking for final exit
                    if (phase.shouldHit)
                        OnSwingPerform?.Invoke(equippedItem, AimPointWorld);

                    bool isFinalPhase = phaseIndex == swingConfig.Phases.Length - 1;
                    bool shouldReset = !wantsToSwing || !swingConfig.Loop;

                    if (isFinalPhase && shouldReset)
                    {
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // still not ending so next phase. keeps swinging without resetting
                    // looping: start from phase 0 again

                    // "reset" to phase 0 with `phase.XDuration`, NOT `swingType.ResetXDuration`
                    phaseIndex = isFinalPhase ? 0 : phaseIndex + 1;

                    // this belongs in a state machine. Motion is a sub-state machine of Swing
                    SetMotionToPhase();

                    break;
                case ItemSwingState.Resetting:
                    TickMotionLerp();

                    if (IsMotionDone())
                        SwitchState(ItemSwingState.Ready);

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(swingState));
            }
        }

        private void SwitchState(ItemSwingState state)
        {
            if (state == swingState)
                return;

            if (equippedItem.IsNot(out UsableAsset _))
            {
                swingState = ItemSwingState.Ready;
                return;
            }

            swingState = state;

            switch (state)
            {
                case ItemSwingState.Ready:
                    // Only swap items when reset and ready
                    wantsToSwapItems = false;
                    RefreshItem();

                    OnReady?.Invoke(equippedItem);
                    break;
                case ItemSwingState.Swinging:
                    OnSwingStart?.Invoke(equippedItem, AimPointWorld);
                    phaseIndex = 0;
                    SetMotionToPhase();
                    break;
                case ItemSwingState.Resetting:
                    SetMotionToReady();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state));
            }
        }

        private void RefreshItem()
        {
            UpdateItemSprite();

            phaseIndex = 0;
            ResetMotionStart();

            if (equippedItem.Is(out UsableAsset usableAsset))
                SetSpriteTransformInstant(usableAsset.SwingConfig.ReadyPosition, usableAsset.SwingConfig.ReadyAngle);
        }

#region Motion Helpers
        private void SetMotionToPhase()
        {
            if (equippedItem.IsNot(out UsableAsset usableAsset))
                return;

            ItemSwingConfig swingConfig = usableAsset.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            ResetMotionStart();
            motion.EndPosition = swingConfig.ReadyPosition + phase.moveDelta;
            motion.EndAngle = swingConfig.ReadyAngle + phase.turnDelta;
            motion.MoveDuration = phase.moveDuration;
            motion.TurnDuration = phase.turnDuration;
        }

        private void SetMotionToReady()
        {
            if (equippedItem.IsNot(out UsableAsset usableAsset))
                return;

            ItemSwingConfig swingConfig = usableAsset.SwingConfig;

            ResetMotionStart();
            motion.EndPosition = swingConfig.ReadyPosition;
            motion.EndAngle = swingConfig.ReadyAngle;
            motion.MoveDuration = swingConfig.ResetMoveDuration;
            motion.TurnDuration = swingConfig.ResetTurnDuration;
        }

        private void ResetMotionStart()
        {
            motion = default;
            motion.StartPosition = itemVisual.localPosition;
            motion.StartAngle = itemVisual.localEulerAngles.z;
            // need to reset lerp values too here
            motion.LerpMove = 0;
            motion.LerpTurn = 0;
        }

        private void TickMotionLerp()
        {
            motion.LerpMove = motion.MoveDuration <= 0 || motion.LerpMove >= 1 ? 1
                : Mathf.MoveTowards(motion.LerpMove, 1, Time.deltaTime / motion.MoveDuration);

            motion.LerpTurn = motion.TurnDuration <= 0 || motion.LerpTurn >= 1 ? 1
                : Mathf.MoveTowards(motion.LerpTurn, 1, Time.deltaTime / motion.TurnDuration);

            SetSpriteTransformInstant(
                Vector2.Lerp(motion.StartPosition, motion.EndPosition, motion.LerpMove),
                Mathf.LerpAngle(motion.StartAngle, motion.EndAngle, motion.LerpTurn)
            );
        }

        private bool IsMotionDone() =>
            Mathf.Approximately(motion.LerpMove, 1) && Mathf.Approximately(motion.LerpTurn, 1);
#endregion

        private void SetSpriteTransformInstant(Vector2 targetPosition, float targetAngle) =>
            itemVisual.SetLocalPositionAndRotation(targetPosition, Quaternion.Euler(0, 0, targetAngle));

        private void RotateItemTowardsMouse()
        {
            if (!brain.AimPosition.HasValue)
            {
                itemPivot.localScale = Vector3.zero;
                return;
            }

            aimVector = brain.AimPosition.Value - (Vector2)itemPivot.position;
            float aimAngle = aimVector.ToAngle();
            bool isLeft = aimAngle is < -90 or > 90;

            itemPivot.localScale = Vector3.one.With(y: isLeft ? -1 : 1);
            itemPivot.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }

        private void UpdateItemSprite()
        {
            if (equippedItem.IsNot(out UsableAsset usableAsset))
            {
                itemVisual.localScale = Vector3.zero;
                return;
            }

            itemVisual.localScale = Vector3.one * usableAsset.IconScale;
            itemRenderer.sprite = usableAsset ? usableAsset.Icon : null;
        }

#region Event Handlers
        private void HandleDie(CombatPacket _) => itemRenderer.enabled = false;
        private void HandleRevived(Health reviver) => itemRenderer.enabled = true;
#endregion

#region Child Structs
        private struct MotionState
        {
            public Vector2 StartPosition;
            public Vector2 EndPosition;
            public float StartAngle;
            public float EndAngle;

            public float MoveDuration;
            public float TurnDuration;
            public float LerpMove;
            public float LerpTurn;
        }

        private enum ItemSwingState
        {
            Ready,
            Swinging,
            Resetting
        }
#endregion
    }
}
