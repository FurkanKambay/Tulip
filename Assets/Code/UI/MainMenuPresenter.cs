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
                newButton.clicked -= NewButton_Clicked;
                continueButton.clicked -= ContinueButton_Clicked;
                settingsButton.clicked -= SettingsButton_Clicked;
                quitButton.clicked -= QuitButton_Clicked;
                return;
            }

            root = document.rootVisualElement;
            newButton = root.Q<Button>("new-game-button");
            continueButton = root.Q<Button>("continue-button");
            settingsButton = root.Q<Button>("settings-button");
            quitButton = root.Q<Button>("quit-button");

            newButton.clicked += NewButton_Clicked;
            continueButton.clicked += ContinueButton_Clicked;
            settingsButton.clicked += SettingsButton_Clicked;
            quitButton.clicked += QuitButton_Clicked;

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

        private void NewButton_Clicked()
        {
            DisableButtons();
            newButton.text = "Generating World";

            // TODO: handle the audio elsewhere?
            RuntimeManager.CoreSystem.mixerSuspend();

            GameManager.StartNewGame();

            RuntimeManager.CoreSystem.mixerResume();
        }

        private void ContinueButton_Clicked()
        {
            DisableButtons();
            continueButton.text = "Loading World";

            RuntimeManager.CoreSystem.mixerSuspend();

            Log.Warning("TODO: Continue Game", this);

            RuntimeManager.CoreSystem.mixerResume();
        }

        private void SettingsButton_Clicked() =>
            settingsPresenter.SetUIVisible(true);

        private void QuitButton_Clicked() =>
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
