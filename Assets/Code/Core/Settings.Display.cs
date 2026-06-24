using System;
using System.Collections.Generic;
using System.Linq;
using Furkan.Common.Extensions;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Settings
    {
        [Serializable]
        public sealed record DisplaySettingsBag : ISerializationCallbackReceiver
        {
            [SerializeField] FullScreenMode windowMode = FullScreenMode.FullScreenWindow;
            [SerializeField] string resolution = DefaultResolution;

            private static string DefaultResolution =>
                resolutions.Any() ? resolutions.First() : string.Empty;

            private static List<string> resolutions =
                Screen.resolutions.Select(ResolutionExtensions.AsString).Reverse().ToList();

            internal DisplaySettingsBag()
            {
            }

            // ReSharper disable UnusedMember.Global
            [CreateProperty]
            public string WindowMode
            {
                get => WindowModeToString(windowMode);
                set
                {
                    FullScreenMode newMode = StringToWindowMode(value);
                    Screen.fullScreenMode = newMode;
                    UpdateSetting(ref windowMode, newMode);
                }
            }

            [CreateProperty]
            public string Resolution
            {
                get => resolution;
                set
                {
                    if (!SupportedResolutions.Contains(value))
                        return;

                    resolution = value;

                    Resolution newResolution = value.AsResolution();
                    Screen.SetResolution(newResolution.width, newResolution.height, windowMode);
                    UpdateSetting(ref resolution, resolution);
                }
            }
            // ReSharper restore UnusedMember.Global

            // ReSharper disable UnusedMember.Local
            [CreateProperty]
            private List<string> SupportedResolutions => resolutions;

            [CreateProperty]
            private List<string> supportedWindowModes = new()
            {
                WindowModeToString(FullScreenMode.FullScreenWindow),
                WindowModeToString(FullScreenMode.Windowed)
            };
            // ReSharper restore UnusedMember.Local

#region Converters
            /// TODO: use Localization
            private static string WindowModeToString(FullScreenMode value) =>
                windowModeLabels[value];

            /// TODO: use Localization
            private static FullScreenMode StringToWindowMode(string value) =>
                windowModeLabels.First(kvp => kvp.Value == value).Key;
#endregion

            void ISerializationCallbackReceiver.OnBeforeSerialize() => resolution = Resolution;
            void ISerializationCallbackReceiver.OnAfterDeserialize() { }

            /// TODO: use Localization
            private static readonly Dictionary<FullScreenMode, string> windowModeLabels = new()
            {
                [FullScreenMode.ExclusiveFullScreen] = "Full Exclusive",
                [FullScreenMode.FullScreenWindow] = "Full Borderless",
                [FullScreenMode.MaximizedWindow] = "Maximized",
                [FullScreenMode.Windowed] = "Windowed"
            };
        }
    }
}
