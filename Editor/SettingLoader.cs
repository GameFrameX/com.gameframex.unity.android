// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace GameFrameX.Android.Editor
{
    /// <summary>
    /// 使用 AssetDatabase 发现项目中匹配指定文件名的配置文件。
    /// </summary>
    /// <remarks>
    /// Uses AssetDatabase to discover config files matching a specified filename in the project.
    /// </remarks>
    internal static class SettingLoader
    {
        /// <summary>
        /// 加载所有匹配的配置文件路径。
        /// </summary>
        /// <remarks>
        /// Loads all matching config file paths.
        /// </remarks>
        /// <param name="fileName">目标文件名 / Target file name</param>
        /// <returns>匹配的文件路径列表 / List of matched file paths</returns>
        public static List<string> LoadSettingsData(string fileName)
        {
            var guids = AssetDatabase.FindAssets("t:textasset");
            var results = new List<string>(16);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var newFileName = Path.GetFileName(path);

                if (newFileName.Equals(fileName, System.StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(path);
                }
            }

            results.Sort();
            return results;
        }
    }
}

#endif
