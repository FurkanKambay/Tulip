using System.Collections.Generic;
using Tulip.Character;
using UnityEngine;

namespace Tulip.Data
{
    public class StatusEffector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Health health;

        [Header("Config")]
        [SerializeField] StatusEffectSO[] startingEffects;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<StatusEffect> effects = new();

        private void Awake()
        {
            foreach (StatusEffectSO effect in startingEffects)
                effects.Add(effect.Create(health, health));
        }

        private void Update()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i] is not { } effect)
                    return;

                effect.Tick(Time.deltaTime);

                if (effect.IsDone)
                    effects.RemoveAt(i);
            }
        }
    }
}
