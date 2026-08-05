using FK.Tulip.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace FK.Tulip.Weather
{
    public class RainController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private ValleyConfigAsset valleyConfig;
        [SerializeField] private VisualEffect rainVFX;

        private void Awake()
        {
            Assert.IsNotNull(valleyConfig);

            if (!rainVFX) rainVFX = GetComponent<VisualEffect>();
            Assert.IsNotNull(rainVFX);

            UpdateVFXRainAngle();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateVFXRainAngle();
        }
#endif

        private void UpdateVFXRainAngle()
        {
            if (rainVFX)
                rainVFX.SetFloat(ShaderParams.Angle, valleyConfig.RainAngle);
        }

        private static class ShaderParams
        {
            internal static readonly int Angle = Shader.PropertyToID("Angle");
        }
    }
}
