using System;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class ItemWielder : MonoBehaviour
    {
        public delegate void ItemReadyEvent(ItemStack stack);
        public delegate void ItemSwingEvent(ItemStack stack, Vector3 aimPoint);

        public event ItemReadyEvent OnReady;
        public event ItemSwingEvent OnSwingStart;
        public event ItemSwingEvent OnSwingPerform;
        public event ItemSwingEvent OnThrowPerform;

        internal ItemStack CurrentStack => HotbarItem.IsValid ? HotbarItem : fallbackStack;
        private ItemStack HotbarItem => hotbar ? hotbar.SelectedStack : default;

        public ItemStack HandStack => handStack;

        /// <summary>
        /// Vector from the item pivot to the mouse world point (not normalized).
        /// </summary>
        public Vector2 AimVector => aimVector;

        /// <summary>
        /// Aiming the item to throw, as opposed to for a melee swing.
        /// </summary>
        public bool IsThrowMode { get; private set; }

        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SaintsInterface<Component, IWielderBrain> brain;
        [SerializeField] Hotbar hotbar;
        [SerializeField, Required] SpriteRenderer itemRenderer;

        [Header("Config")]
        [SerializeField] ItemStack fallbackStack;

        // cached references
        private Transform itemPivot;
        private Transform itemVisual;

        // state
        private ItemStack handStack;
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

            if (hotbar)
                hotbar.OnChangeSelection += HandleHotbarSelectionChanged;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;

            if (hotbar)
                hotbar.OnChangeSelection -= HandleHotbarSelectionChanged;
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
            if (!handStack.IsValid || handStack.itemSO.IsNot(out UsableSO usableSO))
                return;

            ItemSwingConfig swingConfig = usableSO.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            // Free to melee swing OR start Throw Mode
            if (swingState == ItemSwingState.Ready)
            {
                RotateItemTowardsMouse();

                // we can use Throw Mode since we're not melee swinging
                float targetChargeAmount = throwChargeAmount + (Time.deltaTime * usableSO.ThrowChargeSpeed);
                throwChargeAmount = IsThrowMode ? Mathf.Clamp01(targetChargeAmount) : 0;
                IsThrowMode = brain.I.WantsToThrow;
            }
            else if (!phase.preventAim)
            {
                // prevent Throw Mode since we're mid-melee-swing
                throwChargeAmount = 0;
                IsThrowMode = false;

                RotateItemTowardsMouse();
            }

            // Throw the item
            if (IsThrowMode && brain.I.WantsToUse && timeSinceLastUse > usableSO.ThrowCooldown)
            {
                OnThrowPerform?.Invoke(HandStack, AimPointWorld);
                timeSinceLastUse = 0f;
            }

            // Early exit before the swing logic while on Throw Mode
            if (IsThrowMode)
                return;

            // We're not aiming, so we can do the swing logic now
            bool wantsToSwing = brain.I.WantsToUse && !wantsToSwapItems;

            switch (swingState)
            {
                case ItemSwingState.Ready:
                    if (wantsToSwing && timeSinceLastUse > usableSO.Cooldown)
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
                        OnSwingPerform?.Invoke(handStack, AimPointWorld);
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // hit if we need to before checking for final exit
                    if (phase.shouldHit)
                        OnSwingPerform?.Invoke(handStack, AimPointWorld);

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

            if (!handStack.IsValid || handStack.itemSO.IsNot(out UsableSO _))
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

                    OnReady?.Invoke(handStack);
                    break;
                case ItemSwingState.Swinging:
                    OnSwingStart?.Invoke(handStack, AimPointWorld);
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
            handStack = CurrentStack;
            UpdateItemSprite();

            phaseIndex = 0;
            ResetMotionStart();

            if (handStack.itemSO.Is(out UsableSO usableSO))
                SetSpriteTransformInstant(usableSO.SwingConfig.ReadyPosition, usableSO.SwingConfig.ReadyAngle);
        }

#region Motion Helpers

        private void SetMotionToPhase()
        {
            if (handStack.itemSO.IsNot(out UsableSO usableSO))
                return;

            ItemSwingConfig swingConfig = usableSO.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            ResetMotionStart();
            motion.EndPosition = swingConfig.ReadyPosition + phase.moveDelta;
            motion.EndAngle = swingConfig.ReadyAngle + phase.turnDelta;
            motion.MoveDuration = phase.moveDuration;
            motion.TurnDuration = phase.turnDuration;
        }

        private void SetMotionToReady()
        {
            if (!handStack.IsValid || handStack.itemSO.IsNot(out UsableSO usableSO))
                return;

            ItemSwingConfig swingConfig = usableSO.SwingConfig;

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

        private void SetSpriteTransformInstant(Vector2 targetPosition, float targetAngle)
        {
            itemVisual.localPosition = targetPosition;
            itemVisual.localEulerAngles = Vector3.forward * targetAngle;
        }

        private void RotateItemTowardsMouse()
        {
            if (!brain.I.AimPosition.HasValue)
            {
                itemPivot.localScale = Vector3.zero;
                return;
            }

            aimVector = brain.I.AimPosition.Value - (Vector2)itemPivot.position;
            float aimAngle = aimVector.ToAngle();
            bool isLeft = aimAngle is < -90 or > 90;

            itemPivot.localScale = Vector3.one.With(y: isLeft ? -1 : 1);
            itemPivot.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }

        private void UpdateItemSprite()
        {
            if (handStack.itemSO.IsNot(out UsableSO usableSO))
            {
                itemVisual.localScale = Vector3.zero;
                return;
            }

            itemVisual.localScale = Vector3.one * usableSO.IconScale;
            itemRenderer.sprite = usableSO ? usableSO.Icon : null;
        }

#region Event Handlers
        private void HandleDie(CombatPacket _) => itemRenderer.enabled = false;
        private void HandleRevived(Health reviver) => itemRenderer.enabled = true;

        private void HandleHotbarSelectionChanged(int _)
        {
            if (swingState != ItemSwingState.Ready)
            {
                wantsToSwapItems = true;
                return;
            }

            // Only update sprite when ready to swing again
            RefreshItem();
        }
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
