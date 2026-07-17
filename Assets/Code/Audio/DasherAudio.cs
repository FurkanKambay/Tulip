using FK.Tulip.Character;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class DasherAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Dasher dasher;

        [Header("FMOD Events")]
        [SerializeField] EventReference dashEvent;

        private EventDescription dashDescription;
        private EventInstance dashInstance;

        private float timeUntilFootstep;

        private void Awake() => dashDescription = RuntimeManager.GetEventDescription(dashEvent);

        private void OnEnable() => dasher.OnDash += Dasher_Dashed;
        private void OnDisable() => dasher.OnDash -= Dasher_Dashed;
        private void Dasher_Dashed() => PlayFootstep();

        private void PlayFootstep()
        {
            dashDescription.createInstance(out dashInstance);
            dashInstance.set3DAttributes(transform.To3DAttributes());
            dashInstance.start();
            dashInstance.release();
        }
    }
}
