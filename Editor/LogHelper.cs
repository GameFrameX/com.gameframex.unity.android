// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System;

namespace GameFrameX.Android.Editor
{
    /// <summary>
    /// Android 插件日志工具，统一输出格式为 [GameFrameX-Android:HH:mm:ss.fff]:。
    /// </summary>
    /// <remarks>
    /// Android plugin log utility with unified format [GameFrameX-Android:HH:mm:ss.fff]:.
    /// </remarks>
    internal static class LogHelper
    {
        /// <summary>
        /// 获取带时间戳的日志前缀。
        /// </summary>
        /// <remarks>
        /// Gets the log prefix with timestamp.
        /// </remarks>
        /// <returns>格式化后的前缀字符串 / Formatted prefix string</returns>
        private static string GetTime()
        {
            return $"[GameFrameX-Android:{DateTime.Now:HH:mm:ss.fff}]:";
        }

        /// <summary>
        /// 输出普通日志。
        /// </summary>
        /// <remarks>
        /// Logs an info message.
        /// </remarks>
        /// <param name="msg">日志消息 / Log message</param>
        public static void Log(string msg)
        {
            UnityEngine.Debug.Log(GetTime() + msg);
        }

        /// <summary>
        /// 输出警告日志。
        /// </summary>
        /// <remarks>
        /// Logs a warning message.
        /// </remarks>
        /// <param name="msg">警告消息 / Warning message</param>
        public static void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(GetTime() + msg);
        }

        /// <summary>
        /// 输出错误日志。
        /// </summary>
        /// <remarks>
        /// Logs an error message.
        /// </remarks>
        /// <param name="msg">错误消息 / Error message</param>
        public static void Error(string msg)
        {
            UnityEngine.Debug.LogError(GetTime() + msg);
        }
    }
}

#endif
