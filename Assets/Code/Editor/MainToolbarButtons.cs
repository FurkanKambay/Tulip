using System.Diagnostics;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FK.Tulip.Editor
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class MainToolbarButtons
    {
        private const string bootScenePath = "0 Boot";
        private const string menusScenePath = "1 Menus";
        private const string gameScenePath = "2 Game";

        private const string testArenaScenePath = "Testing/Arena";
        private const string testGymScenePath = "Testing/Gym";
        private const string testZooScenePath = "Testing/Zoo";

        #region Buttons

        [MainToolbarElement("Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement ProjectSettingsButton()
        {
            var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
            var content = new MainToolbarContent(icon, "Project Settings");
            return new MainToolbarButton(content, () => SettingsService.OpenProjectSettings());
        }

#if UNITY_EDITOR_WIN
        [MainToolbarElement("Open Builds in File Explorer", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement BuildsButton()
        {
            var content = new MainToolbarContent("Builds", "Open Builds in File Explorer");
            return new MainToolbarButton(content, () => Process.Start("explorer.exe", "Builds"));
        }
#endif

        #endregion

        #region Scene Toolbar Button Definitions

        [MainToolbarElement("Tulip Scenes/Boot", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenBootSceneButton() => GetToolbarButton("🚀", bootScenePath);

        [MainToolbarElement("Tulip Scenes/Menus", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenMenusSceneButton() => GetMenusSceneToolbarButton();

        [MainToolbarElement("Tulip Scenes/Game", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenGameSceneButton() => GetGameSceneToolbarButton();

        [MainToolbarElement("Tulip Scenes/Arena", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenArenaSceneButton() => GetToolbarButton("⚔️", testArenaScenePath);

        [MainToolbarElement("Tulip Scenes/Gym", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenGymSceneButton() => GetToolbarButton("🪜", testGymScenePath);

        [MainToolbarElement("Tulip Scenes/Zoo", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenZooSceneButton() => GetToolbarButton("🧱", testZooScenePath);

        #endregion

        #region Scene Button Lambdas

        private static MainToolbarButton GetToolbarButton(string text, string sceneSubPath)
        {
            var content = new MainToolbarContent(text, $"Open \"{sceneSubPath}.unity\"");
            return new MainToolbarButton(content, () => TryOpenScene(sceneSubPath));
        }

        private static MainToolbarButton GetMenusSceneToolbarButton()
        {
            return new MainToolbarButton(new MainToolbarContent("📱️", $"Open \"{menusScenePath}.unity\""), () =>
            {
                TryOpenScene(bootScenePath);

                Scene menusScene = TryOpenScene(menusScenePath, OpenSceneMode.Additive);
                SceneManager.SetActiveScene(menusScene);
            });
        }

        private static MainToolbarButton GetGameSceneToolbarButton()
        {
            return new MainToolbarButton(new MainToolbarContent("🕹️", $"Open \"{gameScenePath}.unity\""), () =>
            {
                // BUG: going into play mode unloads the game scene - bc Boot loads Menus as a single scene on Start
                TryOpenScene(bootScenePath);

                Scene gameScene = TryOpenScene(gameScenePath, OpenSceneMode.Additive);
                SceneManager.SetActiveScene(gameScene);
            });
        }

        #endregion

        private static Scene TryOpenScene(string sceneSubPath, OpenSceneMode openSceneMode = OpenSceneMode.Single)
        {
            if (Application.isPlaying || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return default;

            return EditorSceneManager.OpenScene($"Assets/Level/{sceneSubPath}.unity", openSceneMode);
        }
    }
}
