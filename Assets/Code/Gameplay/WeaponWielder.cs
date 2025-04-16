using System;
using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class WeaponWielder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] ItemWielder itemWielder;
        [SerializeField] Inventory inventory;

        [Header("Config")]
        [SerializeField] ContactFilter2D hitContactFilter;
        [SerializeField] int maxMultiTargetAmount = 9;

        private WeaponData weaponData;
        private Collider2D[] hits = Array.Empty<Collider2D>();

        private void Attack(ItemStack stack, Vector3 targetPoint)
        {
            if (stack.itemData.IsNot(out weaponData))
                return;

            Array.Resize(ref hits, weaponData.IsMultiTarget ? maxMultiTargetAmount : 1);

            foreach (Health target in GetTargets(transform.position, targetPoint))
            {
                if (!target.enabled)
                    continue;

                InventoryModification loot = target.Damage(weaponData.Damage, health);

                if (inventory)
                    inventory.ApplyModification(loot);
            }
        }

        private IEnumerable<Health> GetTargets(Vector2 origin, Vector2 aimPoint)
        {
            Vector2 direction = (aimPoint - origin).normalized;

            var results = new RaycastHit2D[hits.Length];
            int hitCount = Physics2D.Raycast(origin, direction, hitContactFilter, results, weaponData.Range);
            hits = results.Select(hit => hit.collider).ToArray();

            Debug.DrawRay(origin, direction * weaponData.Range, Color.green, 1f);

            return hits
                .Take(hitCount)
                .TakeWhile(hit => (bool)hit)
                .Select(hit => hit.GetComponentInChildren<Health>())
                .TakeWhile(hitHealth => (bool)hitHealth);
        }

        private void OnEnable() => itemWielder.OnSwingPerform += Attack;
        private void OnDisable() => itemWielder.OnSwingPerform -= Attack;
    }
}
