using System.Collections;
using FK.Common;
using FK.Tulip.Audio;
using FK.Tulip.Core;
using FK.Tulip.Data.GameEvents;
using FK.Tulip.Input;
using FMOD.Studio;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;
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
        [SerializeField, Required] private UIDocument document;
        [SerializeField, Required] private UserBrain brain;

        [Header("FMOD Events")]
        [SerializeField] private FMODEvent toggleSfx;

        [CreateProperty, UsedImplicitly] private Settings Settings => Settings.Instance;
        [CreateProperty, UsedImplicitly] private bool IsQuitButtonVisible => !IsInMainMenu && container.visible;
        [CreateProperty, UsedImplicitly] private bool IsSaveExitButtonVisible => !IsInMainMenu && ShouldShowQuitButton;

        private static bool IsInMainMenu => GameManager.CurrentState == GameState.MainMenu;
        private bool ShouldShowQuitButton => container.visible && quitFlyoutButton.value;

        private VisualElement root;
        private VisualElement container;
        private TabView tabView;

        private Toggle optionsButton;
        private Toggle quitFlyoutButton;
        private Button gameExitButton;

        private PARAMETER_DESCRIPTION paramMenuState;

        private void Awake()
        {
            document.enabled = true;
            root = document.rootVisualElement;

            container = root.Q<VisualElement>("options-menu");
            container.visible = false;
            container.dataSource = this;

            tabView = root.Q<TabView>();
            optionsButton = root.Q<Toggle>("options-toggle");
            quitFlyoutButton = root.Q<Toggle>("quit-flyout-button");
            gameExitButton = root.Q<Button>("save-exit-button");

            optionsButton.RegisterCallback<ChangeEvent<bool>>(OptionsButton_Toggled);

            gameExitButton.clicked += SaveExitButton_Clicked;

            tabView.RegisterCallback<NavigationCancelEvent>(TabView_NavigationCanceled);
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

        private void OnEnable()
        {
            root.visible = true;
            container.visible = false;

            gameStateChangeEvent.OnRaised += GameState_Changed;
        }

        private void OnDisable()
        {
            root.visible = false;
            container.visible = false;

            gameStateChangeEvent.OnRaised -= GameState_Changed;
        }

        private void Update()
        {
            if (brain.WantsToPause)
                SetUIVisible(true);

            if (brain.TabSwitchDelta.HasValue)
            {
                // prevent tab switch if a field has a flyout open
                if (tabView.activeTab.focusController.focusedElement is not { delegatesFocus: false })
                    tabView.selectedTabIndex += brain.TabSwitchDelta.Value;
            }
        }

        public void SetUIVisible(bool visible) =>
            optionsButton.value = visible;

        private void TabView_NavigationCanceled(NavigationCancelEvent evt) => SetUIVisible(false);

        private void TabView_ActiveTabChanged(Tab oldTab, Tab newTab)
        {
            // if a field has a flyout open, force it to close before switching tabs
            // preventing the tab from switching in the first place is done when reading the input
            if (oldTab.focusController.focusedElement is { delegatesFocus: false } focusedElement)
                focusedElement.SendEvent(new NavigationCancelEvent());

            FocusFirstFieldOfActiveTab();
        }

        private void OptionsButton_Toggled(ChangeEvent<bool> change)
        {
            container.visible = change.newValue;
            quitFlyoutButton.value = false;

            if (container.visible)
                StartCoroutine(FocusFirstFieldOfActiveTabNextFrame());

            PlayToggleSfx(change.newValue);
            GameManager.SetPaused(change.newValue);

            OnToggled?.Invoke(change.newValue);
        }

        private void FocusFirstFieldOfActiveTab() =>
            tabView.activeTab.Q(className: "option").Focus();

        private IEnumerator FocusFirstFieldOfActiveTabNextFrame()
        {
            yield return new WaitForEndOfFrame();
            FocusFirstFieldOfActiveTab();
        }

        private void PlayToggleSfx(bool toggleState)
        {
            bool created = toggleSfx.CreateNew(out EventInstance sfx);
            if (!created) return;

            sfx.SetParameter(paramMenuState, toggleState);
            sfx.PlayOneShot();
        }

        private void GameState_Changed(GameStateChange args)
        {
            root.visible = args.NewState != GameState.Playing;
        }

        private void SaveExitButton_Clicked()
        {
            SaveGame();
            quitFlyoutButton.value = false;
            optionsButton.value = false;

            GameManager.ReturnToMainMenu();
        }

        // TODO: save game
        private void SaveGame() => Log.Info("Saving...");
    }
}
