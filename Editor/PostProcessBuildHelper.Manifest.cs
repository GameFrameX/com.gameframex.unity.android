// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System.IO;
using System.Xml;

namespace GameFrameX.Android.Editor
{
    internal partial class PostProcessBuildHelper
    {
        /// <summary>
        /// 向 launcher 和 unityLibrary 的 AndroidManifest.xml 中注入权限和 meta-data。
        /// </summary>
        /// <remarks>
        /// Injects permissions and meta-data into AndroidManifest.xml of both launcher and unityLibrary.
        /// </remarks>
        /// <param name="gradleRoot">Gradle 项目根目录 / Gradle project root</param>
        /// <param name="config">合并后的配置 / Merged config</param>
        private static void SetManifest(string gradleRoot, AndroidBuildConfigFile config)
        {
            if (config.launcher != null && (config.launcher.permissions.Count > 0 || config.launcher.metaData.Count > 0))
            {
                var manifestPath = FindFile(gradleRoot, Path.Combine("launcher", "src", "main", "AndroidManifest.xml"));
                if (!string.IsNullOrEmpty(manifestPath))
                {
                    InjectManifestEntries(manifestPath, config.launcher);
                }
                else
                {
                    LogHelper.Warning("Could not find launcher/src/main/AndroidManifest.xml");
                }
            }

            if (config.unityLibrary != null && (config.unityLibrary.permissions.Count > 0 || config.unityLibrary.metaData.Count > 0))
            {
                var manifestPath = FindFile(gradleRoot, Path.Combine("unityLibrary", "src", "main", "AndroidManifest.xml"));
                if (!string.IsNullOrEmpty(manifestPath))
                {
                    InjectManifestEntries(manifestPath, config.unityLibrary);
                }
                else
                {
                    LogHelper.Warning("Could not find unityLibrary/src/main/AndroidManifest.xml");
                }
            }
        }

        /// <summary>
        /// 向单个 AndroidManifest.xml 中注入权限和 meta-data。
        /// </summary>
        /// <remarks>
        /// Injects permissions and meta-data into a single AndroidManifest.xml.
        /// </remarks>
        /// <param name="filePath">AndroidManifest.xml 文件路径 / Path to AndroidManifest.xml</param>
        /// <param name="module">模块配置 / Module config</param>
        private static void InjectManifestEntries(string filePath, AndroidBuildConfigModule module)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            var changed = false;

            if (module.permissions.Count > 0)
            {
                var manifestNode = doc.SelectSingleNode("//manifest") as XmlElement;
                if (manifestNode != null)
                {
                    foreach (var perm in module.permissions)
                    {
                        if (string.IsNullOrEmpty(perm))
                        {
                            continue;
                        }

                        var existing = doc.SelectSingleNode(
                            "//uses-permission[@android:name='" + perm + "']",
                            CreateNamespaceManager(doc));
                        if (existing != null)
                        {
                            continue;
                        }

                        var elem = doc.CreateElement("uses-permission");
                        var attr = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                        attr.Value = perm;
                        elem.Attributes.Append(attr);
                        manifestNode.AppendChild(elem);
                        changed = true;
                        LogHelper.Log("+ AndroidManifest: uses-permission " + perm);
                    }
                }
            }

            if (module.metaData.Count > 0)
            {
                var appNode = doc.SelectSingleNode("//application") as XmlElement;
                if (appNode != null)
                {
                    foreach (var meta in module.metaData)
                    {
                        if (string.IsNullOrEmpty(meta.name))
                        {
                            continue;
                        }

                        var existing = doc.SelectSingleNode(
                            "//meta-data[@android:name='" + meta.name + "']",
                            CreateNamespaceManager(doc));
                        if (existing != null)
                        {
                            continue;
                        }

                        var elem = doc.CreateElement("meta-data");
                        var nameAttr = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                        nameAttr.Value = meta.name;
                        elem.Attributes.Append(nameAttr);
                        var valueAttr = doc.CreateAttribute("android", "value", "http://schemas.android.com/apk/res/android");
                        valueAttr.Value = meta.value;
                        elem.Attributes.Append(valueAttr);
                        appNode.AppendChild(elem);
                        changed = true;
                        LogHelper.Log("+ AndroidManifest: meta-data " + meta.name + "=" + meta.value);
                    }
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

        private static XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
        {
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");
            return nsmgr;
        }
    }
}

#endif
