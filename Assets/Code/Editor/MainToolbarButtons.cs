using System.Diagnostics;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Tulip.Editor
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class MainToolbarButtons
    {
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

        [MainToolbarElement("Tulip Scenes/Boot", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenBootSceneButton() => GetToolbarButton("🚀", "0 Boot");

        [MainToolbarElement("Tulip Scenes/Menus", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenMenusSceneButton() => GetToolbarButton("📱", "1 Menus");

        [MainToolbarElement("Tulip Scenes/Game", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenGameSceneButton() => GetToolbarButton("🕹️", "2 Game");

        [MainToolbarElement("Tulip Scenes/Arena", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenArenaSceneButton() => GetToolbarButton("⚔️", "Testing/Arena");

        [MainToolbarElement("Tulip Scenes/Gym", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenGymSceneButton() => GetToolbarButton("🪜", "Testing/Gym");

        [MainToolbarElement("Tulip Scenes/Zoo", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement OpenZooSceneButton() => GetToolbarButton("🧱", "Testing/Zoo");

        private static MainToolbarButton GetToolbarButton(string text, string sceneSubPath) =>
            new(new MainToolbarContent(text, $"Open \"{sceneSubPath}.unity\""), () => TryOpenScene(sceneSubPath));

        private static void TryOpenScene(string sceneSubPath)
        {
            if (!Application.isPlaying && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene($"Assets/Level/{sceneSubPath}.unity", OpenSceneMode.Single);
        }
    }
}
