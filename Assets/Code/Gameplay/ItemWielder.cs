using System;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Combat;
using FK.Tulip.Core;
using FK.Tulip.Data;
using FK.Tulip.Data.Items;
using FK.Tulip.Input;
using UnityEngine;

namespace FK.Tulip.Gameplay
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
        [SerializeField, Required] private CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] private Health health;
        [SerializeField, Required] private SpriteRenderer itemRenderer;
        [SerializeField, Required] private ItemMotionController itemMotion;

        [Header("Config")]
        [SerializeField] private ItemAsset equippedItem;

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

        private Vector3 AimPointWorld => itemPivot.position + (Vector3)aimVector;

#region Unity Lifecycle
        private void Awake()
        {
            itemVisual = itemRenderer.transform;
            itemPivot = itemVisual.parent;
        }

        private void OnEnable()
        {
            health.OnDie += HandleDie;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;
        }

        private void Start()
        {
            itemMotion.SetItem(equippedItem);
        }

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
            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[itemMotion.PhaseIndex] : default;

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
                    itemMotion.TickMotion();

                    // we're still Lerping, so we skip to the next tick
                    if (!itemMotion.IsDone)
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

                    bool isFinalPhase = itemMotion.PhaseIndex == swingConfig.Phases.Length - 1;
                    bool shouldReset = !wantsToSwing || !swingConfig.Loop;

                    if (isFinalPhase && shouldReset)
                    {
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // still not ending so next phase. keeps swinging without resetting
                    // looping: start from phase 0 again

                    // return to phase 0 with `phase.XDuration`, NOT `swingType.ResetXDuration`
                    if (isFinalPhase)
                        itemMotion.ReturnToFirstPhase();
                    else
                        itemMotion.IncrementPhase();

                    break;
                case ItemSwingState.Resetting:
                    itemMotion.TickMotion();

                    if (itemMotion.IsDone)
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
                    itemMotion.SetItem(equippedItem);
                    OnReady?.Invoke(equippedItem);
                    break;
                case ItemSwingState.Swinging:
                    itemMotion.ReturnToFirstPhase();
                    OnSwingStart?.Invoke(equippedItem, AimPointWorld);
                    break;
                case ItemSwingState.Resetting:
                    itemMotion.ResetToReady();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state));
            }
        }

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

#region Event Handlers
        private void HandleDie(CombatPacket _) => itemRenderer.enabled = false;
        private void HandleRevived(Health reviver) => itemRenderer.enabled = true;
#endregion

        private enum ItemSwingState
        {
            Ready,
            Swinging,
            Resetting
        }
    }
}
