using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class UserBrain : MonoBehaviour, IUserBrain
    {
        [SerializeField] InputActionReference pause;
        [SerializeField] InputActionReference cancel;
        [SerializeField] InputActionReference switchTab;

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
