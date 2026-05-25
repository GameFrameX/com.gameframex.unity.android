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
        /// 向 launcher 和 unityLibrary 的 res/values/strings.xml 中注入多类型 values 资源，
        /// 以及向 launcher 的 res/values-<locale>/strings.xml 注入本地化 string 资源。
        /// </summary>
        /// <remarks>
        /// Injects multi-type values resources into res/values/strings.xml of both launcher and unityLibrary,
        /// and localized string resources into launcher's res/values-<locale>/strings.xml.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetValuesResources(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.launcher != null && config.launcher.resources.Count > 0)
            {
                var stringsPath = FindFile(gradleRoot, Path.Combine("launcher", "src", "main", "res", "values", "strings.xml"));
                if (!string.IsNullOrEmpty(stringsPath))
                {
                    InjectResources(stringsPath, config.launcher.resources);
                }
                else
                {
                    var dir = Path.Combine(gradleRoot, "launcher", "src", "main", "res", "values");
                    stringsPath = Path.Combine(dir, "strings.xml");
                    CreateAndInjectResources(stringsPath, config.launcher.resources);
                }
            }

            if (config.unityLibrary != null && config.unityLibrary.resources.Count > 0)
            {
                var stringsPath = FindFile(gradleRoot, Path.Combine("unityLibrary", "src", "main", "res", "values", "strings.xml"));
                if (!string.IsNullOrEmpty(stringsPath))
                {
                    InjectResources(stringsPath, config.unityLibrary.resources);
                }
                else
                {
                    var dir = Path.Combine(gradleRoot, "unityLibrary", "src", "main", "res", "values");
                    stringsPath = Path.Combine(dir, "strings.xml");
                    CreateAndInjectResources(stringsPath, config.unityLibrary.resources);
                }
            }

            if (config.localizedStringResources != null && config.localizedStringResources.Count > 0)
            {
                foreach (var localeKvp in config.localizedStringResources)
                {
                    if (string.IsNullOrEmpty(localeKvp.Key) || localeKvp.Value == null || localeKvp.Value.Count == 0)
                    {
                        continue;
                    }

                    var localeDir = Path.Combine(gradleRoot, "launcher", "src", "main", "res", "values-" + localeKvp.Key);
                    var localePath = Path.Combine(localeDir, "strings.xml");
                    var existingPath = FindFile(gradleRoot, Path.Combine("launcher", "src", "main", "res", "values-" + localeKvp.Key, "strings.xml"));
                    if (!string.IsNullOrEmpty(existingPath))
                    {
                        InjectStringResourcesSimple(existingPath, localeKvp.Value);
                    }
                    else
                    {
                        CreateAndInjectStringResourcesSimple(localePath, localeKvp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 向已存在的 strings.xml 中注入多类型 values 资源，同名 name 会被覆盖。
        /// </summary>
        /// <remarks>
        /// Injects multi-type values resources into an existing strings.xml, overriding by name.
        /// </remarks>
        /// <param name="filePath">strings.xml 文件路径 / Path to strings.xml</param>
        /// <param name="resources">要注入的资源映射 / Resource map to inject</param>
        private static void InjectResources(string filePath, Dictionary<string, Dictionary<string, ResourceValue>> resources)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            var changed = false;

            foreach (var typeKvp in resources)
            {
                var typeName = typeKvp.Key;
                foreach (var resKvp in typeKvp.Value)
                {
                    if (string.IsNullOrEmpty(resKvp.Key))
                    {
                        continue;
                    }

                    var xpath = "//" + typeName + "[@name='" + resKvp.Key + "']";
                    var existing = doc.SelectSingleNode(xpath);
                    if (existing != null)
                    {
                        if (UpdateResourceElement(existing, resKvp.Value))
                        {
                            changed = true;
                            LogHelper.Log("~ strings.xml: " + typeName + " " + resKvp.Key + "=" + resKvp.Value.text);
                        }
                    }
                    else
                    {
                        var elem = CreateResourceElement(doc, typeName, resKvp.Key, resKvp.Value);
                        doc.DocumentElement.AppendChild(elem);
                        changed = true;
                        LogHelper.Log("+ strings.xml: " + typeName + " " + resKvp.Key + "=" + resKvp.Value.text);
                    }
                }
            }

            if (changed)
            {
                SaveXmlDocument(doc, filePath);
            }
        }

        /// <summary>
        /// 创建新的 strings.xml 并写入多类型 values 资源。
        /// </summary>
        /// <remarks>
        /// Creates a new strings.xml and writes multi-type values resources into it.
        /// </remarks>
        /// <param name="filePath">strings.xml 文件路径 / Path to strings.xml</param>
        /// <param name="resources">要写入的资源映射 / Resource map to write</param>
        private static void CreateAndInjectResources(string filePath, Dictionary<string, Dictionary<string, ResourceValue>> resources)
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

            foreach (var typeKvp in resources)
            {
                var typeName = typeKvp.Key;
                foreach (var resKvp in typeKvp.Value)
                {
                    if (string.IsNullOrEmpty(resKvp.Key))
                    {
                        continue;
                    }

                    var elem = CreateResourceElement(doc, typeName, resKvp.Key, resKvp.Value);
                    root.AppendChild(elem);
                    LogHelper.Log("+ strings.xml (new): " + typeName + " " + resKvp.Key + "=" + resKvp.Value.text);
                }
            }

            if (root.ChildNodes.Count == 0)
            {
                return;
            }

            SaveXmlDocument(doc, filePath);
        }

        /// <summary>
        /// 向已存在的 strings.xml 中注入简单 string 资源（用于本地化，无额外属性）。
        /// </summary>
        /// <remarks>
        /// Injects simple string resources into an existing strings.xml (for localization, no extra attributes).
        /// </remarks>
        private static void InjectStringResourcesSimple(string filePath, Dictionary<string, string> resources)
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
                        LogHelper.Log("~ strings.xml [" + Path.GetFileName(Path.GetDirectoryName(filePath)) + "]: string " + kvp.Key + "=" + kvp.Value);
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
                    LogHelper.Log("+ strings.xml [" + Path.GetFileName(Path.GetDirectoryName(filePath)) + "]: string " + kvp.Key + "=" + kvp.Value);
                }
            }

            if (changed)
            {
                SaveXmlDocument(doc, filePath);
            }
        }

        /// <summary>
        /// 创建新的 strings.xml 并写入简单 string 资源（用于本地化）。
        /// </summary>
        /// <remarks>
        /// Creates a new strings.xml and writes simple string resources into it (for localization).
        /// </remarks>
        private static void CreateAndInjectStringResourcesSimple(string filePath, Dictionary<string, string> resources)
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
                LogHelper.Log("+ strings.xml (new) [" + Path.GetFileName(Path.GetDirectoryName(filePath)) + "]: string " + kvp.Key + "=" + kvp.Value);
            }

            if (root.ChildNodes.Count == 0)
            {
                return;
            }

            SaveXmlDocument(doc, filePath);
        }

        private static XmlElement CreateResourceElement(XmlDocument doc, string typeName, string name, ResourceValue rv)
        {
            var elem = doc.CreateElement(typeName);
            var nameAttr = doc.CreateAttribute("name");
            nameAttr.Value = name;
            elem.Attributes.Append(nameAttr);
            elem.InnerText = rv.text;

            foreach (var attrKvp in rv.attributes)
            {
                var attr = doc.CreateAttribute(attrKvp.Key);
                attr.Value = attrKvp.Value;
                elem.Attributes.Append(attr);
            }

            return elem;
        }

        private static bool UpdateResourceElement(XmlNode existing, ResourceValue rv)
        {
            var changed = false;

            if (existing.InnerText != rv.text)
            {
                existing.InnerText = rv.text;
                changed = true;
            }

            if (existing.Attributes == null)
            {
                return changed;
            }

            foreach (var attrKvp in rv.attributes)
            {
                var existingAttr = existing.Attributes[attrKvp.Key];
                if (existingAttr != null)
                {
                    if (existingAttr.Value != attrKvp.Value)
                    {
                        existingAttr.Value = attrKvp.Value;
                        changed = true;
                    }
                }
                else
                {
                    var attr = existing.OwnerDocument.CreateAttribute(attrKvp.Key);
                    attr.Value = attrKvp.Value;
                    existing.Attributes.Append(attr);
                    changed = true;
                }
            }

            return changed;
        }

        private static void SaveXmlDocument(XmlDocument doc, string filePath)
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
}

#endif
