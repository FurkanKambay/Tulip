using FK.Common;
using FK.Tulip.Character;
using FK.Tulip.GameWorld;
using UnityEngine;

namespace FK.Tulip.Combat
{
    public class Respawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private Health health;

        [Header("Config")]
        [SerializeField] private bool autoRespawn = true;
        [SerializeField] private float respawnDelay;
        [SerializeField] private Vector3 respawnPosition;

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

            ResetToRespawnPosition();
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
            ResetToRespawnPosition();
            health.Revive();
        }

        private void ResetToRespawnPosition()
        {
            subject.position = respawnPosition;
        }

        private void Health_Die(CombatPacket _) => SecondsUntilRespawn = respawnDelay;
    }
}
