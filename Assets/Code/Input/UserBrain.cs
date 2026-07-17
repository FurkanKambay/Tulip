using UnityEngine;
using UnityEngine.InputSystem;

namespace FK.Tulip.Input
{
    public class UserBrain : MonoBehaviour
    {
        [SerializeField] private InputActionReference pause;
        [SerializeField] private InputActionReference cancel;
        [SerializeField] private InputActionReference switchTab;

        public bool WantsToPause { get; private set; }
        public bool WantsToCancel { get; private set; }
        public int? TabSwitchDelta { get; private set; }

        private void Update()
        {
            WantsToPause = pause.action.triggered;
            WantsToCancel = cancel.action.triggered;
            TabSwitchDelta = switchTab.action.triggered ? (int)switchTab.action.ReadValue<float>() : null;
        }
    }
}
