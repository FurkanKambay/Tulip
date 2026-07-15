using System;
using UnityEngine;

namespace Tulip.Combat
{
    /// TODO: support non-Health effects
    [Serializable]
    public class StatusEffect
    {
        public bool IsDone => !Asset.IsPermanent && RemainingDuration <= 0;

        [field: SerializeField] public StatusEffectAsset Asset { get; private set; }
        [field: SerializeField] public Health Source { get; private set; }
        [field: SerializeField] public Health Target { get; private set; }

        public float RemainingDuration { get; private set; }

        private float timeSinceLastProc;

        internal StatusEffect(StatusEffectAsset asset, Health source, Health target)
        {
            Asset = asset;
            Source = source;
            Target = target;
            RemainingDuration = asset.Duration;
        }

        public void Tick(float deltaTime)
        {
            timeSinceLastProc += deltaTime;
            RemainingDuration -= deltaTime;

            if (timeSinceLastProc < Asset.Rate || IsDone)
                return;

            Proc();
            timeSinceLastProc = 0;
        }

        private void Proc()
        {
            // BUG: deaths by statuses don't award loot
            if (Asset.Amount < 0)
                Target.Damage(-Asset.Amount, Source, DamageType.StatusEffect);
            else
                Target.Heal(Asset.Amount, Source);
        }
    }
}
