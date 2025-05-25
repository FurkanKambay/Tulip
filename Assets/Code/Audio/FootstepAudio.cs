using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Audio
{
    public class FootstepAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SaintsInterface<Component, ICharacterMovement> movement;
        [SerializeField] SurroundsChecker surrounds;

        [Header("FMOD Events")]
        [SerializeField] EventReference footstepEvent;

        [Header("Config")]
        [SerializeField, Min(0.01f)] float footStepInterval;
        [SerializeField, Min(0)] float velocityThreshold;

        private EventDescription footstepDescription;
        private PARAMETER_DESCRIPTION paramGroundMaterial;

        private EventInstance footstepInstance;

        private float timeUntilFootstep;

        private void Awake()
        {
            footstepDescription = RuntimeManager.GetEventDescription(footstepEvent);
            footstepDescription.getParameterDescriptionByName("Ground Material", out paramGroundMaterial);
        }

        private void Update()
        {
            timeUntilFootstep -= Time.deltaTime;

            float velocity = Mathf.Abs(movement.I.Velocity.x);
            // TODO: match footstep interval with feet movement (animation too)

            // TODO: different sfx when move direction changes
            if (timeUntilFootstep <= 0 && velocity > velocityThreshold)
            {
                PlayFootstep();
                timeUntilFootstep = footStepInterval;
            }
        }

        private void PlayFootstep()
        {
            footstepDescription.createInstance(out footstepInstance);

            footstepInstance.set3DAttributes(transform.To3DAttributes());
            footstepInstance.setParameterByID(paramGroundMaterial.id, surrounds.GroundMaterial.GetHashCode());

            footstepInstance.start();
            footstepInstance.release();
        }
    }
}
