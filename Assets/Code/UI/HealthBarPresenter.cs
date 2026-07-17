using FK.Common;
using FK.Tulip.Combat;
using UnityEngine;

namespace FK.Tulip.UI
{
    public sealed class HealthBarPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private Health health;
        [SerializeField, Required] private SpriteRenderer healthBarSprite;

        [Header("Config")]
        [SerializeField] private float changeSpeed = 10f;
        [SerializeField] private bool showBar = true;

        private float targetValue;

        private void Awake()
        {
            healthBarSprite.enabled = false;
            targetValue = health.MaxHealth;
        }

        private void Update()
        {
            healthBarSprite.enabled = showBar && health.IsHurt;
            if (!showBar) return;

            targetValue = Mathf.Lerp(targetValue, health.Ratio, changeSpeed * Time.deltaTime);
            healthBarSprite.material.SetFloat(ShaderParams.Value, targetValue);
        }

        private static class ShaderParams
        {
            internal static readonly int Value = Shader.PropertyToID("_Value");
        }
    }
}
