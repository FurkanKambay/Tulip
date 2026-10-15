using System;
using System.Diagnostics;
using FK.Common;
using UnityEditor;
using UnityEngine;

namespace FK.Tulip.Editor
{
    internal sealed class HelperCommands : MonoBehaviour
    {
        [MenuItem("Tools/Force Reserialize Assets", priority = 10)]
        private static void ForceReserializeAssets()
        {
            if (!EditorApplication.isPlaying)
                AssetDatabase.ForceReserializeAssets();
        }

        [MenuItem("Tools/Install Local .gitconfig", priority = 15)]
        private static void InstallLocalGitConfig()
        {
            try
            {
                Process.Start("git", "config --local include.path ../.gitconfig");
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}
