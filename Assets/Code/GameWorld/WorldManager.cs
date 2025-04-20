using Furkan.Common;
using Tulip.Core;
using UnityEngine;

namespace Tulip.GameWorld
{
    /// <summary>
    /// Handles game flow events like starting a new game, continuing from a save, and quitting.
    /// </summary>
    /// <seealso cref="GameManager"/>
    public class WorldManager : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] EventChannelSO newGameEvent;
        [SerializeField] EventChannelSO continueGameEvent;
        [SerializeField] EventChannelSO saveQuitEvent;

        private void OnEnable()
        {
            newGameEvent.OnRaised += NewGame_Requested;
            continueGameEvent.OnRaised += ContinueGame_Requested;
            saveQuitEvent.OnRaised += SaveQuit_Requested;
        }

        private void OnDisable()
        {
            newGameEvent.OnRaised -= NewGame_Requested;
            continueGameEvent.OnRaised -= ContinueGame_Requested;
            saveQuitEvent.OnRaised -= SaveQuit_Requested;
        }

        private static async void NewGame_Requested()
        {
            await GameManager.StartNewGame();
            // TODO: save the new world to disk
        }

        private void ContinueGame_Requested()
        {
            Debug.LogWarning("TODO: Continue Game", this);
            // TODO: load latest save instead
        }

        private static async void SaveQuit_Requested()
        {
            // TODO: save the world to disk
            // only store the tile delta and other world state
            // default world should be stored in scene, and its WorldSO cached on build?

            // TODO: reload the scene with the authored world instead
            await GameManager.ReturnToMainMenu();
        }
    }
}
