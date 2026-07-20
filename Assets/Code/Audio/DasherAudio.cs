using FK.Tulip.Character;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class DasherAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Dasher dasher;

        [Header("FMOD Events")]
        [SerializeField] private FMODEvent dash;

        private void Awake()
        {
            dash.Describe();
        }

        private void OnEnable() => dasher.OnDash += Dasher_Dashed;
        private void OnDisable() => dasher.OnDash -= Dasher_Dashed;

        private void Dasher_Dashed() => PlayFootstep();

        private void PlayFootstep()
        {
            bool created = dash.CreateNew(out EventInstance sfx);
            if (!created) return;

            sfx.set3DAttributes(transform.To3DAttributes());
            sfx.PlayOneShot();
        }
    }
}
