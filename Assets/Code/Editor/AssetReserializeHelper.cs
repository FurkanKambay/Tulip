using UnityEditor;
using UnityEngine;

namespace Tulip.Editor
{
    public sealed class AssetReserializeHelper : MonoBehaviour
    {
        [MenuItem("Tools/Force Reserialize Assets", priority = 10)]
        private static void ForceReserializeAssets()
        {
            if (!EditorApplication.isPlaying)
                AssetDatabase.ForceReserializeAssets();
        }
    }
}
