using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class BiomeMusic : MonoBehaviour
    {
        [Header("FMOD Events")]
        [SerializeField] EventReference biomeMusicEvent;

        [Header("References")]
        [SerializeField, Required] EntityLocationDeterminer playerLocation;

        [Header("Config")]
        [SerializeField] Biome startingBiome;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;
        private PARAMETER_DESCRIPTION paramPlayerLocation;

        private async void Awake()
        {
            await AudioBusManager.WaitForAllBanksToLoad();

            EventDescription musicDescription = RuntimeManager.GetEventDescription(biomeMusicEvent);
            musicDescription.getParameterDescriptionByName("Biome", out paramBiome);
            musicDescription.getParameterDescriptionByName("Player Location", out paramPlayerLocation);
            musicDescription.createInstance(out musicInstance);

            SetBiome(startingBiome);
        }

        private void OnEnable() => musicInstance.start();
        private void OnDisable() => musicInstance.stop(STOP_MODE.ALLOWFADEOUT);

        private void Update()
        {
            if (musicInstance.isValid())
                musicInstance.setParameterByID(paramPlayerLocation.id, playerLocation.Location.GetHashCode());
        }

        private void SetBiome(Biome biome) =>
            musicInstance.setParameterByID(paramBiome.id, biome.GetHashCode());
    }
}
