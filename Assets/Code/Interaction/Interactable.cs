using System;
using FK.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FK.Tulip.Interaction
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Interactable : MonoBehaviour
    {
        public event Action OnInteract;

        [Header("Input")]
        [SerializeField] InputActionReference point;
        [SerializeField] InputActionReference interact;

        [Header("References")]
        [SerializeField] BoxCollider2D trigger;

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        // TODO: highlight sprite outline
        private void Update()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(point.action.ReadValue<Vector2>());
            bool isHovering = trigger.OverlapPoint(mouseWorld);

            if (!isHovering) return;

            if (interact.action.triggered)
                OnInteract?.Invoke();
        }

        private void OnValidate()
        {
            if (!trigger || !trigger.isTrigger)
                Log.Error("Interaction trigger box is invalid!", trigger);
        }
    }
}
