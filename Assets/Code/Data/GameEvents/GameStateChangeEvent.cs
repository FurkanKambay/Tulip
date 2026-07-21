using System.Runtime.CompilerServices;
using FK.Common.Events;
using FK.Tulip.Core;
using UnityEngine;

namespace FK.Tulip.Data.GameEvents
{
    [CreateAssetMenu(menuName = "Game Events/Game State Change")]
    public sealed class GameStateChangeEvent : GameEvent<GameStateChange>
    {
        public void Raise(Object sender, GameState oldState, GameState newState,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            Raise(sender, new GameStateChange(oldState, newState), callerMember, callerFile, callerLine);
        }
    }
}
