using System;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class ItemWielder : MonoBehaviour
    {
        public delegate void ItemReadyEvent(ItemStack stack);
        public delegate void ItemSwingEvent(ItemStack stack, Vector3 aimPoint);

        public event ItemReadyEvent OnReady;
        public event ItemSwingEvent OnSwingStart;
        public event ItemSwingEvent OnSwingPerform;

        public ItemStack CurrentStack => HotbarItem.IsValid ? HotbarItem : fallbackStack;
        private ItemStack HotbarItem => hotbar ? hotbar.SelectedStack : default;

        public Vector2 AimDirection => lastAimDirection;

        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SaintsInterface<Component, IWielderBrain> brain;
        [SerializeField] Hotbar hotbar;
        [SerializeField, Required] SpriteRenderer itemRenderer;

        [Header("Config")]
        [SerializeField] ItemStack fallbackStack;

        private Transform itemPivot;
        private Transform itemVisual;

        // state
        private ItemStack handStack;
        private float timeSinceLastUse;
        private ItemSwingState swingState;
        private Vector3 rendererScale;
        private Vector2 lastAimDirection;

        // state: phase (motion)
        private bool wantsToSwapItems;
        private int phaseIndex;
        private MotionState motion;

        private Vector3 AimPointWorld => itemPivot.position + (Vector3)lastAimDirection;

        private void Awake()
        {
            itemVisual = itemRenderer.transform;
            itemPivot = itemVisual.parent;
        }

        private void Start() => RefreshItem();

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            if (Time.deltaTime > 0)
                TickSwingState();
        }

        private void TickSwingState()
        {
            if (!handStack.IsValid || handStack.itemData.IsNot(out UsableData usableData))
                return;

            bool wantsToUse = brain.I.WantsToUse && !wantsToSwapItems;
            ItemSwingConfig swingConfig = usableData!.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            if (!phase.preventAim || swingState == ItemSwingState.Ready)
                AimItem();

            switch (swingState)
            {
                case ItemSwingState.Ready:
                    if (wantsToUse && timeSinceLastUse > usableData.Cooldown)
                    {
                        SwitchState(ItemSwingState.Swinging);
                        timeSinceLastUse = 0f;
                    }

                    break;
                case ItemSwingState.Swinging:
                    // cancel the swing if needed
                    if (phase.isCancelable && !wantsToUse)
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
                    bool shouldReset = !wantsToUse || !swingConfig.Loop;

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

            if (!handStack.IsValid || handStack.itemData.IsNot(out UsableData _))
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

            if (handStack.itemData.Is(out UsableData usableData))
                SetSpriteTransformInstant(usableData!.SwingConfig.ReadyPosition, usableData.SwingConfig.ReadyAngle);
        }

#region Motion Helpers

        private void SetMotionToPhase()
        {
            if (handStack.itemData.IsNot(out UsableData usableData))
                return;

            ItemSwingConfig swingConfig = usableData!.SwingConfig;
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            ResetMotionStart();
            motion.EndPosition = swingConfig.ReadyPosition + phase.moveDelta;
            motion.EndAngle = swingConfig.ReadyAngle + phase.turnDelta;
            motion.MoveDuration = phase.moveDuration;
            motion.TurnDuration = phase.turnDuration;
        }

        private void SetMotionToReady()
        {
            if (!handStack.IsValid || handStack.itemData.IsNot(out UsableData usableData))
                return;

            ItemSwingConfig swingConfig = usableData!.SwingConfig;

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

        private void AimItem()
        {
            if (!brain.I.AimPosition.HasValue)
            {
                itemPivot.localScale = Vector3.zero;
                return;
            }

            lastAimDirection = brain.I.AimPosition.Value - (Vector2)itemPivot.position;
            float aimAngle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
            bool isLeft = aimAngle is < -90 or > 90;

            itemPivot.localScale = Vector3.one.With(y: isLeft ? -1 : 1);
            itemPivot.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }

        private void UpdateItemSprite()
        {
            if (handStack.itemData.IsNot(out UsableData usableData))
            {
                itemVisual.localScale = Vector3.zero;
                return;
            }

            (Color tint, float scale) = usableData.Is(out PlaceableData placeableData)
                ? (placeableData!.Color, usableData!.IconScale * 0.8f)
                : (Color.white, usableData!.IconScale);

            itemVisual.localScale = Vector3.one * scale;
            itemRenderer.sprite = usableData ? usableData.Icon : null;
            itemRenderer.color = tint;
        }

        private void HandleDie(HealthChangeEventArgs _) => itemRenderer.enabled = false;
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
    }
}
