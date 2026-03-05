using System.Collections;
using Furkan.Common;
using JetBrains.Annotations;
using SaintsField;
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

    [PublicAPI]
    public sealed class GameManager : MonoBehaviour
    {
        [LayoutGroup("/Main Menu Scene", ELayout.FoldoutBox)]
        [SaintsRow(true)]
        [SerializeField] SceneInfo mainMenuSceneInfo;

        [LayoutGroup("/Game Scene", ELayout.FoldoutBox)]
        [SaintsRow(true)]
        [SerializeField] SceneInfo gameSceneInfo;

        [LayoutEnd]
        [SerializeField] bool showSplashScreen;
        [SerializeField] float splashScreenDuration = 2;

        [ShowInInspector]
        internal static GameState CurrentState { get; private set; }

        private static GameManager instance;

#region Unity Callbacks, Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            CurrentState = GameState.MainMenu;
        }

        private void Awake()
        {
            Assert.IsNull(instance);
            DontDestroyOnLoad(gameObject);
            instance = this;

            mainMenuSceneInfo = new SceneInfo(1);
            gameSceneInfo = new SceneInfo(2);
        }

        private void OnEnable() => Application.wantsToQuit += IsSafeToQuit;
        private void OnDisable() => Application.wantsToQuit -= IsSafeToQuit;

#endregion

#region Scene Switching

        private IEnumerator Start()
        {
            if (!showSplashScreen || Application.isEditor)
                yield return mainMenuSceneInfo.LoadAsync(LoadSceneMode.Single);
            else
            {
                yield return mainMenuSceneInfo.PreloadAsync(LoadSceneMode.Single);
                yield return new WaitForSecondsRealtime(splashScreenDuration);
                yield return mainMenuSceneInfo.ActivateScene();
            }

            yield return gameSceneInfo.PreloadAsync();

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Called when the user wants to start playing.
        /// </summary>
        private void ActivateGameScene() =>
            StartCoroutine(gameSceneInfo.ActivateScene());

        /// <summary>
        /// Called when returning to the main menu.
        /// </summary>
        private void ReloadGameScene() =>
            StartCoroutine(gameSceneInfo.ReloadAsync());

#endregion

#region High-Level Game Flow API

        [LayoutGroup("Buttons", ELayout.Background, marginTop: 8)]
        [Button, Ordered]
        public static void ReturnToMainMenu() =>
            SwitchTo(GameState.MainMenu);

        [Button, Ordered]
        public static void StartNewGame() =>
            SwitchTo(GameState.Playing);

        [Button, Ordered]
        public static void SetPaused(bool shouldPause)
        {
            GameState newState = CurrentState switch
            {
                GameState.Playing when shouldPause => GameState.Paused,
                GameState.Paused when !shouldPause => GameState.Playing,
                _                                  => CurrentState
            };

            SwitchTo(newState);
        }

        [Button, Ordered]
        internal static void QuitGame()
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

#region Private Helper Methods

        private static void SwitchTo(GameState newState)
        {
            if (newState == CurrentState)
                return;

            GameState oldState = CurrentState;
            CurrentState = newState;

            UpdateTimeScale();
            UpdateInputs();

            GameStateChange.Raise(oldState, newState);

            if (oldState is GameState.MainMenu)
                instance.ActivateGameScene();
            else if (newState is GameState.MainMenu)
                instance.ReloadGameScene();
        }

        private static void UpdateTimeScale() =>
            Time.timeScale = CurrentState == GameState.Paused ? 0 : 1;

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
            Debug.Log("Quit requested. Should save game first.");

            return true;
        }

#endregion
    }
}
