namespace FK.Tulip.Core
{
    public readonly struct GameStateChange
    {
        public readonly GameState OldState;
        public readonly GameState NewState;

        public GameStateChange(GameState oldState, GameState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
