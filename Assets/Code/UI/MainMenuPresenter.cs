using System.Collections;
using FK.Common;
using FK.Tulip.Core;
using FK.Tulip.Data.GameEvents;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

namespace FK.Tulip.UI
{
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [Header("Game Events")]
        [SerializeField, Required] private GameStateChangeEvent gameStateChangeEvent;

        [Header("References")]
        [SerializeField, Required] private UIDocument document;

        private SettingsPresenter settingsPresenter;
        private VisualElement root;
        private Button newButton;
        private Button continueButton;
        private Button settingsButton;
        private Button quitButton;

        private void Awake()
        {
            settingsPresenter = FindAnyObjectByType<SettingsPresenter>();
            UpdateCallbacks(GameManager.CurrentState);
            SetMainMenuVisible(true);
        }

        private void OnEnable()
        {
            gameStateChangeEvent.OnRaised += GameState_Changed;
            settingsPresenter.OnToggled += Settings_Shown;
        }

        private void OnDisable()
        {
            gameStateChangeEvent.OnRaised -= GameState_Changed;
            settingsPresenter.OnToggled -= Settings_Shown;
        }

        private void Settings_Shown(bool visible) =>
            SetMainMenuVisible(!visible);

        private void UpdateCallbacks(GameState newState)
        {
            bool inMainMenu = newState == GameState.MainMenu;

            document.enabled = inMainMenu;

            if (!inMainMenu)
            {
                newButton.UnregisterCallback<ClickEvent>(NewButton_Clicked);
                newButton.UnregisterCallback<NavigationSubmitEvent>(NewButton_NavigationSubmitted);

                continueButton.UnregisterCallback<ClickEvent>(ContinueButton_Clicked);
                continueButton.UnregisterCallback<NavigationSubmitEvent>(ContinueButton_NavigationSubmitted);

                settingsButton.UnregisterCallback<ClickEvent>(SettingsButton_Clicked);
                settingsButton.UnregisterCallback<NavigationSubmitEvent>(SettingsButton_NavigationSubmitted);

                quitButton.UnregisterCallback<ClickEvent>(QuitButton_Clicked);
                quitButton.UnregisterCallback<NavigationSubmitEvent>(QuitButton_NavigationSubmitted);
                return;
            }

            root = document.rootVisualElement;
            newButton = root.Q<Button>("new-game-button");
            continueButton = root.Q<Button>("continue-button");
            settingsButton = root.Q<Button>("settings-button");
            quitButton = root.Q<Button>("quit-button");

            newButton.RegisterCallback<ClickEvent>(NewButton_Clicked);
            newButton.RegisterCallback<NavigationSubmitEvent>(NewButton_NavigationSubmitted);

            continueButton.RegisterCallback<ClickEvent>(ContinueButton_Clicked);
            continueButton.RegisterCallback<NavigationSubmitEvent>(ContinueButton_NavigationSubmitted);

            settingsButton.RegisterCallback<ClickEvent>(SettingsButton_Clicked);
            settingsButton.RegisterCallback<NavigationSubmitEvent>(SettingsButton_NavigationSubmitted);

            quitButton.RegisterCallback<ClickEvent>(QuitButton_Clicked);
            quitButton.RegisterCallback<NavigationSubmitEvent>(QuitButton_NavigationSubmitted);

            // TODO: enable continue button if there's a latest save
            // TODO: support multiple saves
            continueButton.SetEnabled(false);
        }

        private void SetMainMenuVisible(bool visible)
        {
            root.visible = visible;

            if (visible)
                StartCoroutine(FocusFirstButtonNextFrame());
        }

        private IEnumerator FocusFirstButtonNextFrame()
        {
            yield return new WaitForEndOfFrame();
            newButton.Focus();
        }

        private void NewButton_NavigationSubmitted(NavigationSubmitEvent evt) => NewButton_Clicked(null);
        private void ContinueButton_NavigationSubmitted(NavigationSubmitEvent evt) => ContinueButton_Clicked(null);
        private void SettingsButton_NavigationSubmitted(NavigationSubmitEvent evt) => SettingsButton_Clicked(null);
        private void QuitButton_NavigationSubmitted(NavigationSubmitEvent _) => QuitButton_Clicked(null);

        private void NewButton_Clicked(ClickEvent _)
        {
            DisableButtons();
            newButton.text = "Generating World";

            // TODO: handle the audio elsewhere?
            RuntimeManager.CoreSystem.mixerSuspend();

            GameManager.StartNewGame();

            RuntimeManager.CoreSystem.mixerResume();
        }

        private void ContinueButton_Clicked(ClickEvent _)
        {
            DisableButtons();
            continueButton.text = "Loading World";

            RuntimeManager.CoreSystem.mixerSuspend();

            Log.Warning("TODO: Continue Game", this);

            RuntimeManager.CoreSystem.mixerResume();
        }

        private void SettingsButton_Clicked(ClickEvent _) =>
            settingsPresenter.SetUIVisible(true);

        private void QuitButton_Clicked(ClickEvent _) =>
            GameManager.QuitGame();

        private void DisableButtons()
        {
            newButton.SetEnabled(false);
            continueButton.SetEnabled(false);
        }

        private void GameState_Changed(GameStateChange args) =>
            UpdateCallbacks(args.NewState);
    }
}
