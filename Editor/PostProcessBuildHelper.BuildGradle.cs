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
        /// 向 launcher 和 unityLibrary 的 build.gradle 中注入依赖。
        /// </summary>
        /// <remarks>
        /// Injects dependencies into the build.gradle of both launcher and unityLibrary.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetBuildGradle(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.launcher != null && config.launcher.dependencies.Count > 0)
            {
                var launcherPath = FindFile(gradleRoot, Path.Combine("launcher", "build.gradle"));
                if (!string.IsNullOrEmpty(launcherPath))
                {
                    InjectDependenciesBlock(launcherPath, config.launcher.dependencies);
                }
                else
                {
                    LogHelper.Warning("Could not find launcher/build.gradle");
                }
            }

            if (config.unityLibrary != null && config.unityLibrary.dependencies.Count > 0)
            {
                var libPath = FindFile(gradleRoot, Path.Combine("unityLibrary", "build.gradle"));
                if (!string.IsNullOrEmpty(libPath))
                {
                    InjectDependenciesBlock(libPath, config.unityLibrary.dependencies);
                }
                else
                {
                    LogHelper.Warning("Could not find unityLibrary/build.gradle");
                }
            }
        }

        /// <summary>
        /// 向单个 build.gradle 的 dependencies 块中注入依赖。
        /// </summary>
        /// <remarks>
        /// Injects dependencies into the dependencies block of a single build.gradle.
        /// </remarks>
        /// <param name="filePath">build.gradle 文件路径 / Path to build.gradle</param>
        /// <param name="dependencies">依赖列表 / Dependency list</param>
        private static void InjectDependenciesBlock(string filePath, List<GradleDependency> dependencies)
        {
            var lines = File.ReadAllLines(filePath);
            var modified = new List<string>(lines);
            var changed = false;

            var depsStart = -1;
            var depsEnd = -1;
            var braceDepth = 0;
            var inDependencies = false;

            for (var i = 0; i < modified.Count; i++)
            {
                var line = modified[i].Trim();

                if (line.StartsWith("dependencies") && line.Contains("{"))
                {
                    inDependencies = true;
                    depsStart = i;
                    braceDepth = 1;
                    continue;
                }

                if (inDependencies)
                {
                    if (line.Contains("{"))
                    {
                        braceDepth++;
                    }

                    if (line.Contains("}"))
                    {
                        braceDepth--;
                        if (braceDepth == 0)
                        {
                            depsEnd = i;
                            break;
                        }
                    }
                }
            }

            if (depsStart < 0 || depsEnd < 0)
            {
                LogHelper.Warning("Could not find dependencies block in " + Path.GetFileName(filePath));
                return;
            }

            for (var i = dependencies.Count - 1; i >= 0; i--)
            {
                var dep = dependencies[i];
                var depLine = "    " + dep.configuration + " '" + dep.notation + "'";

                if (ContainsLine(modified, dep.notation))
                {
                    continue;
                }

                modified.Insert(depsEnd, depLine);
                changed = true;
                LogHelper.Log("+ " + Path.GetFileName(Path.GetDirectoryName(filePath)) + "/build.gradle: " + dep.configuration + " '" + dep.notation + "'");
            }

            if (changed)
            {
                File.WriteAllLines(filePath, modified);
            }
        }
    }
}

#endif
