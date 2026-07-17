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
            [SerializeField, Range(0, 100)] private int masterVolume = 80;
            [SerializeField, Range(0, 100)] private int musicVolume = 80;
            [SerializeField, Range(0, 100)] private int effectsVolume = 80;
            [SerializeField, Range(0, 100)] private int uiVolume = 80;

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
        }
    }
}
