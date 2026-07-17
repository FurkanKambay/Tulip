using FK.Common;
using UnityEngine;

namespace FK.Tulip.Interaction
{
    public class Dialogue : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private Interactable interactable;

        [Header("Config")]
        [SerializeField, TextArea] private string greetingText;

        private void OnEnable() => interactable.OnInteract += HandleInteract;
        private void OnDisable() => interactable.OnInteract -= HandleInteract;

        private void HandleInteract() => Log.Info(greetingText);
    }
}
