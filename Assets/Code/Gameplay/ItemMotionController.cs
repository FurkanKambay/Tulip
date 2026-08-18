using FK.Common.Extensions;
using FK.Tulip.Data;
using FK.Tulip.Data.Items;
using UnityEngine;
using Vertx.Attributes;

namespace FK.Tulip.Gameplay
{
    internal sealed class ItemMotionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform itemPivot;
        [SerializeField] private SpriteRenderer itemRenderer;

        [Header("Debug")]
        [ReadOnlyField, SerializeField] private UsableAsset usableAsset;
        [ReadOnlyField, SerializeField] private int phaseIndex;

        // TODO: rename to KeyframeIndex
        public int PhaseIndex => phaseIndex;

        public bool IsDone => Mathf.Approximately(lerpMove, 1f) && Mathf.Approximately(lerpTurn, 1f);

        private Vector2 fromPosition;
        private Vector2 toPosition;
        private float fromAngle;
        private float toAngle;
        private float moveDuration;
        private float turnDuration;
        private float lerpMove;
        private float lerpTurn;

        private Transform itemVisual;

        private void Awake()
        {
            itemVisual = itemRenderer.transform;
        }

        public void SetItem(ItemAsset itemAsset)
        {
            itemRenderer.sprite = itemAsset ? itemAsset.Icon : null;
            itemVisual.localScale = itemAsset ? Vector3.one * itemAsset.IconScale : Vector3.zero;

            if (itemAsset.IsNot(out usableAsset))
                itemVisual.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
            else
            {
                ItemSwingConfig swing = usableAsset.SwingConfig;
                itemVisual.SetLocalPositionAndAngle(swing.ReadyPosition, swing.ReadyAngle);
            }

            phaseIndex = 0;
            ResetMotionValues();
        }

        public void ReturnToFirstPhase()
        {
            phaseIndex = 0;
            SetMotionToCurrentPhase();
        }

        public void IncrementPhase()
        {
            phaseIndex++;
            SetMotionToCurrentPhase();
        }

        public void ResetToReady()
        {
            if (!usableAsset || usableAsset.SwingConfig.IsNot(out ItemSwingConfig swingConfig))
                return;

            ResetMotionValues();
            toPosition = swingConfig.ReadyPosition;
            toAngle = swingConfig.ReadyAngle;
            moveDuration = swingConfig.ResetMoveDuration;
            turnDuration = swingConfig.ResetTurnDuration;
        }

        public void TickMotion()
        {
            lerpMove = moveDuration <= 0 || lerpMove >= 1
                ? 1
                : Mathf.MoveTowards(lerpMove, 1, Time.deltaTime / moveDuration);

            lerpTurn = turnDuration <= 0 || lerpTurn >= 1
                ? 1
                : Mathf.MoveTowards(lerpTurn, 1, Time.deltaTime / turnDuration);

            itemVisual.SetLocalPositionAndAngle(
                Vector2.Lerp(fromPosition, toPosition, lerpMove),
                Mathf.LerpAngle(fromAngle, toAngle, lerpTurn)
            );
        }

        private void SetMotionToCurrentPhase()
        {
            if (!usableAsset || usableAsset.SwingConfig.IsNot(out ItemSwingConfig swingConfig))
                return;

            UsePhase phase = swingConfig.Phases.Length > 0 ? swingConfig.Phases[phaseIndex] : default;

            ResetMotionValues();
            toPosition = swingConfig.ReadyPosition + phase.moveDelta;
            toAngle = swingConfig.ReadyAngle + phase.turnDelta;
            moveDuration = phase.moveDuration;
            turnDuration = phase.turnDuration;
        }

        private void ResetMotionValues()
        {
            fromPosition = itemVisual.localPosition;
            fromAngle = itemVisual.localEulerAngles.z;

            toPosition = Vector2.zero;
            toAngle = 0f;
            moveDuration = 0f;
            turnDuration = 0f;
            lerpMove = 0f;
            lerpTurn = 0f;
        }
    }
}
