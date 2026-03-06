using System;

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
