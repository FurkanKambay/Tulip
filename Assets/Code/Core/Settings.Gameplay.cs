using System;

namespace FK.Tulip.Core
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
