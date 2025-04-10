using SaintsField;
using Tulip.Character;
using UnityEngine;

namespace Tulip.UI
{
    public sealed class HealthBarPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SpriteRenderer healthBarSprite;

        [Header("Config")]
        [SerializeField] float changeSpeed = 10f;
        [SerializeField] bool showBar = true;

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
