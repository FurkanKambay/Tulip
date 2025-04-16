using System.Linq;
using SaintsField;
using SaintsField.Playa;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Item Swing Config")]
    public class ItemSwingConfig : ScriptableObject
    {
        public Vector2 ReadyPosition => readyPosition;
        public float ReadyAngle => readyAngle;
        public float ResetMoveDuration => resetMoveDuration;
        public float ResetTurnDuration => resetTurnDuration;

        /// Avoid resetting and loop to phase 0 after last phase.
        public bool Loop => loop;
        public UsePhase[] Phases => phases;

        public float TimeToFirstHit { get; private set; }

        [Header("Config")]
        [SerializeField] protected Vector2 readyPosition;

        [OverlayRichLabel("<color=grey>deg")]
        [SerializeField] protected float readyAngle;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField, Min(0)] protected float resetMoveDuration;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField, Min(0)] protected float resetTurnDuration;

        [Header("Phases")]
        [InfoBox("Avoid resetting and loop to phase 0 after last phase.", show: nameof(loop))]
        [SerializeField] protected bool loop;

        [SaintsRow(inline: true)]
        [SerializeField] protected UsePhase[] phases;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout, marginTop: 16)]
        [SerializeField, ReadOnly] protected UsableData[] usedBy;
        // ReSharper restore NotAccessedField.Global

        private void Awake() =>
            TimeToFirstHit = FindTimeToFirstHit();

        private void OnValidate()
        {
            TimeToFirstHit = FindTimeToFirstHit();

            if (phases.Length > 0 && !phases.Any(phase => phase.shouldHit))
                phases[0].shouldHit = true;

            usedBy = Resources.FindObjectsOfTypeAll<UsableData>()
                .Where(usableData => usableData.SwingConfig == this)
                .ToArray();
        }

        private float FindTimeToFirstHit()
        {
            if (Phases.Length == 0)
                return 0;

            float durationSum = Phases
                .TakeWhile(p => !p.shouldHit)
                .Sum(p => Mathf.Max(p.moveDuration, p.turnDuration));

            UsePhase hitPhase = Phases.First(p => p.shouldHit);
            return durationSum + Mathf.Max(hitPhase.moveDuration, hitPhase.turnDuration);
        }

    }
}
