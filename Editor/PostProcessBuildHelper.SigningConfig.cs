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
            var storeFilePath = signingConfig.storeFile.Replace('\\', '/');

            if (ContainsLine(lines, "storeFile file(\"" + storeFilePath + "\")"))
            {
                LogHelper.Log(module + "/build.gradle already contains this signing config, skipping");
                return;
            }

            // Append new android {} block at end of file.
            // Gradle merges multiple android {} blocks, later declarations override earlier ones.
            lines.Add("");
            lines.Add("android {");
            lines.Add("    signingConfigs {");
            lines.Add("        release {");
            lines.Add("            storeFile file(\"" + storeFilePath + "\")");
            lines.Add("            storePassword \"" + signingConfig.storePassword + "\"");
            lines.Add("            keyAlias \"" + signingConfig.keyAlias + "\"");
            lines.Add("            keyPassword \"" + signingConfig.keyPassword + "\"");
            lines.Add("        }");
            lines.Add("    }");
            lines.Add("    buildTypes {");
            lines.Add("        release {");
            lines.Add("            signingConfig signingConfigs.release");
            lines.Add("        }");
            lines.Add("    }");
            lines.Add("}");

            File.WriteAllLines(filePath, lines);
            LogHelper.Log("Injected signing config into " + module + "/build.gradle");
        }
    }
}

#endif
