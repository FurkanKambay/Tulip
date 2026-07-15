using Furkan.Common.Extensions;
using Tulip.Data;
using Tulip.Data.Sets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Dev
{
    /// <summary>
    /// Shift + Q-E: switch entity
    /// Shift + S: spawn selected entity
    /// Shift + X: destroy all spawns
    /// </summary>
    public class ArenaManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform spawnParent;

        [Header("Config")]
        [SerializeField] private EntitySet spawnSet;
        [SerializeField] private EntityAsset spawnedEntity;

        private Camera camera;
        private int spawnIndex;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard.shiftKey.isPressed)
            {
                if (keyboard.sKey.wasPressedThisFrame)
                    SpawnEntityAtCursor();
                if (keyboard.xKey.wasPressedThisFrame)
                    DestroyAllSpawns();

                if (keyboard.qKey.wasPressedThisFrame)
                    SwitchSpawnedEntity(-1);
                else if (keyboard.eKey.wasPressedThisFrame)
                    SwitchSpawnedEntity(+1);
            }
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 14;

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Label("<color=yellow>Spawn Entity");
                GUILayout.Space(8);
                GUILayout.Label("<color=grey>[Shift + Q] Previous");

                for (int i = 0; i < spawnSet.Count; i++)
                {
                    EntityAsset entity = spawnSet[i];

                    bool selected = i == spawnIndex;
                    string color = selected ? "yellow" : "white";
                    string info = selected ? "[Shift + S]" : string.Empty;

                    GUILayout.Label($"{i + 1,2}. <color={color}>{entity.Prefab.name} {info}");
                }

                GUILayout.Label("<color=grey>[Shift + E] Next");
                GUILayout.Space(8);
                GUILayout.Label("<color=grey>[Shift + X] Destroy All");
            }
        }

        private void SwitchSpawnedEntity(int direction)
        {
            spawnIndex = (int)Mathf.Repeat(spawnIndex + direction, spawnSet.Count);
            spawnedEntity = spawnSet[spawnIndex];
        }

        private void SpawnEntityAtCursor()
        {
            Vector2 screenPoint = Pointer.current.position.ReadValue();
            Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint).With(z: 0);
            Instantiate(spawnedEntity.Prefab, worldPoint, Quaternion.identity, spawnParent);
        }

        [ContextMenu("Destroy All Spawns (Shift + X)")]
        private void DestroyAllSpawns()
        {
            for (int i = spawnParent.childCount - 1; i >= 0; i--)
                Destroy(spawnParent.GetChild(i).gameObject);
        }
    }
}
