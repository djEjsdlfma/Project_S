using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace MoonLib.ScriptFinder_Pro.RunTime.DevLogs
{
    public static class DevLog
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object msg) => Debug.Log(msg);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object msg) => Debug.LogWarning(msg);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object msg) => Debug.LogError(msg);
    }
}