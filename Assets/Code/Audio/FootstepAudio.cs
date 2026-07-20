using FK.Tulip.Character;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class FootstepAudio : MonoBehaviour
    {
        [Header("Brain")]
        [SerializeField] private CharacterMovement movement;

        [Header("References")]
        [SerializeField] private SurroundsChecker surrounds;

        [Header("FMOD Events")]
        [SerializeField] private FMODEvent footsteps;

        [Header("Config")]
        [SerializeField, Min(0.01f)] private float footStepInterval;
        [SerializeField, Min(0)] private float velocityThreshold;

        private PARAMETER_DESCRIPTION paramGroundMaterial;

        private float timeUntilFootstep;

        private void Awake()
        {
            footsteps.Describe();
            footsteps.DescribeParameter("Ground Material", out paramGroundMaterial);
        }

        private void Update()
        {
            timeUntilFootstep -= Time.deltaTime;

            float velocity = Mathf.Abs(movement.Velocity.x);
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
            bool created = footsteps.CreateNew(out EventInstance sfx);
            if (!created) return;

            sfx.set3DAttributes(transform.To3DAttributes());

            sfx.SetParameter(paramGroundMaterial, surrounds.GroundMaterial);
            sfx.PlayOneShot();
        }
    }
}
