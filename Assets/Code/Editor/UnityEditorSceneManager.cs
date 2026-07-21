using UnityEditor;
using UnityEditor.SceneManagement;

namespace FK.Tulip.Editor
{
    [InitializeOnLoad]
    internal static class UnityEditorSceneManager
    {
        private const string menuName = "Tools/Set Play Mode Start Scene";
        private const string sceneName = "0 Boot";

        private static bool enabled;

        static UnityEditorSceneManager()
        {
            enabled = EditorPrefs.GetBool(menuName, defaultValue: false);
            EditorApplication.delayCall += () => Toggle(enabled);
        }

        [MenuItem(menuName, priority = 5)]
        private static void ToggleAction() => Toggle(!enabled);

        private static void Toggle(bool newValue)
        {
            enabled = newValue;
            Menu.SetChecked(menuName, enabled);
            EditorPrefs.SetBool(menuName, enabled);

            SceneAsset bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>($"Assets/Level/{sceneName}.unity");
            EditorSceneManager.playModeStartScene = enabled ? bootScene : null;
        }
    }
}
