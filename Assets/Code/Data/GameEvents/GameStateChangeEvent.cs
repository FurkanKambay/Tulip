using FK.Common;
using FK.Tulip.Core;
using UnityEngine;

namespace FK.Tulip.Data.GameEvents
{
    [CreateAssetMenu(menuName = "Game Events/Game State Change")]
    public sealed class GameStateChangeEvent : GameEvent<GameStateChange>
    {
        public void Raise(Object sender, GameState oldState, GameState newState) =>
            Raise(sender, new GameStateChange(oldState, newState));
    }
}
