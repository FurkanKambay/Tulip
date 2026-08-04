using System;
using UnityEngine;

namespace FK.Tulip.Weather
{
    [CreateAssetMenu(menuName = "Weather/Rain Exposure Speeds")]
    public class RainExposureSpeedAsset : ScriptableObject
    {
        [SerializeField] private float[] exposureSpeeds;

        public float GetExposureSpeedAt(RainExposureLevel level)
        {
            int index = (int)level;
            if (index < 0 || index >= exposureSpeeds.Length)
                throw new ArgumentOutOfRangeException(nameof(level));

            return exposureSpeeds[index];
        }

        private void OnValidate()
        {
            Array.Resize(ref exposureSpeeds, (int)RainExposureLevel.Maximum + 1);
        }
    }
}
