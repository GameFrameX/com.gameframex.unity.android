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
        /// 向 launcher 和 unityLibrary 的 build.gradle 中注入 SDK 版本配置。
        /// </summary>
        /// <remarks>
        /// Injects SDK version configuration into the build.gradle of both launcher and unityLibrary.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetAndroidBlock(string gradleRoot, AndroidBuildConfigFile config)
        {
            ApplyAndroidBlock(gradleRoot, "launcher", config.launcher);
            ApplyAndroidBlock(gradleRoot, "unityLibrary", config.unityLibrary);
        }

        private static void ApplyAndroidBlock(string gradleRoot, string module, AndroidBuildConfigModule configModule)
        {
            if (configModule == null)
            {
                return;
            }

            var hasAndroidFields = !string.IsNullOrEmpty(configModule.compileSdkVersion) ||
                                   !string.IsNullOrEmpty(configModule.buildToolsVersion);
            var hasDefaultConfigFields = !string.IsNullOrEmpty(configModule.minSdkVersion) ||
                                         !string.IsNullOrEmpty(configModule.targetSdkVersion);

            if (!hasAndroidFields && !hasDefaultConfigFields)
            {
                return;
            }

            var filePath = FindFile(gradleRoot, Path.Combine(module, "build.gradle"));
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Warning("Could not find " + module + "/build.gradle");
                return;
            }

            var lines = new List<string>(File.ReadAllLines(filePath));
            var changed = false;

            var androidStart = -1;
            var androidEnd = -1;
            var defaultConfigStart = -1;
            var defaultConfigEnd = -1;
            var braceDepth = 0;
            var inAndroid = false;
            var inDefaultConfig = false;
            var androidBraceDepth = 0;

            for (var i = 0; i < lines.Count; i++)
            {
                var trimmed = lines[i].Trim();

                if (!inAndroid && trimmed.StartsWith("android") && trimmed.Contains("{"))
                {
                    inAndroid = true;
                    androidStart = i;
                    androidBraceDepth = 1;
                    braceDepth = 1;
                    continue;
                }

                if (inAndroid)
                {
                    if (trimmed.Contains("{"))
                    {
                        braceDepth++;
                    }

                    if (trimmed.Contains("}"))
                    {
                        braceDepth--;
                        if (braceDepth == 0)
                        {
                            androidEnd = i;
                            inAndroid = false;
                            continue;
                        }
                    }

                    if (!inDefaultConfig && (trimmed.StartsWith("defaultConfig") && trimmed.Contains("{")))
                    {
                        inDefaultConfig = true;
                        defaultConfigStart = i;
                    }
                    else if (inDefaultConfig)
                    {
                        if (trimmed.Contains("}"))
                        {
                            inDefaultConfig = false;
                            defaultConfigEnd = i;
                        }
                    }
                }
            }

            if (hasAndroidFields && androidStart >= 0)
            {
                changed |= ReplaceOrInsertField(lines, "compileSdk", configModule.compileSdkVersion, androidStart,
                    androidEnd >= 0 ? androidEnd : lines.Count - 1);
                changed |= ReplaceOrInsertField(lines, "buildToolsVersion", configModule.buildToolsVersion,
                    androidStart, androidEnd >= 0 ? androidEnd : lines.Count - 1);
            }

            if (hasDefaultConfigFields && defaultConfigStart >= 0 && defaultConfigEnd >= 0)
            {
                changed |= ReplaceOrInsertField(lines, "minSdk", configModule.minSdkVersion, defaultConfigStart,
                    defaultConfigEnd);
                changed |= ReplaceOrInsertField(lines, "targetSdk", configModule.targetSdkVersion, defaultConfigStart,
                    defaultConfigEnd);
            }
            else if (hasDefaultConfigFields && androidStart >= 0 && androidEnd >= 0)
            {
                var insertIndex = androidEnd;
                lines.Insert(insertIndex, "        }");
                lines.Insert(insertIndex, "");
                insertIndex = lines.Count - 2;
                if (!string.IsNullOrEmpty(configModule.minSdkVersion))
                {
                    lines.Insert(insertIndex, "            minSdkVersion " + configModule.minSdkVersion);
                }

                if (!string.IsNullOrEmpty(configModule.targetSdkVersion))
                {
                    lines.Insert(insertIndex, "            targetSdkVersion " + configModule.targetSdkVersion);
                }

                lines.Insert(insertIndex, "        defaultConfig {");
                changed = true;
            }

            if (changed)
            {
                File.WriteAllLines(filePath, lines);
                LogHelper.Log("Updated SDK versions in " + module + "/build.gradle");
            }
        }

        private static bool ReplaceOrInsertField(List<string> lines, string fieldKey, string value, int blockStart,
            int blockEnd)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            for (var i = blockStart; i <= blockEnd && i < lines.Count; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.StartsWith(fieldKey) && (trimmed.Contains("Version") || trimmed.Contains("=") ||
                                                      char.IsDigit(trimmed[fieldKey.Length..].TrimStart()[0])))
                {
                    lines[i] = BuildSdkLine(lines[i], fieldKey, value);
                    LogHelper.Log("  " + fieldKey + " -> " + value);
                    return true;
                }
            }

            var indent = DetectIndent(lines, blockStart);
            lines.Insert(blockEnd, indent + fieldKey + "Version " + value);
            LogHelper.Log("  + " + fieldKey + "Version " + value);
            return true;
        }

        private static string BuildSdkLine(string originalLine, string fieldKey, string value)
        {
            var indent = originalLine.Substring(0, originalLine.Length - originalLine.TrimStart().Length);
            return indent + fieldKey + "Version " + value;
        }

        private static string DetectIndent(List<string> lines, int aroundIndex)
        {
            for (var i = aroundIndex + 1; i < lines.Count && i < aroundIndex + 10; i++)
            {
                var trimmed = lines[i].TrimStart();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                var indent = lines[i].Substring(0, lines[i].Length - trimmed.Length);
                if (indent.Length > 0)
                {
                    return indent;
                }
            }

            return "        ";
        }
    }
}

#endif
