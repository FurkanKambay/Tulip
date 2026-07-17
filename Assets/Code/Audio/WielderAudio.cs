using FK.Tulip.Data.Items;
using FK.Tulip.Gameplay;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    public class WielderAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemWielder itemWielder;

        [Header("FMOD Events")]
        [SerializeField] private EventReference itemSwingEvent;

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

        private void HandleItemSwing(ItemAsset item, Vector3 _) =>
            RuntimeManager.PlayOneShotAttached(itemSwingEvent, transform.gameObject);
    }
}
