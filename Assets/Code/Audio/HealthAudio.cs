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
        [SerializeField, Required] Health health;

        [Header("FMOD Events")]
        [SerializeField] EventReference hurtEvent;

        private PARAMETER_DESCRIPTION paramAliveness;
        private PARAMETER_DESCRIPTION paramDamageType;

        private async void Awake()
        {
            await AudioBusManager.WaitForAllBanksToLoad();

            EventDescription description = RuntimeManager.GetEventDescription(hurtEvent);
            description.getParameterDescriptionByName("Aliveness", out paramAliveness);
            description.getParameterDescriptionByName("Damage Type", out paramDamageType);
        }

        private void OnEnable() => health.OnHurt += HandleHurt;
        private void OnDisable() => health.OnHurt -= HandleHurt;

        private void HandleHurt(CombatPacket combatPacket)
        {
            EventInstance hurtSfx = RuntimeManager.CreateInstance(hurtEvent);
            RuntimeManager.AttachInstanceToGameObject(hurtSfx, transform.gameObject);

            hurtSfx.setParameterByID(paramAliveness.id, combatPacket.Target.IsAlive.GetHashCode());
            hurtSfx.setParameterByID(paramDamageType.id, combatPacket.DamageType.GetHashCode());

            hurtSfx.start();
            hurtSfx.release();
        }
    }
}
