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
        private Button quitButton;

        private void Awake()
        {
            settingsPresenter = FindAnyObjectByType<SettingsPresenter>();
            UpdateCallbacks(GameManager.CurrentState);
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
            root.visible = !visible;

        private void UpdateCallbacks(GameState newState)
        {
            bool inMainMenu = newState == GameState.MainMenu;

            document.enabled = inMainMenu;

            if (!inMainMenu)
            {
                newButton.UnregisterCallback<ClickEvent>(NewButton_Clicked);
                continueButton.UnregisterCallback<ClickEvent>(ContinueButton_Clicked);
                quitButton.UnregisterCallback<ClickEvent>(QuitButton_Clicked);
                return;
            }

            root = document.rootVisualElement;
            newButton = root.Q<Button>("new-game-button");
            continueButton = root.Q<Button>("continue-button");
            quitButton = root.Q<Button>("quit-button");

            newButton.RegisterCallback<ClickEvent>(NewButton_Clicked);
            continueButton.RegisterCallback<ClickEvent>(ContinueButton_Clicked);
            quitButton.RegisterCallback<ClickEvent>(QuitButton_Clicked);

            // TODO: enable continue button if there's a latest save
            // TODO: support multiple saves
            continueButton.SetEnabled(false);
        }

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
