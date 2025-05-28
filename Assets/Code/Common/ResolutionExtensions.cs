using JetBrains.Annotations;
using UnityEngine;

namespace Furkan.Common
{
    [PublicAPI]
    public static class ResolutionExtensions
    {
        public static string AsString(this Resolution resolution) =>
            $"{resolution.width} x {resolution.height}";

        public static Resolution AsResolution(this string self)
        {
            if (self == null || !self.Contains('x'))
                return default;

            string[] resolutionParts = self.Split('x', 2);
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);

            return new Resolution
            {
                width = width,
                height = height
            };
        }
    }
}
