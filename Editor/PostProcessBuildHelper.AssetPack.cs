// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 创建 Asset Pack 模块并更新 settings.gradle。
        /// </summary>
        /// <remarks>
        /// Creates asset pack modules and updates settings.gradle.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetAssetPacks(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (!config.assetPacksEnabled)
            {
                return;
            }

            if (config.assetPacks == null || config.assetPacks.Count == 0)
            {
                return;
            }

            foreach (var pack in config.assetPacks)
            {
                CreateAssetPackModule(gradleRoot, pack);
            }

            InjectAssetPackIncludes(gradleRoot, config.assetPacks);
            InjectAssetPacksToLauncher(gradleRoot, config.assetPacks);
        }

        /// <summary>
        /// 创建单个 Asset Pack 的目录结构、build.gradle 和 AndroidManifest.xml，
        /// 并将对应的 StreamingAssets 子目录移动到模块中。
        /// </summary>
        /// <remarks>
        /// Creates the directory structure, build.gradle, and AndroidManifest.xml for
        /// a single asset pack module, and moves the matching StreamingAssets subdirectory into it.
        /// </remarks>
        private static void CreateAssetPackModule(string gradleRoot, AssetPackConfig pack)
        {
            if (string.IsNullOrEmpty(pack.name))
            {
                LogHelper.Warning("Asset pack has empty name, skipping.");
                return;
            }

            if (!Regex.IsMatch(pack.name, @"^[a-zA-Z][a-zA-Z0-9_\-]*$"))
            {
                LogHelper.Warning("Asset pack name '" + pack.name +
                                  "' contains invalid characters. Only letters, digits, underscores and hyphens are allowed. Skipping.");
                return;
            }

            var packDir = Path.Combine(gradleRoot, pack.name);
            var assetsDir = Path.Combine(packDir, "src", "main", "assets");

            if (!Directory.Exists(assetsDir))
            {
                Directory.CreateDirectory(assetsDir);
            }

            if (!string.IsNullOrEmpty(pack.streamingAssetsPath))
            {
                var streamingRoot = Path.Combine(
                    gradleRoot, "unityLibrary", "src", "main", "assets");
                var sourceDir = Path.Combine(streamingRoot, pack.streamingAssetsPath);

                if (Directory.Exists(sourceDir))
                {
                    var destDir = Path.Combine(assetsDir, pack.streamingAssetsPath);
                    if (Directory.Exists(destDir))
                    {
                        Directory.Delete(destDir, true);
                    }

                    Directory.Move(sourceDir, destDir);
                    LogHelper.Log("+ AssetPack '" + pack.name + "': moved " +
                                  pack.streamingAssetsPath + " -> " + destDir);
                }
                else
                {
                    LogHelper.Warning("AssetPack '" + pack.name + "': StreamingAssets subdirectory not found: " +
                                      sourceDir);
                }
            }

            var buildGradlePath = Path.Combine(packDir, "build.gradle");
            var buildGradleContent = GenerateAssetPackBuildGradle(pack.name, pack.deliveryType);
            File.WriteAllText(buildGradlePath, buildGradleContent);
            LogHelper.Log("+ AssetPack '" + pack.name + "': created build.gradle");
        }

        /// <summary>
        /// 生成 Asset Pack 的 build.gradle 内容。
        /// </summary>
        /// <remarks>
        /// Generates the build.gradle content for an asset pack module.
        /// </remarks>
        private static string GenerateAssetPackBuildGradle(string packName, string deliveryType)
        {
            var dt = string.IsNullOrEmpty(deliveryType) ? "install-time" : deliveryType;
            return
                "plugins {\n" +
                "    id 'com.android.asset-pack'\n" +
                "}\n" +
                "\n" +
                "assetPack {\n" +
                "    packName = '" + packName + "'\n" +
                "    dynamicDelivery {\n" +
                "        deliveryType = '" + dt + "'\n" +
                "    }\n" +
                "}\n";
        }

        /// <summary>
        /// 向 launcher/build.gradle 末尾追加 assetPacks 声明。
        /// </summary>
        /// <remarks>
        /// Appends asset pack include statements to the end of settings.gradle.
        /// </remarks>
        private static void InjectAssetPackIncludes(string gradleRoot, List<AssetPackConfig> packs)
        {
            var filePath = FindFile(gradleRoot, "settings.gradle");
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Warning("Could not find settings.gradle for asset pack includes");
                return;
            }

            var lines = new List<string>(File.ReadAllLines(filePath));
            var changed = false;

            foreach (var pack in packs)
            {
                var includeLine = "include ':" + pack.name + "'";

                if (ContainsLine(lines, includeLine))
                {
                    continue;
                }

                lines.Add(includeLine);
                changed = true;
                LogHelper.Log("+ settings.gradle: " + includeLine);
            }

            if (changed)
            {
                File.WriteAllLines(filePath, lines);
            }
        }

        /// <summary>
        /// 向 launcher/build.gradle 末尾追加 assetPacks 声明。
        /// </summary>
        /// <remarks>
        /// Appends assetPacks declaration to the end of launcher/build.gradle.
        /// </remarks>
        private static void InjectAssetPacksToLauncher(string gradleRoot, List<AssetPackConfig> packs)
        {
            var filePath = FindFile(gradleRoot, Path.Combine("launcher", "build.gradle"));
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Warning("Could not find launcher/build.gradle for assetPacks injection");
                return;
            }

            var lines = new List<string>(File.ReadAllLines(filePath));

            var packRefs = new List<string>();
            foreach (var pack in packs)
            {
                packRefs.Add("':" + pack.name + "'");
            }

            var assetPacksLine = "assetPacks = [" + string.Join(", ", packRefs) + "]";

            if (ContainsLine(lines, "assetPacks"))
            {
                return;
            }

            lines.Add("");
            lines.Add("android {");
            lines.Add("    " + assetPacksLine);
            lines.Add("}");

            File.WriteAllLines(filePath, lines);
            LogHelper.Log("+ launcher/build.gradle: added android { " + assetPacksLine + " }");
        }
    }
}

#endif
