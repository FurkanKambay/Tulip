using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FK.Common
{
    [PublicAPI]
    public static class Log
    {
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void WithFrame(string message)
        {
            string frameCount = $"<color=#669>Frame #{Time.frameCount,4}</color>";
            string deltaTimeMS = $"<color=#588>({Time.deltaTime * 1000,4:N0} ms)</color>";
            Debug.Log($"{frameCount} {deltaTimeMS} {message}");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Info(string m) => Debug.Log(m);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Info(string m, Object c) => Debug.Log(m, c);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Warning(string m) => Debug.LogWarning(m);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Warning(string m, Object c) => Debug.LogWarning(m, c);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Error(string m) => Debug.LogError(m);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Error(string m, Object c) => Debug.LogError(m, c);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Exception(System.Exception e) => Debug.LogException(e);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack, Conditional("ENABLE_LOGS")]
        public static void Exception(System.Exception e, Object c) => Debug.LogException(e, c);
    }
}
