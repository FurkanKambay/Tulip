using Furkan.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Core
{
    public sealed class InputManager : MonoBehaviour
    {
        [SerializeField, Required] InputActionAsset inputActionAsset;

        private InputActionMap heroControls;
        private InputActionMap uiControls;

        private void Awake()
        {
            heroControls = inputActionAsset.FindActionMap("Hero");
            uiControls = inputActionAsset.FindActionMap("UI");

            ActivateHeroControls();
        }

        internal void ActivateHeroControls()
        {
            heroControls.Enable();
            uiControls.Disable();
        }

        internal void ActivateUIControls()
        {
            heroControls.Disable();
            uiControls.Enable();
        }
    }
}
