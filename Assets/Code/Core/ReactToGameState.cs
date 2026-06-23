using System;
using Furkan.Common;
using Tulip.Data.GameEvents;
using UnityEngine;

namespace Tulip.Core
{
    public class ReactToGameState : MonoBehaviour
    {
        [SerializeField, Required] GameStateChangeEvent gameStateChangeEvent;

        [Header("Config")]
        [SerializeField] bool activeInMainMenu;
        [SerializeField] bool activeInGame;
        [SerializeField] bool activeInPauseMenu;

        private void Awake()
        {
            gameStateChangeEvent.OnRaised += GameState_Changed;
            SwitchState(GameState.MainMenu);
        }

        private void OnDestroy() =>
            gameStateChangeEvent.OnRaised -= GameState_Changed;

        private void SwitchState(GameState newState)
        {
            gameObject.SetActive(
                newState switch
                {
                    GameState.MainMenu => activeInMainMenu,
                    GameState.Playing => activeInGame,
                    GameState.Paused => activeInPauseMenu,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
        }

        private void GameState_Changed(GameStateChange args) =>
            SwitchState(args.NewState);
    }
}
