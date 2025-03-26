using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.Core;
using Tulip.GameWorld;
using Tulip.Input;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Settings = Tulip.Core.Settings;

namespace Tulip.UI
{
    public delegate void MenuToggleEvent(bool visible);

    public class SettingsPresenter : MonoBehaviour
    {
        public event MenuToggleEvent OnToggled;

        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] UserBrain brain;
        [SerializeField, Required] WorldManager worldManager;

        [Header("FMOD Events")]
        [SerializeField] EventReference toggleSfx;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] Settings Settings => Settings.Instance;
        [CreateProperty] bool IsQuitConfirmButtonVisible => IsInMainMenu && ShouldShowQuitButton;
        [CreateProperty] bool IsSaveExitButtonVisible => !IsInMainMenu && ShouldShowQuitButton;
        // ReSharper restore UnusedMember.Local

        private static bool IsInMainMenu => GameManager.CurrentState == GameState.MainMenu;
        private bool ShouldShowQuitButton => container.visible && quitFlyoutButton.value;

        private VisualElement root;
        private VisualElement container;
        private TabView tabView;

        private Toggle optionsButton;
        private Toggle quitFlyoutButton;
        private Button menuQuitButton;
        private Button gameExitButton;

        private PARAMETER_DESCRIPTION paramMenuState;

        private void Awake()
        {
            document.enabled = true;
            root = document.rootVisualElement;

            container = root.Q<VisualElement>("MainContainer");
            container.visible = false;
            container.dataSource = this;

            tabView = root.Q<TabView>();
            optionsButton = root.Q<Toggle>("OptionsButton");
            quitFlyoutButton = root.Q<Toggle>("QuitFlyoutButton");
            gameExitButton = root.Q<Button>("SaveExitButton");
            menuQuitButton = root.Q<Button>("QuitConfirmButton");

            optionsButton.RegisterCallback<ChangeEvent<bool>>(OptionsButton_Toggled);
            gameExitButton.RegisterCallback<ClickEvent>(SaveExitButton_Clicked);
            menuQuitButton.RegisterCallback<ClickEvent>(QuitButton_Clicked);

#if UNITY_WEBGL
            root.Q<DropdownField>("VideoResolution").RemoveFromHierarchy();
#endif
        }

        private async void Start()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                await Awaitable.NextFrameAsync();

            EventDescription sfxDescription = RuntimeManager.GetEventDescription(toggleSfx);
            sfxDescription.getParameterDescriptionByName("Menu State", out paramMenuState);
        }

        private void OnEnable()
        {
            root.visible = true;
            container.visible = false;

            GameManager.OnGameStateChange += HandleGameStateChange;
        }

        private void OnDisable()
        {
            root.visible = false;
            container.visible = false;

            GameManager.OnGameStateChange -= HandleGameStateChange;
        }

        private void Update()
        {
            // TODO: rewrite this in a better way
            if (GameManager.CurrentState == GameState.MainMenu)
            {
                // same as <cancel.action.triggered> in Main Menu
                if (brain.WantsToMenu)
                    optionsButton.value = !optionsButton.value;
            }
            else
            {
                if (brain.WantsToMenu)
                    optionsButton.value = true;

                if (brain.WantsToCancel)
                    optionsButton.value = false;
            }

            if (brain.TabSwitchDelta.HasValue)
                tabView.selectedTabIndex += brain.TabSwitchDelta.Value;
        }

        private void OptionsButton_Toggled(ChangeEvent<bool> change)
        {
            container.visible = change.newValue;
            quitFlyoutButton.value = false;

            PlayToggleSfx(change.newValue);
            GameManager.SetPaused(change.newValue);

            OnToggled?.Invoke(change.newValue);
        }

        private void PlayToggleSfx(bool toggleState)
        {
            EventInstance sfx = RuntimeManager.CreateInstance(toggleSfx);
            sfx.setParameterByID(paramMenuState.id, toggleState.GetHashCode());
            sfx.start();
            sfx.release();
        }

        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            root.visible = newState != GameState.Playing;
        }

        private void SaveExitButton_Clicked(ClickEvent _)
        {
            SaveGame();
            quitFlyoutButton.value = false;
            optionsButton.value = false;

            worldManager.ReturnToMainMenu();
        }

        private void QuitButton_Clicked(ClickEvent _) => GameManager.QuitGame();

        // TODO: save game
        private void SaveGame() => Debug.Log("Saving...");
    }
}
