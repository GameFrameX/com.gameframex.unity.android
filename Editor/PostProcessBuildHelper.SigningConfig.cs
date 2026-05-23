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
        /// 向 launcher 的 build.gradle 中注入签名配置。
        /// </summary>
        /// <remarks>
        /// Injects signing configuration into the launcher's build.gradle.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetSigningConfig(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.launcher != null)
            {
                ApplySigningConfig(gradleRoot, "launcher", config.launcher.signingConfig);
            }

            if (config.unityLibrary != null)
            {
                ApplySigningConfig(gradleRoot, "unityLibrary", config.unityLibrary.signingConfig);
            }
        }

        private static void ApplySigningConfig(string gradleRoot, string module, SigningConfig signingConfig)
        {
            if (signingConfig == null || string.IsNullOrEmpty(signingConfig.storeFile))
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
            var braceDepth = 0;
            var inAndroid = false;
            var signingConfigsStart = -1;
            var signingConfigsEnd = -1;
            var inSigningConfigs = false;
            var signingConfigsBraceDepth = 0;
            var buildTypesReleaseStart = -1;
            var buildTypesReleaseEnd = -1;
            var inBuildTypes = false;
            var inBuildTypesRelease = false;
            var buildTypesBraceDepth = 0;

            for (var i = 0; i < lines.Count; i++)
            {
                var trimmed = lines[i].Trim();

                if (!inAndroid && trimmed.StartsWith("android") && trimmed.Contains("{"))
                {
                    inAndroid = true;
                    androidStart = i;
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

                    if (!inSigningConfigs && trimmed.StartsWith("signingConfigs") && trimmed.Contains("{"))
                    {
                        inSigningConfigs = true;
                        signingConfigsStart = i;
                        signingConfigsBraceDepth = 1;
                        continue;
                    }

                    if (inSigningConfigs)
                    {
                        if (trimmed.Contains("{"))
                        {
                            signingConfigsBraceDepth++;
                        }

                        if (trimmed.Contains("}"))
                        {
                            signingConfigsBraceDepth--;
                            if (signingConfigsBraceDepth == 0)
                            {
                                inSigningConfigs = false;
                                signingConfigsEnd = i;
                            }
                        }

                        continue;
                    }

                    if (!inBuildTypes && trimmed.StartsWith("buildTypes") && trimmed.Contains("{"))
                    {
                        inBuildTypes = true;
                        buildTypesBraceDepth = 1;
                        continue;
                    }

                    if (inBuildTypes)
                    {
                        if (trimmed.Contains("{"))
                        {
                            buildTypesBraceDepth++;
                        }

                        if (trimmed.Contains("}"))
                        {
                            buildTypesBraceDepth--;
                            if (buildTypesBraceDepth == 0)
                            {
                                inBuildTypes = false;
                            }
                        }

                        if (!inBuildTypesRelease && trimmed.StartsWith("release") && trimmed.Contains("{"))
                        {
                            inBuildTypesRelease = true;
                            buildTypesReleaseStart = i;
                            continue;
                        }

                        if (inBuildTypesRelease)
                        {
                            if (trimmed == "}")
                            {
                                inBuildTypesRelease = false;
                                buildTypesReleaseEnd = i;
                            }
                            else if (trimmed.Contains("signingConfig"))
                            {
                                return;
                            }
                        }
                    }
                }
            }

            if (androidStart < 0)
            {
                LogHelper.Warning("Could not find android {} block in " + module + "/build.gradle");
                return;
            }

            var storeFilePath = signingConfig.storeFile.Replace('\\', '/');

            if (signingConfigsStart >= 0 && signingConfigsEnd >= 0)
            {
                var releaseExists = false;
                for (var i = signingConfigsStart; i <= signingConfigsEnd; i++)
                {
                    if (lines[i].Trim().StartsWith("release"))
                    {
                        releaseExists = true;
                        break;
                    }
                }

                if (!releaseExists)
                {
                    var indent = DetectSignIndent(lines, signingConfigsStart);
                    lines.Insert(signingConfigsEnd, indent + "}");
                    lines.Insert(signingConfigsEnd, indent + "    keyPassword \"" + signingConfig.keyPassword + "\"");
                    lines.Insert(signingConfigsEnd, indent + "    keyAlias \"" + signingConfig.keyAlias + "\"");
                    lines.Insert(signingConfigsEnd, indent + "    storePassword \"" + signingConfig.storePassword + "\"");
                    lines.Insert(signingConfigsEnd, indent + "    storeFile file(\"" + storeFilePath + "\")");
                    lines.Insert(signingConfigsEnd, indent + "release {");
                    changed = true;
                }
            }
            else
            {
                var insertAt = androidEnd >= 0 ? androidEnd : lines.Count;
                var indent = DetectSignIndent(lines, androidStart);
                lines.Insert(insertAt, indent + "}");
                lines.Insert(insertAt, indent + "        keyPassword \"" + signingConfig.keyPassword + "\"");
                lines.Insert(insertAt, indent + "        keyAlias \"" + signingConfig.keyAlias + "\"");
                lines.Insert(insertAt, indent + "        storePassword \"" + signingConfig.storePassword + "\"");
                lines.Insert(insertAt, indent + "        storeFile file(\"" + storeFilePath + "\")");
                lines.Insert(insertAt, indent + "    release {");
                lines.Insert(insertAt, indent + "signingConfigs {");
                changed = true;
            }

            if (buildTypesReleaseStart >= 0 && buildTypesReleaseEnd >= 0)
            {
                var indent = DetectSignIndent(lines, buildTypesReleaseStart);
                lines.Insert(buildTypesReleaseEnd, indent + "    signingConfig signingConfigs.release");
                changed = true;
            }
            else if (androidEnd >= 0)
            {
                var targetIndex = androidEnd + (changed ? 7 : 0);
                var indent = DetectSignIndent(lines, androidStart);
                lines.Insert(targetIndex, indent + "    }");
                lines.Insert(targetIndex, indent + "        signingConfig signingConfigs.release");
                lines.Insert(targetIndex, indent + "    release {");
                lines.Insert(targetIndex, indent + "buildTypes {");
                changed = true;
            }

            if (changed)
            {
                File.WriteAllLines(filePath, lines);
                LogHelper.Log("Injected signing config into " + module + "/build.gradle");
            }
        }

        private static string DetectSignIndent(List<string> lines, int aroundIndex)
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

            return "    ";
        }
    }
}

#endif
