using SaintsField.Playa;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tulip.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused
    }

    public class GameManager : MonoBehaviour
    {
        [ShowInInspector]
        public static GameState CurrentState { get; private set; }

        private const int bootSceneIndex = 0;
        private const int mainMenuSceneIndex = 1;
        private const int gameSceneIndex = 2;

        private static Scene BootScene => SceneManager.GetSceneByBuildIndex(bootSceneIndex);
        private static Scene MainMenuScene => SceneManager.GetSceneByBuildIndex(mainMenuSceneIndex);
        private static Scene GameScene => SceneManager.GetSceneByBuildIndex(gameSceneIndex);

        private static AsyncOperation gameSceneLoadOperation;

        #region Unity Callbacks
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            CurrentState = GameState.MainMenu;
            gameSceneLoadOperation = null;

#if UNITY_EDITOR
            if (!BootScene.isLoaded)
                SceneManager.LoadSceneAsync(bootSceneIndex, LoadSceneMode.Additive);

            if (GameScene.isLoaded)
                SceneManager.UnloadSceneAsync(GameScene);
#endif
        }

        private void Awake()
        {
            if (!MainMenuScene.isLoaded)
                SceneManager.LoadScene(mainMenuSceneIndex, LoadSceneMode.Additive);
        }

        private async void Start() => await LoadGameAsync();

        private void OnEnable() => Application.wantsToQuit += IsSafeToQuit;
        private void OnDisable() => Application.wantsToQuit -= IsSafeToQuit;

        private static async Awaitable LoadGameAsync()
        {
            gameSceneLoadOperation = SceneManager.LoadSceneAsync(gameSceneIndex, LoadSceneMode.Additive);
            Assert.IsNotNull(gameSceneLoadOperation);
            gameSceneLoadOperation.allowSceneActivation = false;

            await gameSceneLoadOperation;

            SceneManager.SetActiveScene(GameScene);
        }
        #endregion

        #region Public Static Methods
        [LayoutGroup("Buttons", ELayout.Background, marginTop: 8)]
        [Button, Ordered]
        public static async Awaitable ReturnToMainMenu() =>
            await SwitchTo(GameState.MainMenu);

        [Button, Ordered]
        public static async Awaitable StartNewGame() =>
            await SwitchTo(GameState.Playing);

        [Button, Ordered]
        public static async Awaitable SetPaused(bool shouldPause)
        {
            await SwitchTo(
                CurrentState switch
                {
                    GameState.Playing when shouldPause => GameState.Paused,
                    GameState.Paused when !shouldPause => GameState.Playing,
                    _ => CurrentState
                }
            );
        }

        public static void QuitGame()
        {
            if (!IsSafeToQuit())
                return;

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Private Static Methods
        private static async Awaitable SwitchTo(GameState newState)
        {
            if (newState == CurrentState)
                return;

            GameState oldState = CurrentState;
            CurrentState = newState;

            UpdateTimeScale();
            UpdateInputs();

            GameStateChange.Raise(oldState, newState);

            if (CurrentState != GameState.MainMenu)
                AllowGameSceneActivation();
            else
            {
                await SceneManager.UnloadSceneAsync(GameScene);
                await LoadGameAsync();
            }
        }

        private static void AllowGameSceneActivation()
        {
            if (gameSceneLoadOperation != null)
                gameSceneLoadOperation.allowSceneActivation = true;
        }

        private static void UpdateTimeScale() => Time.timeScale = CurrentState switch
        {
            GameState.Paused when Settings.Gameplay.AllowPause => 0,
            _ => 1
        };

        private static void UpdateInputs()
        {
            InputActionMap playerControls = InputSystem.actions.actionMaps[0];
            InputActionMap uiControls = InputSystem.actions.actionMaps[1];

            if (CurrentState == GameState.Playing)
            {
                playerControls.Enable();
                uiControls.Disable();
            }
            else
            {
                playerControls.Disable();
                uiControls.Enable();
            }
        }

        private static bool IsSafeToQuit()
        {
            if (CurrentState is GameState.MainMenu)
                return true;

            // TODO: save game before quitting
            Debug.LogWarning("Quit requested. Should save game first.");

            return true;
        }
        #endregion
    }
}
