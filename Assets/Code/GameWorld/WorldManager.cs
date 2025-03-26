using System;
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

        public WorldData World => loadedWorld ?? playgroundStructure.WorldData;

        private readonly WorldSaveDictionary worldSaves = new();
        private WorldData loadedWorld;

        private const string onlyWorldName = "World";

        private void Awake() => ReturnToMainMenu();

        [Button]
        public void ReturnToMainMenu()
        {
            if (loadedWorld == null)
                return;

            // TODO: reload the scene with the authored world instead
            loadedWorld = playgroundStructure.WorldData;
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.MainMenu);
        }

        [Button]
        public void CreateNewWorld(string worldName = onlyWorldName)
        {
            if (!CanSaveWorld(worldName))
                return;

            worldSaves[worldName] = playgroundStructure.WorldData;
            // TODO: save world data (store only the delta from authored world)
        }

        [Button]
        public void LoadWorld(string worldName = onlyWorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = worldSaves[worldName];
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.Playing);
        }

        [Button]
        public void DeleteWorld(string worldName = onlyWorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = null;
            worldSaves.Remove(worldName);
        }

        public bool CanSaveWorld(string worldName = onlyWorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && !worldSaves.ContainsKey(worldName);

        public bool CanLoadWorld(string worldName = onlyWorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && worldSaves.ContainsKey(worldName)
            && loadedWorld?.Name != worldName;
    }
}
