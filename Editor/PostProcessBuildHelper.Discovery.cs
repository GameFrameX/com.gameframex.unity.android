// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        private const string ConfigFileName = "AndroidBuildConfig.json";

        /// <summary>
        /// 发现并合并配置文件，供 Hook 和主流程调用。
        /// </summary>
        /// <remarks>
        /// Discovers and merges config files, callable from Hook and main flow.
        /// </remarks>
        /// <returns>合并后的配置文件 / Merged config file</returns>
        internal static AndroidBuildConfigFile RunDiscoveryForHook()
        {
            return RunDiscovery();
        }

        private static AndroidBuildConfigFile RunDiscovery()
        {
            var jsonPaths = SettingLoader.LoadSettingsData(ConfigFileName);

            if (jsonPaths.Count == 0)
            {
                LogHelper.Log("No " + ConfigFileName + " files found.");
                return null;
            }

            LogHelper.Log("Found " + jsonPaths.Count + " config file(s):");
            foreach (var p in jsonPaths)
            {
                LogHelper.Log("  - " + p);
            }

            return MergeConfigs(jsonPaths);
        }

        private static AndroidBuildConfigFile MergeConfigs(List<string> files)
        {
            var merged = new AndroidBuildConfigFile
            {
                providerName = "Merged Config",
                launcher = new AndroidBuildConfigModule(),
                unityLibrary = new AndroidBuildConfigModule(),
            };
            var seenRepoUrls = new HashSet<string>();

            // Per-module dedup sets
            var launcherDeps = new HashSet<string>();
            var launcherPerms = new HashSet<string>();
            var launcherMeta = new HashSet<string>();
            var libDeps = new HashSet<string>();
            var libPerms = new HashSet<string>();
            var libMeta = new HashSet<string>();

            foreach (var filePath in files)
            {
                var config = LoadConfig(filePath);
                if (config == null)
                {
                    continue;
                }

                var label = !string.IsNullOrEmpty(config.providerName) ? config.providerName : filePath;
                LogHelper.Log("Merging config from: " + label);

                MergeRepositories(merged, config, seenRepoUrls);
                MergeGradleWrapper(merged, config);
                MergeModule(merged.launcher, config.launcher, launcherDeps, launcherPerms, launcherMeta);
                MergeModule(merged.unityLibrary, config.unityLibrary, libDeps, libPerms, libMeta);
            }

            LogMergedSummary(merged);
            return merged;
        }

        private static void MergeModule(AndroidBuildConfigModule target, AndroidBuildConfigModule source,
            HashSet<string> seenDeps, HashSet<string> seenPerms, HashSet<string> seenMeta)
        {
            if (source == null)
            {
                return;
            }

            MergeDependencies(target, source, seenDeps);
            MergePermissions(target, source, seenPerms);
            MergeMetaData(target, source, seenMeta);
        }

        private static AndroidBuildConfigFile LoadConfig(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return Utility.Json.ToObject<AndroidBuildConfigFile>(json);
            }
            catch (System.Exception ex)
            {
                LogHelper.Error("Failed to load " + filePath + ": " + ex.Message);
                return null;
            }
        }

        private static void MergeRepositories(AndroidBuildConfigFile merged, AndroidBuildConfigFile source, HashSet<string> seen)
        {
            if (source.mavenRepositories == null)
            {
                return;
            }

            foreach (var repo in source.mavenRepositories)
            {
                if (string.IsNullOrEmpty(repo.url) || !seen.Add(repo.url))
                {
                    continue;
                }

                merged.mavenRepositories.Add(repo);
            }
        }

        private static void MergeGradleWrapper(AndroidBuildConfigFile merged, AndroidBuildConfigFile source)
        {
            if (source.gradleWrapper == null)
            {
                return;
            }

            foreach (var kvp in source.gradleWrapper)
            {
                merged.gradleWrapper[kvp.Key] = kvp.Value;
            }
        }

        private static void MergeDependencies(AndroidBuildConfigModule target, AndroidBuildConfigModule source, HashSet<string> seen)
        {
            if (source.dependencies == null)
            {
                return;
            }

            foreach (var dep in source.dependencies)
            {
                var key = dep.configuration + ":" + dep.notation;
                if (string.IsNullOrEmpty(dep.notation) || !seen.Add(key))
                {
                    continue;
                }

                target.dependencies.Add(dep);
            }
        }

        private static void MergePermissions(AndroidBuildConfigModule target, AndroidBuildConfigModule source, HashSet<string> seen)
        {
            if (source.permissions == null)
            {
                return;
            }

            foreach (var perm in source.permissions)
            {
                if (string.IsNullOrEmpty(perm) || !seen.Add(perm))
                {
                    continue;
                }

                target.permissions.Add(perm);
            }
        }

        private static void MergeMetaData(AndroidBuildConfigModule target, AndroidBuildConfigModule source, HashSet<string> seen)
        {
            if (source.metaData == null)
            {
                return;
            }

            foreach (var meta in source.metaData)
            {
                if (string.IsNullOrEmpty(meta.name) || !seen.Add(meta.name))
                {
                    continue;
                }

                target.metaData.Add(meta);
            }
        }

        private static void LogMergedSummary(AndroidBuildConfigFile merged)
        {
            LogHelper.Log("Merged result: " +
                          merged.mavenRepositories.Count + " repo(s), " +
                          merged.gradleWrapper.Count + " gradle-wrapper prop(s)");

            if (merged.launcher != null)
            {
                LogHelper.Log("  launcher: " +
                              merged.launcher.dependencies.Count + " dep(s), " +
                              merged.launcher.permissions.Count + " perm(s), " +
                              merged.launcher.metaData.Count + " meta-data(s)");
            }

            if (merged.unityLibrary != null)
            {
                LogHelper.Log("  unityLibrary: " +
                              merged.unityLibrary.dependencies.Count + " dep(s), " +
                              merged.unityLibrary.permissions.Count + " perm(s), " +
                              merged.unityLibrary.metaData.Count + " meta-data(s)");
            }
        }
    }
}

#endif
