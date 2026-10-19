using System.Collections;
using FK.Common;
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

        [Header("Debug")]
        [SerializeField, Min(0)] private float artificialStartDelay;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;

        private IEnumerator Start()
        {
            if (artificialStartDelay > 0)
            {
                Log.Info($"{logPrefix} waiting for banks to load... (with artificial delay)", this);
                yield return Awaitable.WaitForSecondsAsync(artificialStartDelay);
            }

            yield return AudioManager.WaitForAllBanksToLoad();

            biomeMusic.Describe();
            biomeMusic.DescribeParameter("Biome", out paramBiome);
            biomeMusic.StartNewInstance();

            SetBiome(startingBiome);
        }

        private void OnDestroy()
        {
            biomeMusic.StopInstance();
            biomeMusic.Instance.release();
        }

        private void SetBiome(Biome biome) =>
            biomeMusic.Instance.SetParameter(paramBiome, biome);

        private static readonly string logPrefix = $"[{nameof(BiomeMusic)}]";
    }
}
