using System.Collections.Generic;
using Tulip.Character;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody2D body;

        [Header("State")]
        [SerializeField] Health ownerHealth;
        [SerializeField] WeaponData sourceWeapon;
        [SerializeField] List<Transform> damagedTargets;

        public Rigidbody2D Body => body;

        public Health OwnerHealth
        {
            get => ownerHealth;
            set => ownerHealth = value;
        }

        public WeaponData SourceWeapon
        {
            get => sourceWeapon;
            set => sourceWeapon = value;
        }

        public List<Transform> DamagedTargets => damagedTargets;

        public void Destroy()
        {
            // TODO: disable and reset state instead (for pooling)
            damagedTargets.Clear();

            Destroy(gameObject);
        }
    }
}
