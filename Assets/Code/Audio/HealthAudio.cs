using FK.Common;
using FK.Tulip.Combat;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class HealthAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private Health health;

        [Header("FMOD Events")]
        [SerializeField] private FMODEvent getHurt;

        private PARAMETER_DESCRIPTION paramAliveness;
        private PARAMETER_DESCRIPTION paramDamageType;

        private async void Awake()
        {
            await AudioManager.WaitForAllBanksToLoad();

            getHurt.Describe();
            getHurt.DescribeParameter("Aliveness", out paramAliveness);
            getHurt.DescribeParameter("Damage Type", out paramDamageType);
        }

        private void OnEnable() => health.OnHurt += HandleHurt;
        private void OnDisable() => health.OnHurt -= HandleHurt;

        private void HandleHurt(CombatPacket combatPacket)
        {
            bool success = getHurt.CreateNew(out EventInstance sfx);
            if (!success) return;

            RuntimeManager.AttachInstanceToGameObject(sfx, transform.gameObject);

            sfx.SetParameter(paramAliveness, combatPacket.Target.IsAlive);
            sfx.SetParameter(paramDamageType, combatPacket.DamageType);
            sfx.PlayOneShot();
        }
    }
}
