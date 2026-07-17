using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class BiomeMusic : MonoBehaviour
    {
        [Header("FMOD Events")]
        [SerializeField] private EventReference biomeMusicEvent;

        [Header("Config")]
        [SerializeField] private Biome startingBiome;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;

        private async void Awake()
        {
            await AudioBusManager.WaitForAllBanksToLoad();

            EventDescription musicDescription = RuntimeManager.GetEventDescription(biomeMusicEvent);
            musicDescription.getParameterDescriptionByName("Biome", out paramBiome);
            musicDescription.createInstance(out musicInstance);

            SetBiome(startingBiome);
        }

        private void OnEnable() => musicInstance.start();
        private void OnDisable() => musicInstance.stop(STOP_MODE.ALLOWFADEOUT);

        private void SetBiome(Biome biome) =>
            musicInstance.setParameterByID(paramBiome.id, biome.GetHashCode());
    }
}
