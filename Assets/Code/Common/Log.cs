using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Furkan.Common
{
    [PublicAPI]
    public static class Log
    {
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void WithFrame(string message)
        {
            string frameCount = $"<color=#669>Frame #{Time.frameCount,4}</color>";
            string deltaTimeMS = $"<color=#588>({Time.deltaTime * 1000,4:N0} ms)</color>";
            Debug.Log($"{frameCount} {deltaTimeMS} {message}");
        }

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Info(string m) => Debug.Log(m);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Info(string m, Object c) => Debug.Log(m, c);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Warning(string m) => Debug.LogWarning(m);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Warning(string m, Object c) => Debug.LogWarning(m, c);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Error(string m) => Debug.LogError(m);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Error(string m, Object c) => Debug.LogError(m, c);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Exception(System.Exception e) => Debug.LogException(e);

        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Exception(System.Exception e, Object c) => Debug.LogException(e, c);
    }
}
