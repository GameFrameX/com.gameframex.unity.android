// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using GameFrameX.Editor;
using UnityEditor;

namespace GameFrameX.Android.Editor
{
    /// <summary>
    /// GameFrameX 构建流程的预钩子，在构建前输出 AndroidBuildConfig 配置摘要。
    /// </summary>
    /// <remarks>
    /// GameFrameX build pre-hook that logs AndroidBuildConfig summary before building.
    /// </remarks>
    internal class PostProcessBuildHelperHook : IBuilderPreHookHandler
    {
        /// <summary>
        /// 钩子优先级，数值越小越先执行。
        /// </summary>
        /// <remarks>
        /// Hook priority; lower values execute first.
        /// </remarks>
        public int Priority => 100;

        /// <summary>
        /// 在构建前执行，输出发现的配置文件摘要。
        /// </summary>
        /// <remarks>
        /// Executed before building; logs discovered config file summary.
        /// </remarks>
        /// <param name="target">构建目标平台 / Build target platform</param>
        /// <param name="path">构建路径 / Build path</param>
        public void Run(BuildTarget target, string path)
        {
            if (target != BuildTarget.Android)
            {
                return;
            }

            LogHelper.Log("===== Android Build Config Pre-Hook =====");
            LogHelper.Log("Build target: " + target);
            LogHelper.Log("Build path: " + path);

            var files = SettingLoader.LoadSettingsData("AndroidBuildConfig.json");
            if (files.Count == 0)
            {
                LogHelper.Log("No AndroidBuildConfig.json files found.");
                LogHelper.Log("===== Pre-Hook End =====");
                return;
            }

            LogHelper.Log("Discovered " + files.Count + " config file(s):");
            for (var i = 0; i < files.Count; i++)
            {
                LogHelper.Log("  [" + (i + 1) + "] " + files[i]);
            }

            var config = PostProcessBuildHelper.RunDiscoveryForHook();
            if (config != null)
            {
                LogHelper.Log("Configuration summary:");
                LogHelper.Log("  Maven repositories: " + config.mavenRepositories.Count);
                LogHelper.Log("  Gradle wrapper: " + config.gradleWrapper.Count + " prop(s)");

                if (config.launcher != null)
                {
                    LogHelper.Log("  launcher: " +
                                  config.launcher.dependencies.Count + " dep(s), " +
                                  config.launcher.permissions.Count + " perm(s), " +
                                  config.launcher.metaData.Count + " meta-data(s)");
                }

                if (config.unityLibrary != null)
                {
                    LogHelper.Log("  unityLibrary: " +
                                  config.unityLibrary.dependencies.Count + " dep(s), " +
                                  config.unityLibrary.permissions.Count + " perm(s), " +
                                  config.unityLibrary.metaData.Count + " meta-data(s)");
                }
            }

            LogHelper.Log("===== Pre-Hook End =====");
        }
    }
}

#endif
