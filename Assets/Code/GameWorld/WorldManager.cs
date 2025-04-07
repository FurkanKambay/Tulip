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

    public class WorldManager : MonoBehaviour, IWorldProvider
    {
        public event IWorldProvider.ProvideWorldEvent OnProvideWorld;

        [Header("References")]
        [SerializeField] StructureData playgroundStructure;

        [Header("Events")]
        [SerializeField] EventChannelData newGameEvent;
        [SerializeField] EventChannelData continueGameEvent;
        [SerializeField] EventChannelData saveQuitEvent;

        public WorldData World => loadedWorld ?? playgroundStructure.WorldData;

        private readonly WorldSaveDictionary worldSaves = new();

        private WorldData loadedWorld;

        private const string onlyWorldName = "World";

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

#region Event Subscriptions
        private void NewGame_Requested()
        {
            DeleteWorld();
            CreateNewWorld();
            LoadWorld();
        }

        private void ContinueGame_Requested() =>
            LoadWorld();

        private void SaveQuit_Requested() =>
            ReturnToMainMenu();
#endregion

        [Button]
        private void ReturnToMainMenu()
        {
            if (loadedWorld == null)
                return;

            // TODO: reload the scene with the authored world instead
            loadedWorld = playgroundStructure.WorldData;
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.MainMenu);
        }

        [Button]
        private void CreateNewWorld(string worldName = onlyWorldName)
        {
            if (!CanSaveWorld(worldName))
                return;

            worldSaves[worldName] = playgroundStructure.WorldData;
            // TODO: save world data (store only the delta from authored world)
        }

        [Button]
        private void LoadWorld(string worldName = onlyWorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = worldSaves[worldName];
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.Playing);
        }

        [Button]
        private void DeleteWorld(string worldName = onlyWorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = null;
            worldSaves.Remove(worldName);
        }

        private bool CanSaveWorld(string worldName = onlyWorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && !worldSaves.ContainsKey(worldName);

        public bool CanLoadWorld(string worldName = onlyWorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && worldSaves.ContainsKey(worldName)
            && loadedWorld?.Name != worldName;
    }
}
