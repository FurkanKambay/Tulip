using SaintsField;
using Tulip.Character;
using Tulip.Data.Gameplay;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class Respawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Config")]
        [SerializeField] bool autoRespawn = true;
        [SerializeField] float respawnDelay;
        [SerializeField] Vector3 respawnPosition;

        public float SecondsUntilRespawn { get; private set; }
        public bool CanRespawn => SecondsUntilRespawn <= 0;

        private TangibleEntity entity;
        private Transform subject;
        private World world;

        private void Awake()
        {
            entity = health.GetComponentInParent<TangibleEntity>();
            subject = entity.transform;
            world = entity.World;

            SetPosition();
        }

        private void OnEnable() => health.OnDie += Health_Die;
        private void OnDisable() => health.OnDie -= Health_Die;

        private void Update()
        {
            if (health.IsAlive)
                return;

            SecondsUntilRespawn -= Time.deltaTime;

            if (autoRespawn)
                TryRespawn();
        }

        [ContextMenu(nameof(TryRespawn))]
        public void TryRespawn()
        {
            if (!CanRespawn)
                return;

            SecondsUntilRespawn = 0;
            SetPosition();
            health.Revive();
        }

        private void SetPosition()
        {
            Vector2Int cell = world.WorldToCell(respawnPosition);

            while (!world.CanAccommodate(cell, entity.EntitySO.Size))
                cell.y++;

            subject.position = world.CellCenter(cell);
        }

        private void Health_Die(HealthChangeEventArgs _) => SecondsUntilRespawn = respawnDelay;
    }
}
