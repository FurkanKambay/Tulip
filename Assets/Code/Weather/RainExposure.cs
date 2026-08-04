using FK.Common.Extensions;
using UnityEngine;

namespace FK.Tulip.Weather
{
    public class RainExposure : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private RainExposureSpeedAsset rainExposureSpeedAsset;

        [Header("State")]
        [SerializeField, Min(0)] private float exposedTime;
        [SerializeField, Range(0, 100)] private float exposure;

        private IRainDetector rainDetector;

        public void Init(IRainDetector rainDetector)
        {
            this.rainDetector = rainDetector;
        }

        private void Start()
        {
            if (rainDetector.Missing() && !TryGetComponent(out rainDetector))
                throw new MissingComponentException("Rain detector not found!");
        }

        private void Update()
        {
            RainExposureLevel level = rainDetector.RainExposureLevel;

            if (level is RainExposureLevel.None)
                exposedTime = 0;
            else
                exposedTime += Time.deltaTime;

            float exposureSpeed = rainExposureSpeedAsset.GetExposureSpeedAt(level);
            exposure += exposureSpeed * Time.deltaTime;
            exposure = Mathf.Clamp(exposure, 0, 100);
        }
    }
}
