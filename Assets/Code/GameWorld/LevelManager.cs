using Furkan.Common;
using Tulip.Character;
using Tulip.Data;
using Tulip.Player;
using Tulip.UI;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Config - Hero Spawn")]
        [SerializeField, Required] private EntitySO heroAsset;
        [SerializeField, Required] private Transform spawnParent;
        [SerializeField, Required] private Transform spawnLocation;

        [Header("Config - Injections")]
        [SerializeField, Required] private CameraFollow cameraFollow;
        [SerializeField, Required] private DeathOverlayPresenter deathUI;
        [SerializeField, Required] private World world;

        [Header("State")]
        [SerializeField] private TangibleEntity hero;

        private void Awake()
        {
            if (!spawnLocation)
                spawnLocation = transform;
            if (!hero)
                hero = TangibleEntity.Spawn(heroAsset, world, spawnLocation.position, spawnParent);

            cameraFollow.SetTarget(hero.transform);
            deathUI.SetPlayer(hero);
        }
    }
}
