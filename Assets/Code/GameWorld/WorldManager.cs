using FK.Common;
using FK.Tulip.Core;
using UnityEngine;

namespace FK.Tulip.GameWorld
{
    /// <summary>
    /// Handles game flow events like starting a new game, continuing from a save, and quitting.
    /// </summary>
    /// <seealso cref="GameManager"/>
    public sealed class WorldManager : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent newGameEvent;
        [SerializeField] private GameEvent continueGameEvent;
        [SerializeField] private GameEvent saveQuitEvent;

        private void Awake() =>
            DontDestroyOnLoad(gameObject);

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

        private static void NewGame_Requested()
        {
            GameManager.StartNewGame();
            // TODO: save the new world to disk
        }

        private void ContinueGame_Requested()
        {
            Log.Warning("TODO: Continue Game", this);
            // TODO: load latest save instead
        }

        private static void SaveQuit_Requested()
        {
            // TODO: save the world to disk
            // only store the tile delta and other world state
            // default world should be stored in scene, and its WorldSO cached on build?

            // TODO: reload the scene with the authored world instead
            GameManager.ReturnToMainMenu();
        }
    }
}
