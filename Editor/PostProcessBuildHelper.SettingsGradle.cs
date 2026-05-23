// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 向 settings.gradle 的 repositories 块中注入 Maven 仓库。
        /// </summary>
        /// <remarks>
        /// Injects Maven repositories into the repositories block of settings.gradle.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetSettingsGradle(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.mavenRepositories == null || config.mavenRepositories.Count == 0)
            {
                return;
            }

            var filePath = FindFile(gradleRoot, "settings.gradle");
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Warning("Could not find settings.gradle");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var modified = new List<string>(lines);
            var changed = false;

            var repoBlockStart = -1;
            var repoBlockEnd = -1;
            var braceDepth = 0;

            for (var i = 0; i < modified.Count; i++)
            {
                var line = modified[i].Trim();

                if (line.StartsWith("dependencyResolutionManagement", StringComparison.OrdinalIgnoreCase))
                {
                    braceDepth = line.Contains("{") ? 1 : 0;
                    continue;
                }

                if (braceDepth == 1 && line.StartsWith("repositories", StringComparison.OrdinalIgnoreCase))
                {
                    repoBlockStart = i;
                    braceDepth += line.Contains("{") ? 1 : 0;
                    continue;
                }

                if (repoBlockStart >= 0 && braceDepth > 0)
                {
                    if (line.Contains("{"))
                    {
                        braceDepth++;
                    }

                    if (line.Contains("}"))
                    {
                        braceDepth--;
                        if (braceDepth == 1)
                        {
                            repoBlockEnd = i;
                            break;
                        }
                    }
                }
            }

            if (repoBlockStart < 0 || repoBlockEnd < 0)
            {
                LogHelper.Warning("Could not find repositories block in " + Path.GetFileName(filePath));
                return;
            }

            for (var i = config.mavenRepositories.Count - 1; i >= 0; i--)
            {
                var repo = config.mavenRepositories[i];
                var mavenLine = "        maven { url '" + repo.url + "' }";

                if (ContainsLine(modified, repo.url))
                {
                    continue;
                }

                modified.Insert(repoBlockEnd, mavenLine);
                changed = true;
                LogHelper.Log("+ settings.gradle: maven { url '" + repo.url + "' }");
            }

            if (changed)
            {
                File.WriteAllLines(filePath, modified);
            }
        }
    }
}

#endif
