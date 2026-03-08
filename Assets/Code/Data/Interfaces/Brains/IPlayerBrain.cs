using UnityEngine;

namespace Tulip.Data
{
    public interface IPlayerBrain : ICharacterBrain, IJumperBrain, IDasherBrain
    {
        public Vector2 AimPointScreen { get; }
    }
}
