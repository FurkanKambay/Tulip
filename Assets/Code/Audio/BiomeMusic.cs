using System.Collections;
using FMOD.Studio;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class BiomeMusic : MonoBehaviour
    {
        [Header("FMOD Events")]
        [SerializeField] private FMODEvent biomeMusic;

        [Header("Config")]
        [SerializeField, EnumButtons] private Biome startingBiome;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;

        private IEnumerator Start()
        {
            // Log.Info($"{logPrefix} waiting for banks to load...", this);
            // yield return Awaitable.WaitForSecondsAsync(2f); // for testing
            yield return AudioManager.WaitForAllBanksToLoad();

            // Log.Info($"{logPrefix} starting BGM...", this);
            biomeMusic.Describe();
            biomeMusic.DescribeParameter("Biome", out paramBiome);
            biomeMusic.StartNewInstance();

            SetBiome(startingBiome);
        }

        private void SetBiome(Biome biome) =>
            biomeMusic.Instance.SetParameter(paramBiome, biome);

        private static readonly string logPrefix = $"[{nameof(BiomeMusic)}]";
    }
}
