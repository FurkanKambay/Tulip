using System;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Settings
    {
        [Serializable]
        public sealed record GameplaySettingsBag
        {
            internal GameplaySettingsBag()
            {
            }
        }
    }
}
