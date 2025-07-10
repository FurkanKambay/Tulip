using System;
using UnityEngine;

namespace Tulip.Core
{
    public class ReactToGameState : MonoBehaviour
    {
        [SerializeField] bool activeInMainMenu;
        [SerializeField] bool activeInGame;
        [SerializeField] bool activeInPauseMenu;

        private void Awake()
        {
            GameStateChange.Event += GameState_Changed;
            SwitchState(GameState.MainMenu);
        }

        private void OnDestroy() =>
            GameStateChange.Event -= GameState_Changed;

        private void SwitchState(GameState newState)
        {
            gameObject.SetActive(newState switch
            {
                GameState.MainMenu => activeInMainMenu,
                GameState.Playing  => activeInGame,
                GameState.Paused   => activeInPauseMenu,
                _                  => throw new ArgumentOutOfRangeException()
            });
        }

        private void GameState_Changed(GameStateChange args) =>
            SwitchState(args.NewState);
    }
}
