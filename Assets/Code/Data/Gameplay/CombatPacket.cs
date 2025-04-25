using Tulip.Character;
using UnityEngine;

namespace Tulip.Data.Gameplay
{
    public enum DamageType
    {
        MeleeWeapon,
        RangedWeapon,
        StatusEffect
    }

    public struct CombatPacket
    {
        public float Amount;
        public Health Source;
        public Health Target;
        public Vector2 SourcePosition;
        public DamageType DamageType;

        public override readonly string ToString() =>
            $"[Combat Packet] {Source.Entity.name} to {Target.Entity.name} for {Amount} with {DamageType}";
    }
}
