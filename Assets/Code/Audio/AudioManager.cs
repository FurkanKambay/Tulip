using System.Threading.Tasks;
using FK.Common;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Settings = FK.Tulip.Core.Settings;

namespace FK.Tulip.Audio
{
    internal sealed class AudioManager : MonoBehaviour
    {
        private VCA masterVCA;
        private VCA musicVCA;
        private VCA ambienceVCA;
        private VCA sfxVCA;
        private VCA uiVCA;

        private async void Awake()
        {
            DontDestroyOnLoad(gameObject);
            await WaitForAllBanksToLoad();

            masterVCA = RuntimeManager.GetVCA("vca:/Master");
            ambienceVCA = RuntimeManager.GetVCA("vca:/Ambience");
            musicVCA = RuntimeManager.GetVCA("vca:/Music");
            sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
            uiVCA = RuntimeManager.GetVCA("vca:/UI");
        }

        private void OnEnable()
        {
            Settings.OnUpdate += Settings_Updated;
        }

        private void OnDisable()
        {
            Settings.OnUpdate -= Settings_Updated;
        }

        private async void Start() => await UpdateVolumes();

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!RuntimeManager.StudioSystem.isValid())
                return;

            if (Settings.Audio.MuteInBackground)
                RuntimeManager.PauseAllEvents(!hasFocus);

            if (!hasFocus)
                RuntimeManager.CoreSystem.mixerSuspend();
            else
                RuntimeManager.CoreSystem.mixerResume();
        }

        internal static async Awaitable WaitForAllBanksToLoad()
        {
            int frames = 0;
            while (!RuntimeManager.HaveAllBanksLoaded)
            {
                await Awaitable.NextFrameAsync();
                frames++;
            }

            if (frames > 0)
                Log.Info($"{logPrefix} All banks have been loaded after {frames} frames.");
        }

        private async void Settings_Updated() => await UpdateVolumes();

        private async Task UpdateVolumes()
        {
            await WaitForAllBanksToLoad();

            SetVolume(masterVCA, Settings.Audio.MasterVolume);
            SetVolume(musicVCA, Settings.Audio.MusicVolume);
            SetVolume(ambienceVCA, Settings.Audio.AmbienceVolume);
            SetVolume(sfxVCA, Settings.Audio.EffectsVolume);
            SetVolume(uiVCA, Settings.Audio.UIVolume);
        }

        private static void SetVolume(VCA vca, int value) =>
            vca.setVolume(Mathf.InverseLerp(0, 100, value));

        private static readonly string logPrefix = $"[{nameof(AudioManager)}]";
    }
}
