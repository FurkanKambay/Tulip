using System;

namespace Tulip.Core
{
    public readonly struct GameStateChange
    {
        public static event Action<GameStateChange> Event;

        public readonly GameState OldState;
        public readonly GameState NewState;

        private GameStateChange(GameState oldState, GameState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public static void Raise(GameState oldState, GameState newState) =>
            Event?.Invoke(new GameStateChange(oldState, newState));
    }
}
