using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Furkan.Common
{
    [PublicAPI]
    public static class FurkanLogger
    {
        [HideInCallstack, Conditional("UNITY_EDITOR")]
        public static void LogWithFrame(string message)
        {
            string frameCount = $"<color=#669>Frame #{Time.frameCount,4}</color>";
            string deltaTimeMS = $"<color=#588>({Time.deltaTime * 1000,4:N0} ms)</color>";
            Debug.Log($"{frameCount} {deltaTimeMS} {message}");
        }
    }
}
