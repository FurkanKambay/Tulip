using FMODUnity;
using Tulip.Data;
using Tulip.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class WielderAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ItemWielder itemWielder;

        [Header("FMOD Events")]
        [SerializeField] EventReference itemSwingEvent;

        private void OnEnable()
        {
            if (itemWielder)
                itemWielder.OnSwingPerform += HandleItemSwing;
        }

        private void OnDisable()
        {
            if (itemWielder)
                itemWielder.OnSwingPerform -= HandleItemSwing;
        }

        private void HandleItemSwing(ItemStack stack, Vector3 _) =>
            RuntimeManager.PlayOneShotAttached(itemSwingEvent, transform.gameObject);
    }
}
