// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.IO;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 将配置中声明的文件或目录复制到 Gradle 项目中。
        /// </summary>
        /// <remarks>
        /// Copies files or directories declared in config into the Gradle project.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void CopyFiles(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.fileCopies != null)
            {
                foreach (var entry in config.fileCopies)
                {
                    if (string.IsNullOrEmpty(entry.source) || string.IsNullOrEmpty(entry.destination))
                    {
                        continue;
                    }

                    if (!File.Exists(entry.source))
                    {
                        LogHelper.Warning("File copy source not found or not a file: " + entry.source);
                        continue;
                    }

                    var destPath = Path.Combine(gradleRoot, entry.destination);
                    var dir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.Copy(entry.source, destPath, true);
                    LogHelper.Log("+ Copy file: " + Path.GetFileName(entry.source) + " -> " + entry.destination);
                }
            }

            if (config.directoryCopies != null)
            {
                foreach (var entry in config.directoryCopies)
                {
                    if (string.IsNullOrEmpty(entry.source) || string.IsNullOrEmpty(entry.destination))
                    {
                        continue;
                    }

                    if (!Directory.Exists(entry.source))
                    {
                        LogHelper.Warning("Directory copy source not found or not a directory: " + entry.source);
                        continue;
                    }

                    var destPath = Path.Combine(gradleRoot, entry.destination);
                    CopyDirectory(entry.source, destPath);
                    LogHelper.Log("+ Copy dir: " + Path.GetFileName(entry.source) + " -> " + entry.destination);
                }
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(destDir, fileName), true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(dir);
                CopyDirectory(dir, Path.Combine(destDir, dirName));
            }
        }
    }
}

#endif
