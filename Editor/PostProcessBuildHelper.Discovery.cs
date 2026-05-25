// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
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
            var seenAssetPackNames = new HashSet<string>();

            // Per-module dedup sets
            var launcherDeps = new HashSet<string>();
            var launcherPerms = new HashSet<string>();
            var launcherMeta = new HashSet<string>();
            var launcherAppAttrs = new HashSet<string>();
            var libDeps = new HashSet<string>();
            var libPerms = new HashSet<string>();
            var libMeta = new HashSet<string>();
            var libAppAttrs = new HashSet<string>();

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
                MergeGradleProperties(merged, config);
                MergeFileCopies(merged, config);
                if (config.assetPacksEnabled)
                {
                    merged.assetPacksEnabled = true;
                }
                MergeAssetPacks(merged, config, seenAssetPackNames);
                MergeLocalizedStringResources(merged, config);
                MergeModule(merged.launcher, config.launcher, launcherDeps, launcherPerms, launcherMeta, launcherAppAttrs);
                MergeModule(merged.unityLibrary, config.unityLibrary, libDeps, libPerms, libMeta, libAppAttrs);
            }

            LogMergedSummary(merged);
            return merged;
        }

        private static void MergeModule(AndroidBuildConfigModule target, AndroidBuildConfigModule source,
            HashSet<string> seenDeps, HashSet<string> seenPerms, HashSet<string> seenMeta, HashSet<string> seenAppAttrs)
        {
            if (source == null)
            {
                return;
            }

            MergeDependencies(target, source, seenDeps);
            MergePermissions(target, source, seenPerms);
            MergeMetaData(target, source, seenMeta);
            MergeApplicationAttributes(target, source, seenAppAttrs);
            MergeStringResources(target, source);
            MergeSdkVersions(target, source);
        }

        private static AndroidBuildConfigFile LoadConfig(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var decoded = MiniJSON.JsonDecode(json) as System.Collections.Hashtable;
                if (decoded == null)
                {
                    LogHelper.Error("Failed to parse JSON from " + filePath);
                    return null;
                }

                var config = new AndroidBuildConfigFile();
                config.providerName = decoded["providerName"] as string;

                if (decoded["mavenRepositories"] is System.Collections.ArrayList repos)
                {
                    foreach (System.Collections.Hashtable repo in repos)
                    {
                        config.mavenRepositories.Add(new MavenRepository
                        {
                            name = repo["name"] as string,
                            url = repo["url"] as string,
                        });
                    }
                }

                if (decoded["gradleWrapper"] is System.Collections.Hashtable wrapper)
                {
                    foreach (System.Collections.DictionaryEntry kvp in wrapper)
                    {
                        config.gradleWrapper[kvp.Key as string] = kvp.Value as string;
                    }
                }

                if (decoded["gradleProperties"] is System.Collections.Hashtable props)
                {
                    foreach (System.Collections.DictionaryEntry kvp in props)
                    {
                        config.gradleProperties[kvp.Key as string] = kvp.Value as string;
                    }
                }

                var configDir = Path.GetDirectoryName(filePath);
                if (decoded["fileCopies"] is System.Collections.Hashtable fileCopiesTable)
                {
                    foreach (System.Collections.DictionaryEntry kvp in fileCopiesTable)
                    {
                        var src = kvp.Key as string;
                        var dst = kvp.Value as string;
                        if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst))
                        {
                            continue;
                        }

                        var resolvedSrc = Path.IsPathRooted(src)
                            ? src
                            : Path.GetFullPath(Path.Combine(configDir, src));
                        config.fileCopies[resolvedSrc] = dst;
                    }
                }

                if (decoded["directoryCopies"] is System.Collections.Hashtable dirCopiesTable)
                {
                    foreach (System.Collections.DictionaryEntry kvp in dirCopiesTable)
                    {
                        var src = kvp.Key as string;
                        var dst = kvp.Value as string;
                        if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst))
                        {
                            continue;
                        }

                        var resolvedSrc = Path.IsPathRooted(src)
                            ? src
                            : Path.GetFullPath(Path.Combine(configDir, src));
                        config.directoryCopies[resolvedSrc] = dst;
                    }
                }

                if (decoded["assetPacksEnabled"] != null)
                {
                    config.assetPacksEnabled = (bool)decoded["assetPacksEnabled"];
                }

                if (decoded["assetPacks"] is System.Collections.ArrayList assetPacksList)
                {
                    foreach (System.Collections.Hashtable pack in assetPacksList)
                    {
                        config.assetPacks.Add(new AssetPackConfig
                        {
                            name = pack["name"] as string,
                            deliveryType = pack["deliveryType"] as string,
                            streamingAssetsPath = pack["streamingAssetsPath"] as string,
                        });
                    }
                }

                if (decoded["localizedStringResources"] is System.Collections.Hashtable localizedRes)
                {
                    foreach (System.Collections.DictionaryEntry localeEntry in localizedRes)
                    {
                        var locale = localeEntry.Key as string;
                        if (string.IsNullOrEmpty(locale))
                        {
                            continue;
                        }

                        if (localeEntry.Value is System.Collections.Hashtable localeStrings)
                        {
                            var dict = new Dictionary<string, string>();
                            foreach (System.Collections.DictionaryEntry kvp in localeStrings)
                            {
                                var key = kvp.Key as string;
                                if (!string.IsNullOrEmpty(key))
                                {
                                    dict[key] = kvp.Value as string;
                                }
                            }

                            config.localizedStringResources[locale] = dict;
                        }
                    }
                }

                if (decoded["launcher"] is System.Collections.Hashtable launcherObj)
                {
                    config.launcher = ParseModule(launcherObj);
                    config.launcher.signingConfig = ParseSigningConfig(launcherObj, configDir);
                }

                if (decoded["unityLibrary"] is System.Collections.Hashtable unityLibObj)
                {
                    config.unityLibrary = ParseModule(unityLibObj);
                    config.unityLibrary.signingConfig = ParseSigningConfig(unityLibObj, configDir);
                }

                return config;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error("Failed to load " + filePath + ": " + ex.Message);
                return null;
            }
        }

        private static AndroidBuildConfigModule ParseModule(System.Collections.Hashtable table)
        {
            var module = new AndroidBuildConfigModule();

            module.compileSdkVersion = table["compileSdkVersion"] as string;
            module.buildToolsVersion = table["buildToolsVersion"] as string;
            module.minSdkVersion = table["minSdkVersion"] as string;
            module.targetSdkVersion = table["targetSdkVersion"] as string;
            module.applicationId = table["applicationId"] as string;
            module.versionName = table["versionName"] as string;

            if (table["dependencies"] is System.Collections.ArrayList deps)
            {
                foreach (System.Collections.Hashtable dep in deps)
                {
                    module.dependencies.Add(new GradleDependency
                    {
                        configuration = dep["configuration"] as string,
                        notation = dep["notation"] as string,
                    });
                }
            }

            if (table["permissions"] is System.Collections.ArrayList perms)
            {
                foreach (string perm in perms)
                {
                    module.permissions.Add(perm);
                }
            }

            if (table["metaData"] is System.Collections.ArrayList metas)
            {
                foreach (System.Collections.Hashtable meta in metas)
                {
                    module.metaData.Add(new ManifestMetaData
                    {
                        name = meta["name"] as string,
                        value = meta["value"] as string,
                    });
                }
            }

            if (table["applicationAttributes"] is System.Collections.ArrayList appAttrs)
            {
                foreach (System.Collections.Hashtable attr in appAttrs)
                {
                    module.applicationAttributes.Add(new ApplicationAttribute
                    {
                        name = attr["name"] as string,
                        value = attr["value"] as string,
                    });
                }
            }

            if (table["stringResources"] is System.Collections.Hashtable stringRes)
            {
                foreach (System.Collections.DictionaryEntry kvp in stringRes)
                {
                    var key = kvp.Key as string;
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    module.stringResources[key] = kvp.Value as string;
                }
            }

            return module;
        }

        private static SigningConfig ParseSigningConfig(System.Collections.Hashtable table, string configDir)
        {
            if (!(table["signingConfig"] is System.Collections.Hashtable signingObj))
            {
                return null;
            }

            var storeFile = signingObj["storeFile"] as string;
            if (string.IsNullOrEmpty(storeFile))
            {
                return null;
            }

            return new SigningConfig
            {
                storeFile = storeFile,
                storePassword = signingObj["storePassword"] as string,
                keyAlias = signingObj["keyAlias"] as string,
                keyPassword = signingObj["keyPassword"] as string,
            };
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

        private static void MergeGradleProperties(AndroidBuildConfigFile merged, AndroidBuildConfigFile source)
        {
            if (source.gradleProperties == null)
            {
                return;
            }

            foreach (var kvp in source.gradleProperties)
            {
                merged.gradleProperties[kvp.Key] = kvp.Value;
            }
        }

        private static void MergeFileCopies(AndroidBuildConfigFile merged, AndroidBuildConfigFile source)
        {
            if (source.fileCopies != null)
            {
                foreach (var kvp in source.fileCopies)
                {
                    merged.fileCopies[kvp.Key] = kvp.Value;
                }
            }

            if (source.directoryCopies != null)
            {
                foreach (var kvp in source.directoryCopies)
                {
                    merged.directoryCopies[kvp.Key] = kvp.Value;
                }
            }
        }

        private static void MergeAssetPacks(AndroidBuildConfigFile merged, AndroidBuildConfigFile source, HashSet<string> seenNames)
        {
            if (source.assetPacks == null)
            {
                return;
            }

            foreach (var pack in source.assetPacks)
            {
                if (string.IsNullOrEmpty(pack.name))
                {
                    continue;
                }

                if (seenNames.Add(pack.name))
                {
                    merged.assetPacks.Add(pack);
                }
                else
                {
                    for (var i = 0; i < merged.assetPacks.Count; i++)
                    {
                        if (merged.assetPacks[i].name == pack.name)
                        {
                            if (!string.IsNullOrEmpty(pack.deliveryType))
                            {
                                merged.assetPacks[i] = new AssetPackConfig
                                {
                                    name = pack.name,
                                    deliveryType = pack.deliveryType,
                                    streamingAssetsPath = !string.IsNullOrEmpty(pack.streamingAssetsPath)
                                        ? pack.streamingAssetsPath
                                        : merged.assetPacks[i].streamingAssetsPath,
                                };
                            }

                            break;
                        }
                    }
                }
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

        private static void MergeApplicationAttributes(AndroidBuildConfigModule target, AndroidBuildConfigModule source, HashSet<string> seen)
        {
            if (source.applicationAttributes == null)
            {
                return;
            }

            foreach (var attr in source.applicationAttributes)
            {
                if (string.IsNullOrEmpty(attr.name) || !seen.Add(attr.name))
                {
                    continue;
                }

                target.applicationAttributes.Add(attr);
            }
        }

        private static void MergeStringResources(AndroidBuildConfigModule target, AndroidBuildConfigModule source)
        {
            if (source.stringResources == null)
            {
                return;
            }

            foreach (var kvp in source.stringResources)
            {
                target.stringResources[kvp.Key] = kvp.Value;
            }
        }

        private static void MergeLocalizedStringResources(AndroidBuildConfigFile target, AndroidBuildConfigFile source)
        {
            if (source.localizedStringResources == null)
            {
                return;
            }

            foreach (var localeKvp in source.localizedStringResources)
            {
                if (string.IsNullOrEmpty(localeKvp.Key) || localeKvp.Value == null)
                {
                    continue;
                }

                if (!target.localizedStringResources.TryGetValue(localeKvp.Key, out var targetLocale))
                {
                    targetLocale = new Dictionary<string, string>();
                    target.localizedStringResources[localeKvp.Key] = targetLocale;
                }

                foreach (var kvp in localeKvp.Value)
                {
                    targetLocale[kvp.Key] = kvp.Value;
                }
            }
        }

        private static void MergeSdkVersions(AndroidBuildConfigModule target, AndroidBuildConfigModule source)
        {
            MergeSdkVersionMax(target, source, nameof(AndroidBuildConfigModule.compileSdkVersion));
            MergeSdkVersionMax(target, source, nameof(AndroidBuildConfigModule.minSdkVersion));
            MergeSdkVersionMax(target, source, nameof(AndroidBuildConfigModule.targetSdkVersion));

            if (!string.IsNullOrEmpty(source.buildToolsVersion))
            {
                target.buildToolsVersion = source.buildToolsVersion;
            }

            if (!string.IsNullOrEmpty(source.applicationId))
            {
                target.applicationId = source.applicationId;
            }

            if (!string.IsNullOrEmpty(source.versionName))
            {
                target.versionName = source.versionName;
            }

            if (source.signingConfig != null)
            {
                target.signingConfig = source.signingConfig;
            }
        }

        private static void MergeSdkVersionMax(AndroidBuildConfigModule target, AndroidBuildConfigModule source, string fieldName)
        {
            var sourceValue = GetSdkVersionField(source, fieldName);
            if (string.IsNullOrEmpty(sourceValue))
            {
                return;
            }

            var targetValue = GetSdkVersionField(target, fieldName);
            if (string.IsNullOrEmpty(targetValue))
            {
                SetSdkVersionField(target, fieldName, sourceValue);
                return;
            }

            if (int.TryParse(sourceValue, out var sourceInt) && int.TryParse(targetValue, out var targetInt))
            {
                if (sourceInt > targetInt)
                {
                    SetSdkVersionField(target, fieldName, sourceValue);
                }
            }
        }

        private static string GetSdkVersionField(AndroidBuildConfigModule module, string fieldName)
        {
            return fieldName switch
            {
                nameof(AndroidBuildConfigModule.compileSdkVersion) => module.compileSdkVersion,
                nameof(AndroidBuildConfigModule.minSdkVersion) => module.minSdkVersion,
                nameof(AndroidBuildConfigModule.targetSdkVersion) => module.targetSdkVersion,
                _ => null,
            };
        }

        private static void SetSdkVersionField(AndroidBuildConfigModule module, string fieldName, string value)
        {
            switch (fieldName)
            {
                case nameof(AndroidBuildConfigModule.compileSdkVersion):
                    module.compileSdkVersion = value;
                    break;
                case nameof(AndroidBuildConfigModule.minSdkVersion):
                    module.minSdkVersion = value;
                    break;
                case nameof(AndroidBuildConfigModule.targetSdkVersion):
                    module.targetSdkVersion = value;
                    break;
            }
        }

        private static void LogMergedSummary(AndroidBuildConfigFile merged)
        {
            LogHelper.Log("Merged result: " +
                          merged.mavenRepositories.Count + " repo(s), " +
                          merged.gradleWrapper.Count + " gradle-wrapper prop(s), " +
                          merged.gradleProperties.Count + " gradle prop(s), " +
                          merged.fileCopies.Count + " file copy(ies), " +
                          merged.directoryCopies.Count + " dir copy(ies), " +
                          merged.assetPacks.Count + " asset pack(s), " +
                          merged.localizedStringResources.Count + " locale(s)");

            if (merged.launcher != null)
            {
                LogHelper.Log("  launcher: " +
                              merged.launcher.dependencies.Count + " dep(s), " +
                              merged.launcher.permissions.Count + " perm(s), " +
                              merged.launcher.metaData.Count + " meta-data(s), " +
                              merged.launcher.stringResources.Count + " string-res(s)" +
                              LogSdkVersions(merged.launcher));
            }

            if (merged.unityLibrary != null)
            {
                LogHelper.Log("  unityLibrary: " +
                              merged.unityLibrary.dependencies.Count + " dep(s), " +
                              merged.unityLibrary.permissions.Count + " perm(s), " +
                              merged.unityLibrary.metaData.Count + " meta-data(s), " +
                              merged.unityLibrary.stringResources.Count + " string-res(s)" +
                              LogSdkVersions(merged.unityLibrary));
            }

            if (merged.assetPacks.Count > 0)
            {
                foreach (var pack in merged.assetPacks)
                {
                    LogHelper.Log("  assetPack: " + pack.name +
                                  " deliveryType=" + pack.deliveryType +
                                  " streamingAssetsPath=" + pack.streamingAssetsPath);
                }
            }
        }

        internal static string LogSdkVersions(AndroidBuildConfigModule module)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(module.compileSdkVersion)) parts.Add("compileSdk=" + module.compileSdkVersion);
            if (!string.IsNullOrEmpty(module.buildToolsVersion)) parts.Add("buildTools=" + module.buildToolsVersion);
            if (!string.IsNullOrEmpty(module.minSdkVersion)) parts.Add("minSdk=" + module.minSdkVersion);
            if (!string.IsNullOrEmpty(module.targetSdkVersion)) parts.Add("targetSdk=" + module.targetSdkVersion);
            return parts.Count > 0 ? ", " + string.Join(", ", parts) : "";
        }
    }
}

#endif
