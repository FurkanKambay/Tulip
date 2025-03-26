using FMODUnity;
using SaintsField;
using Tulip.Core;
using Tulip.GameWorld;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] WorldManager worldManager;
        [SerializeField, Required] SettingsPresenter settingsPresenter;

        private VisualElement root;
        private Button newButton;
        private Button continueButton;

        private void Awake() =>
            UpdateCallbacks(GameManager.CurrentState);

        private void OnEnable()
        {
            GameManager.OnGameStateChange += GameManager_StateChanged;
            settingsPresenter.OnToggled += Settings_Shown;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChange -= GameManager_StateChanged;
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
            newButton = root.Q<Button>("NewButton");
            continueButton = root.Q<Button>("ContinueButton");
            newButton.RegisterCallback<ClickEvent>(NewButton_Clicked);
            continueButton.RegisterCallback<ClickEvent>(ContinueButton_Clicked);

            bool worldExists = worldManager.CanLoadWorld();
            continueButton.SetEnabled(worldExists);
        }

        private void NewButton_Clicked(ClickEvent _)
        {
            DisableButtons();
            newButton.text = "Generating World";

            RuntimeManager.CoreSystem.mixerSuspend();

            worldManager.DeleteWorld();
            worldManager.CreateNewWorld();
            worldManager.LoadWorld();

            RuntimeManager.CoreSystem.mixerResume();
        }

        private void ContinueButton_Clicked(ClickEvent _)
        {
            DisableButtons();
            continueButton.text = "Loading World";

            RuntimeManager.CoreSystem.mixerSuspend();
            worldManager.LoadWorld();
            RuntimeManager.CoreSystem.mixerResume();
        }

        private void DisableButtons()
        {
            newButton.SetEnabled(false);
            continueButton.SetEnabled(false);
        }

        private void GameManager_StateChanged(GameState oldState, GameState newState) =>
            UpdateCallbacks(newState);
    }
}
