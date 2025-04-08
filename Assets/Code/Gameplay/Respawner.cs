using SaintsField;
using Tulip.Character;
using Tulip.Core;
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
        }

        private void OnEnable()
        {
            GameStateChange.Event += GameState_Change;
            health.OnDie += Health_Die;
        }

        private void OnDisable()
        {
            GameStateChange.Event -= GameState_Change;
            health.OnDie -= Health_Die;
        }

        private void GameState_Change(GameStateChange args)
        {
            bool startedPlaying = args.NewState is GameState.Playing && args.OldState is not GameState.Paused;

            if (args.NewState == GameState.MainMenu || startedPlaying)
                TryRespawn();
        }

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

            while (!world.CanAccommodate(cell, entity.EntityData.Size))
                cell.y++;

            subject.position = world.CellCenter(cell);
        }

        private void Health_Die(HealthChangeEventArgs _) => SecondsUntilRespawn = respawnDelay;
    }
}
