namespace Tulip.Core
{
    public readonly struct GameStateEventArgs
    {
        public readonly GameState OldState;
        public readonly GameState NewState;

        public GameStateEventArgs(GameState oldState, GameState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
