using JetBrains.Annotations;
using UnityEngine;

namespace FK.Common.Extensions
{
    [PublicAPI]
    public static class LogColorExtensions
    {
        public static string Color<T>(this T value)
        {
            (string color, string label) = value switch
            {
                GameObject go => ("teal", go.name),
                Transform t => ("green", t.name),
                _ => ("white", value.ToString())
            };

            return $"<color={color}>{label}</color>";
        }
    }
}
