#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tulip.Core.Editor
{
    [InitializeOnLoad]
    internal class UnityEditorSceneManager
    {
        static UnityEditorSceneManager()
        {
            SceneAsset bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Level/Boot.unity");
            EditorSceneManager.playModeStartScene = bootScene;
        }
    }
}

#endif
