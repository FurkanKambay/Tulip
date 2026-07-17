using System.Collections;
using FK.Common;
using FK.Tulip.Combat;
using UnityEngine;

namespace FK.Tulip.Character
{
    public class CharacterHealthEffects : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SpriteRenderer sprite;

        [Header("Config")]
        [SerializeField] bool destroyAfterDeath;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float hurtVisualDuration = 0.1f;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float dissolveDuration = 1f;

        private MaterialPropertyBlock materialBlock;

        private void OnEnable()
        {
            materialBlock = new MaterialPropertyBlock();

            health.OnHurt += HandleHurt;
            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private async void HandleHurt(CombatPacket combatPacket)
        {
            if (hurtVisualDuration > 0)
            {
                ApplyShaderProperty(ShaderParams.ReplaceColor, 1);
                await Awaitable.WaitForSecondsAsync(hurtVisualDuration);
                ApplyShaderProperty(ShaderParams.ReplaceColor, 0);
            }

            ApplyShaderProperty(ShaderParams.OutlineThickness, 1);
            await Awaitable.WaitForSecondsAsync(health.InvulnerabilityDuration - hurtVisualDuration);
            ApplyShaderProperty(ShaderParams.OutlineThickness, 0);
        }

        private void HandleRevived(Health reviver)
        {
            sprite.sortingOrder = 0;

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration / 2f, 1, 0));
        }

        private void HandleDied(CombatPacket combatPacket)
        {
            sprite.sortingOrder = -1;

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration));
        }

        private IEnumerator DissolveSprite(float duration, float startAmount = 0, float endAmount = 1)
        {
            float amount = startAmount;

            while (!Mathf.Approximately(amount, endAmount))
            {
                amount = Mathf.MoveTowards(amount, endAmount, Time.deltaTime / duration);

                ApplyShaderProperty(ShaderParams.DissolveAmount, amount);
                yield return null;
            }

            if (destroyAfterDeath)
                Destroy(health.transform.parent.gameObject);
        }

        private void ApplyShaderProperty(int shaderProperty, float value)
        {
            materialBlock.SetFloat(shaderProperty, value);
            sprite.SetPropertyBlock(materialBlock);
        }

        private static class ShaderParams
        {
            internal static readonly int ReplaceColor = Shader.PropertyToID("_ReplaceColor");
            internal static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
            internal static readonly int OutlineThickness = Shader.PropertyToID("_Outline_Thickness");
        }
    }
}
