using System;
using Unity.Properties;
using UnityEngine;

namespace FK.Tulip.Core
{
    public partial class Settings
    {
        [Serializable]
        public sealed record AudioSettingsBag
        {
            [SerializeField, Range(0, 10)] private int masterVolume = 8;
            [SerializeField, Range(0, 10)] private int musicVolume = 8;
            [SerializeField, Range(0, 10)] private int ambienceVolume = 8;
            [SerializeField, Range(0, 10)] private int effectsVolume = 8;
            [SerializeField, Range(0, 10)] private int uiVolume = 8;
            [SerializeField] private bool muteInBackground;

            internal AudioSettingsBag()
            {
            }

            [CreateProperty]
            public int MasterVolume
            {
                get => masterVolume;
                set => UpdateSetting(ref masterVolume, value);
            }

            [CreateProperty]
            public int MusicVolume
            {
                get => musicVolume;
                set => UpdateSetting(ref musicVolume, value);
            }

            [CreateProperty]
            public int AmbienceVolume
            {
                get => ambienceVolume;
                set => UpdateSetting(ref ambienceVolume, value);
            }

            [CreateProperty]
            public int EffectsVolume
            {
                get => effectsVolume;
                set => UpdateSetting(ref effectsVolume, value);
            }

            [CreateProperty]
            public int UIVolume
            {
                get => uiVolume;
                set => UpdateSetting(ref uiVolume, value);
            }

            [CreateProperty]
            public bool MuteInBackground
            {
                get => muteInBackground;
                set => UpdateSetting(ref muteInBackground, value);
            }
        }
    }
}
