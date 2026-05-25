// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 向 launcher 和 unityLibrary 的 res/values/strings.xml 中注入 string 资源。
        /// </summary>
        /// <remarks>
        /// Injects string resources into res/values/strings.xml of both launcher and unityLibrary.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetStringsXml(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.launcher != null && config.launcher.stringResources.Count > 0)
            {
                var stringsPath = FindFile(gradleRoot, Path.Combine("launcher", "src", "main", "res", "values", "strings.xml"));
                if (!string.IsNullOrEmpty(stringsPath))
                {
                    InjectStringResources(stringsPath, config.launcher.stringResources);
                }
                else
                {
                    var dir = Path.Combine(gradleRoot, "launcher", "src", "main", "res", "values");
                    stringsPath = Path.Combine(dir, "strings.xml");
                    CreateAndInjectStringResources(stringsPath, config.launcher.stringResources);
                }
            }

            if (config.unityLibrary != null && config.unityLibrary.stringResources.Count > 0)
            {
                var stringsPath = FindFile(gradleRoot, Path.Combine("unityLibrary", "src", "main", "res", "values", "strings.xml"));
                if (!string.IsNullOrEmpty(stringsPath))
                {
                    InjectStringResources(stringsPath, config.unityLibrary.stringResources);
                }
                else
                {
                    var dir = Path.Combine(gradleRoot, "unityLibrary", "src", "main", "res", "values");
                    stringsPath = Path.Combine(dir, "strings.xml");
                    CreateAndInjectStringResources(stringsPath, config.unityLibrary.stringResources);
                }
            }
        }

        /// <summary>
        /// 向已存在的 strings.xml 中注入 string 资源，同名 name 会被覆盖。
        /// </summary>
        /// <remarks>
        /// Injects string resources into an existing strings.xml, overriding by name.
        /// </remarks>
        /// <param name="filePath">strings.xml 文件路径 / Path to strings.xml</param>
        /// <param name="resources">要注入的 string 资源映射 / String resource map to inject</param>
        private static void InjectStringResources(string filePath, Dictionary<string, string> resources)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            var changed = false;

            foreach (var kvp in resources)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    continue;
                }

                var existing = doc.SelectSingleNode("//string[@name='" + kvp.Key + "']");
                if (existing != null)
                {
                    if (existing.InnerText != kvp.Value)
                    {
                        existing.InnerText = kvp.Value;
                        changed = true;
                        LogHelper.Log("~ strings.xml: string " + kvp.Key + "=" + kvp.Value);
                    }
                }
                else
                {
                    var elem = doc.CreateElement("string");
                    var nameAttr = doc.CreateAttribute("name");
                    nameAttr.Value = kvp.Key;
                    elem.Attributes.Append(nameAttr);
                    elem.InnerText = kvp.Value;
                    doc.DocumentElement.AppendChild(elem);
                    changed = true;
                    LogHelper.Log("+ strings.xml: string " + kvp.Key + "=" + kvp.Value);
                }
            }

            if (changed)
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    NewLineChars = "\n",
                    OmitXmlDeclaration = false,
                };
                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    doc.Save(writer);
                }
            }
        }

        /// <summary>
        /// 创建新的 strings.xml 并写入 string 资源。
        /// </summary>
        /// <remarks>
        /// Creates a new strings.xml and writes string resources into it.
        /// </remarks>
        /// <param name="filePath">strings.xml 文件路径 / Path to strings.xml</param>
        /// <param name="resources">要写入的 string 资源映射 / String resource map to write</param>
        private static void CreateAndInjectStringResources(string filePath, Dictionary<string, string> resources)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);
            var root = doc.CreateElement("resources");
            doc.AppendChild(root);

            foreach (var kvp in resources)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    continue;
                }

                var elem = doc.CreateElement("string");
                var nameAttr = doc.CreateAttribute("name");
                nameAttr.Value = kvp.Key;
                elem.InnerText = kvp.Value;
                elem.Attributes.Append(nameAttr);
                root.AppendChild(elem);
                LogHelper.Log("+ strings.xml (new): string " + kvp.Key + "=" + kvp.Value);
            }

            if (root.ChildNodes.Count == 0)
            {
                return;
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\n",
                OmitXmlDeclaration = false,
            };
            using (var writer = XmlWriter.Create(filePath, settings))
            {
                doc.Save(writer);
            }
        }
    }
}

#endif
