using Furkan.Common;
using UnityEngine;

namespace Tulip.Interaction
{
    public class Dialogue : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Interactable interactable;

        [Header("Config")]
        [SerializeField, TextArea] string greetingText;

        private void OnEnable() => interactable.OnInteract += HandleInteract;
        private void OnDisable() => interactable.OnInteract -= HandleInteract;

        private void HandleInteract() => Log.Info(greetingText);
    }
}
