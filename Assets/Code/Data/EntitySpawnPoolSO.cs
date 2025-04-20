using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Entity Spawn Pool")]
    public class EntitySpawnPoolSO : ScriptableObject
    {
        [SerializeField] EntitySO[] entities;

        public EntitySO[] Entities => entities;
        public int Amount => entities.Length;

        public EntitySO this[int index] => entities[index];
    }
}
