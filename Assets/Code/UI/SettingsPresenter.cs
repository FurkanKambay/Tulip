using System.Collections;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Audio;
using FK.Tulip.Data.GameEvents;
using FMOD.Studio;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Settings = FK.Tulip.Core.Settings;

namespace FK.Tulip.UI
{
    public delegate void MenuToggleEvent(bool visible);

    public sealed class SettingsPresenter : MonoBehaviour
    {
        public event MenuToggleEvent OnToggled;

        [Header("Game Events")]
        [SerializeField, Required] private GameStateChangeEvent gameStateChangeEvent;

        [Header("References")]
        [SerializeField, Required] private PanelRenderer panelRenderer;
        [SerializeField, Required] private InputActionReference switchTabInput;
        [SerializeField, Required] private InputActionReference cancelInput;

        [Header("FMOD Events")]
        [SerializeField] private FMODEvent toggleSfx;

        [CreateProperty, UsedImplicitly] private Settings Settings => Settings.Instance;

        private int version;
        private VisualElement root;
        private TabView tabView;

        private PARAMETER_DESCRIPTION paramMenuState;

        private void OnEnable()
        {
            panelRenderer.RegisterUIReloadCallback(UI_Reloaded);

            switchTabInput.action.performed += SwitchTabInput_Performed;
            cancelInput.action.performed += CancelInput_Performed;
        }

        private void OnDisable()
        {
            panelRenderer.UnregisterUIReloadCallback(UI_Reloaded);

            switchTabInput.action.performed -= SwitchTabInput_Performed;
            cancelInput.action.performed -= CancelInput_Performed;
        }

        private void UI_Reloaded(PanelRenderer renderer, VisualElement rootElement, int version)
        {
            if (version == this.version)
                return;

            Log.Info($"[UI] Settings Menu reloading (version {version})");

            this.version = version;
            root = rootElement;
            root.dataSource = this;

            tabView = root.Q<TabView>();
            tabView.activeTabChanged += TabView_ActiveTabChanged;

#if UNITY_WEBGL
            root.Q<DropdownField>("display-resolution").RemoveFromHierarchy();
#endif
        }

        private IEnumerator Start()
        {
            yield return AudioManager.WaitForAllBanksToLoad();

            toggleSfx.Describe();
            toggleSfx.DescribeParameter("Menu State", out paramMenuState);
        }

        public void SetDisplayed(bool displayed)
        {
            if (panelRenderer.enabled == displayed)
                return;

            panelRenderer.enabled = displayed;

            PlayToggleSfx(displayed);
            OnToggled?.Invoke(displayed);

            // focus first setting field
            if (displayed)
            {
                // can't use ForceSetFocus() with PanelRenderer. just have to wait
                StartCoroutine(FocusFirstSettingInActiveTab_Delayed());
            }
        }

        private void SwitchTabInput_Performed(InputAction.CallbackContext context)
        {
            // bail if page not loaded yet
            if (tabView == null) return;

            // prevent tab switch if a field has a flyout open
            if (root.GetNavigationStatus(out _) is not UINavigationStatus.Locked)
                tabView.selectedTabIndex += (int)context.ReadValue<float>();
        }

        private void CancelInput_Performed(InputAction.CallbackContext context)
        {
            // close page only when: no element has focus / navigation isn't locked by focused element
            if (root.GetNavigationStatus(out _) is not UINavigationStatus.Locked)
                SetDisplayed(false);
        }

        private void TabView_ActiveTabChanged(Tab oldTab, Tab newTab)
        {
            // if a field has a flyout open, force it to close before switching tabs
            // preventing the tab from switching in the first place is done when reading the input
            if (root.GetNavigationStatus(out Focusable focusedElement) is UINavigationStatus.Locked)
                focusedElement.SendEvent(new NavigationCancelEvent());

            FocusFirstSettingInActiveTab();
        }

        private IEnumerator FocusFirstSettingInActiveTab_Delayed()
        {
            yield return null; // order is important
            yield return new WaitForEndOfFrame();

            FocusFirstSettingInActiveTab();
        }

        private void FocusFirstSettingInActiveTab() =>
            tabView?.activeTab.Q(className: "option")?.Focus();

        private void PlayToggleSfx(bool toggleState)
        {
            bool created = toggleSfx.CreateNew(out EventInstance sfx);
            if (!created) return;

            sfx.SetParameter(paramMenuState, toggleState);
            sfx.PlayOneShot();
        }
    }
}
