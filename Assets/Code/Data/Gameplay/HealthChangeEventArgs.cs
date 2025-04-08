using Tulip.Character;
using UnityEngine;

namespace Tulip.Data.Gameplay
{
    public readonly struct HealthChangeEventArgs
    {
        public readonly float Amount;
        public readonly Health Source;
        public readonly Health Target;
        public readonly Vector2 SourcePosition;

        public HealthChangeEventArgs(float amount, Health source, Health target, Vector2 sourcePosition)
        {
            Amount = amount;
            Source = source;
            Target = target;
            SourcePosition = sourcePosition;
        }

        public override string ToString() => $"{Source} to {Target} for {Amount}";
    }
}
