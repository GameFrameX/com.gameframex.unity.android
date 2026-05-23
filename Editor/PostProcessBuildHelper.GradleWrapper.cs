// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 向 gradle-wrapper.properties 中注入键值对配置。
        /// </summary>
        /// <remarks>
        /// Injects key-value pairs into gradle-wrapper.properties.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetGradleWrapper(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.gradleWrapper == null || config.gradleWrapper.Count == 0)
            {
                return;
            }

            var filePath = FindFile(gradleRoot, Path.Combine("gradle", "wrapper", "gradle-wrapper.properties"));
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Warning("Could not find gradle-wrapper.properties");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var modified = new List<string>(lines);
            var changed = false;

            foreach (var kvp in config.gradleWrapper)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    continue;
                }

                var found = false;
                for (var i = 0; i < modified.Count; i++)
                {
                    var line = modified[i].TrimStart();
                    if (line.StartsWith(kvp.Key + "="))
                    {
                        modified[i] = kvp.Key + "=" + kvp.Value;
                        found = true;
                        changed = true;
                        LogHelper.Log("+ gradle-wrapper.properties: " + kvp.Key + "=" + kvp.Value);
                        break;
                    }
                }

                if (!found)
                {
                    modified.Add(kvp.Key + "=" + kvp.Value);
                    changed = true;
                    LogHelper.Log("+ gradle-wrapper.properties: " + kvp.Key + "=" + kvp.Value);
                }
            }

            if (changed)
            {
                File.WriteAllLines(filePath, modified);
            }
        }
    }
}

#endif
