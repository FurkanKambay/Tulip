using System;
using FK.Common;
using FK.Tulip.Data.GameEvents;
using UnityEngine;

namespace FK.Tulip.Core
{
    public class ReactToGameState : MonoBehaviour
    {
        [SerializeField, Required] private GameStateChangeEvent gameStateChangeEvent;

        [Header("Config")]
        [SerializeField] private bool activeInMainMenu;
        [SerializeField] private bool activeInGame;
        [SerializeField] private bool activeInPauseMenu;

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
