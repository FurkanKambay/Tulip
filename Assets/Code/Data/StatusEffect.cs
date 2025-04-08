using System;
using Tulip.Character;
using UnityEngine;

namespace Tulip.Data
{
    /// TODO: support non-Health effects
    [Serializable]
    public class StatusEffect
    {
        public bool IsDone => !Data.IsPermanent && RemainingDuration <= 0;

        [field: SerializeField] public StatusEffectData Data { get; private set; }
        [field: SerializeField] public Health Source { get; private set; }
        [field: SerializeField] public Health Target { get; private set; }

        public float RemainingDuration { get; private set; }

        private float timeSinceLastProc;

        internal StatusEffect(StatusEffectData data, Health source, Health target)
        {
            Data = data;
            Source = source;
            Target = target;
            RemainingDuration = data.Duration;
        }

        public void Tick(float deltaTime)
        {
            timeSinceLastProc += deltaTime;
            RemainingDuration -= deltaTime;

            if (timeSinceLastProc < Data.Rate || IsDone)
                return;

            Proc();
            timeSinceLastProc = 0;
        }

        private void Proc()
        {
            // BUG: deaths by statuses don't award loot
            if (Data.Amount < 0)
                Target.Damage(-Data.Amount, Source, checkInvulnerable: false);
            else
                Target.Heal(Data.Amount, Source);
        }
    }
}
