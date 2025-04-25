using System;
using Tulip.Character;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Data
{
    /// TODO: support non-Health effects
    [Serializable]
    public class StatusEffect
    {
        public bool IsDone => !SO.IsPermanent && RemainingDuration <= 0;

        [field: SerializeField] public StatusEffectSO SO { get; private set; }
        [field: SerializeField] public Health Source { get; private set; }
        [field: SerializeField] public Health Target { get; private set; }

        public float RemainingDuration { get; private set; }

        private float timeSinceLastProc;

        internal StatusEffect(StatusEffectSO so, Health source, Health target)
        {
            SO = so;
            Source = source;
            Target = target;
            RemainingDuration = so.Duration;
        }

        public void Tick(float deltaTime)
        {
            timeSinceLastProc += deltaTime;
            RemainingDuration -= deltaTime;

            if (timeSinceLastProc < SO.Rate || IsDone)
                return;

            Proc();
            timeSinceLastProc = 0;
        }

        private void Proc()
        {
            // BUG: deaths by statuses don't award loot
            if (SO.Amount < 0)
                Target.Damage(-SO.Amount, Source, DamageType.StatusEffect);
            else
                Target.Heal(SO.Amount, Source);
        }
    }
}
