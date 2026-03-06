using FMODUnity;
using Furkan.Common;
using SaintsField;
using Tulip.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;

        [Header("Events")]
        [SerializeField, Required] GameEvent newGameEvent;
        [SerializeField, Required] GameEvent continueGameEvent;

        private SettingsPresenter settingsPresenter;
        private VisualElement root;
        private Button newButton;
        private Button continueButton;

        private void Awake()
        {
            settingsPresenter = FindAnyObjectByType<SettingsPresenter>();
            UpdateCallbacks(GameManager.CurrentState);
        }

        private void OnEnable()
        {
            GameStateChange.Event += GameState_Changed;
            settingsPresenter.OnToggled += Settings_Shown;
        }

        private void OnDisable()
        {
            GameStateChange.Event -= GameState_Changed;
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
                return;
            }

            root = document.rootVisualElement.ElementAt(0);
            newButton = root.Q<Button>("new-game-button");
            continueButton = root.Q<Button>("continue-button");

            newButton.RegisterCallback<ClickEvent>(NewButton_Clicked);
            continueButton.RegisterCallback<ClickEvent>(ContinueButton_Clicked);

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
            newGameEvent.Raise();
            RuntimeManager.CoreSystem.mixerResume();
        }

        private void ContinueButton_Clicked(ClickEvent _)
        {
            DisableButtons();
            continueButton.text = "Loading World";

            RuntimeManager.CoreSystem.mixerSuspend();
            continueGameEvent.Raise();
            RuntimeManager.CoreSystem.mixerResume();
        }

        private void DisableButtons()
        {
            newButton.SetEnabled(false);
            continueButton.SetEnabled(false);
        }

        private void GameState_Changed(GameStateChange args) =>
            UpdateCallbacks(args.NewState);
    }
}
