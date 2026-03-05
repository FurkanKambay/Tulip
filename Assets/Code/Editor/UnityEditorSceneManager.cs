using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tulip.Editor
{
    [InitializeOnLoad]
    internal class UnityEditorSceneManager
    {
        static UnityEditorSceneManager()
        {
            SceneAsset bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Level/0 Boot.unity");
            EditorSceneManager.playModeStartScene = bootScene;
        }
    }
}
