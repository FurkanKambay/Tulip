using FMOD.Studio;
using FMODUnity;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;

        [Header("FMOD Events")]
        [SerializeField] EventReference tilePlacedEvent;
        [SerializeField] EventReference tileDamagedEvent;
        [SerializeField] EventReference tileDestroyedEvent;

        private PARAMETER_DESCRIPTION paramMaterial;
        private PARAMETER_DESCRIPTION paramDamage;

        private async void Awake()
        {
            await AudioBusManager.WaitForAllBanksToLoad();

            EventDescription description = RuntimeManager.GetEventDescription(tileDamagedEvent);
            description.getParameterDescriptionByName("Placeable Material", out paramMaterial);
            description.getParameterDescriptionByName("Damage", out paramDamage);
        }

        private void OnEnable()
        {
            world.OnPlaceTile += HandleTerraformed;
            world.OnHitTile += HandleTerraformed;
            world.OnDestroyTile += HandleTerraformed;
        }

        private void OnDisable()
        {
            world.OnPlaceTile -= HandleTerraformed;
            world.OnHitTile -= HandleTerraformed;
            world.OnDestroyTile -= HandleTerraformed;
        }

        private void HandleTerraformed(TileModification modification)
        {
            EventInstance sfx = modification.Kind switch
            {
                TileModificationKind.Placed => RuntimeManager.CreateInstance(tilePlacedEvent),
                TileModificationKind.Damaged => RuntimeManager.CreateInstance(tileDamagedEvent),
                _ => RuntimeManager.CreateInstance(tileDestroyedEvent)
            };

            PlaceableSO placeableSO = modification.PlaceableSO;

            if (modification.Kind == TileModificationKind.Damaged)
            {
                int tileDamage = world.GetTileDamage(modification.Cell, placeableSO.TileType);
                float tileHealth = (float)tileDamage / placeableSO.Hardness;
                sfx.setParameterByID(paramDamage.id, tileHealth);
            }

            sfx.set3DAttributes(world.CellCenter(modification.Cell).To3DAttributes());
            sfx.setParameterByID(paramMaterial.id, (float)placeableSO.Material, ignoreseekspeed: true);

            sfx.start();
            sfx.release();
        }
    }
}
