// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GameFrameX.Android.Editor
{
    /// <summary>
    /// Android 构建后处理的主编排器，负责配置发现、合并和注入。
    /// </summary>
    /// <remarks>
    /// Main orchestrator for Android post-build processing, handling config discovery, merging, and injection.
    /// </remarks>
    internal partial class PostProcessBuildHelper : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get { return ushort.MaxValue; }
        }

        void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
        {
            try
            {
                Run(path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 主执行流程：发现配置、注入 Gradle 和 Manifest。
        /// </summary>
        /// <remarks>
        /// Main execution flow: discovers config, injects Gradle and Manifest.
        /// </remarks>
        /// <param name="path">Gradle 项目路径 / Gradle project path</param>
        private static void Run(string path)
        {
            LogHelper.Log("OnPostGenerateGradleAndroidProject triggered. Path: " + path);

            var config = RunDiscovery();
            if (config == null)
            {
                LogHelper.Log("No configuration to apply.");
                return;
            }

            var gradleRoot = FindGradleRoot(path);
            if (string.IsNullOrEmpty(gradleRoot))
            {
                LogHelper.Error("Could not locate Gradle project root from: " + path);
                return;
            }

            LogHelper.Log("Gradle root: " + gradleRoot);

            SetSettingsGradle(gradleRoot, config);
            SetAndroidBlock(gradleRoot, config);
            SetBuildGradle(gradleRoot, config);
            SetManifest(gradleRoot, config);
            SetGradleWrapper(gradleRoot, config);
            SetGradleProperties(gradleRoot, config);
            CopyFiles(gradleRoot, config);
        }

        /// <summary>
        /// 在 Gradle 根目录中查找指定相对路径的文件。
        /// </summary>
        /// <remarks>
        /// Finds a file by relative path within the Gradle root directory.
        /// </remarks>
        /// <param name="root">Gradle 根目录 / Gradle root directory</param>
        /// <param name="relativePath">相对路径 / Relative path</param>
        /// <returns>文件完整路径，未找到返回 null / Full file path, or null if not found</returns>
        private static string FindFile(string root, string relativePath)
        {
            var fullPath = Path.Combine(root, relativePath);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = Path.Combine(root, relativePath.Replace('\\', '/'));
            return File.Exists(fullPath) ? fullPath : null;
        }

        /// <summary>
        /// 检查行列表中是否包含指定搜索词。
        /// </summary>
        /// <remarks>
        /// Checks if any line in the list contains the search term.
        /// </remarks>
        /// <param name="lines">行列表 / List of lines</param>
        /// <param name="searchTerm">搜索词 / Search term</param>
        /// <returns>是否包含 / Whether the term is found</returns>
        private static bool ContainsLine(System.Collections.Generic.List<string> lines, string searchTerm)
        {
            foreach (var line in lines)
            {
                if (line.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 向上查找 Gradle 项目根目录（包含 settings.gradle 的目录）。
        /// </summary>
        /// <remarks>
        /// Walks up the directory tree to find the Gradle root (directory containing settings.gradle).
        /// </remarks>
        /// <param name="path">起始路径 / Starting path</param>
        /// <returns>Gradle 根目录路径，未找到返回 null / Gradle root path, or null if not found</returns>
        private static string FindGradleRoot(string path)
        {
            var dir = path;

            for (var i = 0; i < 5; i++)
            {
                if (File.Exists(Path.Combine(dir, "settings.gradle")))
                {
                    return dir;
                }

                var parent = Directory.GetParent(dir);
                if (parent == null)
                {
                    break;
                }

                dir = parent.FullName;
            }

            return null;
        }
    }
}

#endif