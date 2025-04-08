using System;
using Furkan.Common;
using SaintsField;
using SaintsField.Playa;
using Tulip.Core;
using Tulip.Data;
using UnityEngine;

namespace Tulip.GameWorld
{
    [Serializable]
    public class WorldSaveDictionary : SaintsDictionary<string, WorldData>
    {
    }

    public delegate void ProvideWorldEvent(WorldData worldData);

    public class WorldManager : MonoBehaviour
    {
        public event ProvideWorldEvent OnProvideWorld;

        [Header("References")]
        [SerializeField] WorldData world;

        [Header("Events")]
        [SerializeField] EventChannelData newGameEvent;
        [SerializeField] EventChannelData continueGameEvent;
        [SerializeField] EventChannelData saveQuitEvent;

        public WorldData World => loadedWorld.Or(world);

        private WorldData loadedWorld;

#region Unity Lifecycle
        private void Awake() => ReturnToMainMenu();

        private void OnEnable()
        {
            newGameEvent.OnRaised      += NewGame_Requested;
            continueGameEvent.OnRaised += ContinueGame_Requested;
            saveQuitEvent.OnRaised     += SaveQuit_Requested;
        }

        private void OnDisable()
        {
            newGameEvent.OnRaised      -= NewGame_Requested;
            continueGameEvent.OnRaised -= ContinueGame_Requested;
            saveQuitEvent.OnRaised     -= SaveQuit_Requested;
        }
#endregion

#region Event Handlers
        private void NewGame_Requested()
        {
            UnloadWorld();
            LoadInitialWorld();
            // TODO: save the new world
        }

        private void ContinueGame_Requested()
        {
            LoadInitialWorld();
            // TODO: load latest save instead
        }

        private void SaveQuit_Requested()
        {
            // TODO: save the world to disk
            // only store the tile delta and other world state
            // default world should be stored in scene, and its WorldData cached on build?

            ReturnToMainMenu();
        }
#endregion

        [Button]
        private void ReturnToMainMenu()
        {
            if (!loadedWorld)
                return;

            // TODO: reload the scene with the authored world instead
            loadedWorld = world;
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.MainMenu);
        }

        [Button]
        private void LoadInitialWorld()
        {
            loadedWorld = world;
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.Playing);
        }

        [Button]
        private void UnloadWorld() =>
            loadedWorld = null;
    }
}
