// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

#if UNITY_ANDROID && UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace GameFrameX.Android.Editor
{
    /// <summary>
    /// Android 构建配置文件的数据模型，对应 AndroidBuildConfig.json 的反序列化结构。
    /// </summary>
    /// <remarks>
    /// Data model for Android build config files, mapped from AndroidBuildConfig.json deserialization.
    /// </remarks>
    [Serializable]
    internal class AndroidBuildConfigFile
    {
        /// <summary>
        /// 配置提供者名称，仅用于日志标识。
        /// </summary>
        /// <remarks>
        /// Config provider name, used for logging only.
        /// </remarks>
        public string providerName;

        /// <summary>
        /// Maven 仓库列表，注入到根级 settings.gradle。
        /// </summary>
        /// <remarks>
        /// List of Maven repositories, injected into the root-level settings.gradle.
        /// </remarks>
        public List<MavenRepository> mavenRepositories = new List<MavenRepository>();

        /// <summary>
        /// Gradle Wrapper 配置，注入到 gradle-wrapper.properties。
        /// </summary>
        /// <remarks>
        /// Gradle Wrapper config, injected into gradle-wrapper.properties.
        /// </remarks>
        public Dictionary<string, string> gradleWrapper = new Dictionary<string, string>();

        /// <summary>
        /// Gradle 属性配置，注入到 gradle.properties。
        /// </summary>
        /// <remarks>
        /// Gradle properties config, injected into gradle.properties.
        /// </remarks>
        public Dictionary<string, string> gradleProperties = new Dictionary<string, string>();

        /// <summary>
        /// 文件复制映射，key 为源路径，value 为目标路径。支持相对路径和绝对路径。
        /// 相对源路径基于配置文件所在目录解析，相对目标路径基于 Gradle 根目录解析。
        /// </summary>
        /// <remarks>
        /// File copy map, key = source path, value = destination path. Supports both
        /// relative and absolute paths. Relative source is resolved from the config
        /// file directory, relative destination from the Gradle project root.
        /// </remarks>
        public Dictionary<string, string> fileCopies = new Dictionary<string, string>();

        /// <summary>
        /// 目录复制映射，key 为源路径，value 为目标路径。支持相对路径和绝对路径。
        /// 相对源路径基于配置文件所在目录解析，相对目标路径基于 Gradle 根目录解析。
        /// </summary>
        /// <remarks>
        /// Directory copy map, key = source path, value = destination path. Supports
        /// both relative and absolute paths. Relative source is resolved from the
        /// config file directory, relative destination from the Gradle project root.
        /// </remarks>
        public Dictionary<string, string> directoryCopies = new Dictionary<string, string>();

        /// <summary>
        /// Asset Pack 配置列表，用于 Google Play Asset Delivery。
        /// 每个 Asset Pack 对应一个独立的 Gradle 子项目模块。
        /// </summary>
        /// <remarks>
        /// Asset pack configurations for Google Play Asset Delivery.
        /// Each asset pack corresponds to an independent Gradle sub-project module.
        /// </remarks>
        public List<AssetPackConfig> assetPacks = new List<AssetPackConfig>();

        /// <summary>
        /// launcher 模块配置（Android 应用壳）。
        /// </summary>
        /// <remarks>
        /// Launcher module config (Android application shell).
        /// </remarks>
        public AndroidBuildConfigModule launcher;

        /// <summary>
        /// unityLibrary 模块配置（Unity 引擎库）。
        /// </summary>
        /// <remarks>
        /// unityLibrary module config (Unity engine library).
        /// </remarks>
        public AndroidBuildConfigModule unityLibrary;
    }

    /// <summary>
    /// 单个 Gradle 子模块的配置，包含依赖、权限和 meta-data。
    /// </summary>
    /// <remarks>
    /// Configuration for a single Gradle sub-module, containing dependencies, permissions, and meta-data.
    /// </remarks>
    [Serializable]
    internal class AndroidBuildConfigModule
    {
        /// <summary>
        /// Gradle 依赖列表。
        /// </summary>
        /// <remarks>
        /// List of Gradle dependencies.
        /// </remarks>
        public List<GradleDependency> dependencies = new List<GradleDependency>();

        /// <summary>
        /// AndroidManifest 权限列表。
        /// </summary>
        /// <remarks>
        /// List of AndroidManifest permissions.
        /// </remarks>
        public List<string> permissions = new List<string>();

        /// <summary>
        /// AndroidManifest meta-data 列表。
        /// </summary>
        /// <remarks>
        /// List of AndroidManifest meta-data entries.
        /// </remarks>
        public List<ManifestMetaData> metaData = new List<ManifestMetaData>();

        /// <summary>
        /// AndroidManifest application 标签上的属性列表。
        /// </summary>
        /// <remarks>
        /// Attributes on the AndroidManifest application tag.
        /// </remarks>
        public List<ApplicationAttribute> applicationAttributes = new List<ApplicationAttribute>();

        /// <summary>
        /// 编译 SDK 版本，注入到 android {} 块。合并时取最大值。
        /// </summary>
        /// <remarks>
        /// Compile SDK version, injected into android {} block. Merged by taking the maximum value.
        /// </remarks>
        public string compileSdkVersion;

        /// <summary>
        /// 构建工具版本，注入到 android {} 块。合并时后写覆盖。
        /// </summary>
        /// <remarks>
        /// Build tools version, injected into android {} block. Merged by last-writer-wins.
        /// </remarks>
        public string buildToolsVersion;

        /// <summary>
        /// 最低 SDK 版本，注入到 defaultConfig {} 块。合并时取最大值。
        /// </summary>
        /// <remarks>
        /// Minimum SDK version, injected into defaultConfig {} block. Merged by taking the maximum value.
        /// </remarks>
        public string minSdkVersion;

        /// <summary>
        /// 目标 SDK 版本，注入到 defaultConfig {} 块。合并时取最大值。
        /// </summary>
        /// <remarks>
        /// Target SDK version, injected into defaultConfig {} block. Merged by taking the maximum value.
        /// </remarks>
        public string targetSdkVersion;

        /// <summary>
        /// 应用标识符，注入到 defaultConfig {} 块。合并时后写覆盖。
        /// </summary>
        /// <remarks>
        /// Application ID, injected into defaultConfig {} block. Merged by last-writer-wins.
        /// </remarks>
        public string applicationId;

        /// <summary>
        /// 版本名称，注入到 defaultConfig {} 块。合并时后写覆盖。
        /// </summary>
        /// <remarks>
        /// Version name, injected into defaultConfig {} block. Merged by last-writer-wins.
        /// </remarks>
        public string versionName;

        /// <summary>
        /// 签名配置，注入到 build.gradle 的 signingConfigs {} 和 buildTypes {} 块。
        /// </summary>
        /// <remarks>
        /// Signing configuration, injected into signingConfigs {} and buildTypes {} blocks in build.gradle.
        /// </remarks>
        public SigningConfig signingConfig;
    }

    /// <summary>
    /// Maven 仓库信息。
    /// </summary>
    /// <remarks>
    /// Maven repository info.
    /// </remarks>
    [Serializable]
    internal struct MavenRepository
    {
        /// <summary>
        /// 仓库名称（可选）。
        /// </summary>
        /// <remarks>
        /// Repository name (optional).
        /// </remarks>
        public string name;

        /// <summary>
        /// 仓库 URL。
        /// </summary>
        /// <remarks>
        /// Repository URL.
        /// </remarks>
        public string url;
    }

    /// <summary>
    /// Gradle 依赖项。
    /// </summary>
    /// <remarks>
    /// Gradle dependency entry.
    /// </remarks>
    [Serializable]
    internal struct GradleDependency
    {
        /// <summary>
        /// 依赖配置，如 "implementation"、"api"。
        /// </summary>
        /// <remarks>
        /// Dependency configuration, e.g. "implementation", "api".
        /// </remarks>
        public string configuration;

        /// <summary>
        /// 依赖坐标，如 "com.example:sdk:1.0.0"。
        /// </summary>
        /// <remarks>
        /// Dependency notation, e.g. "com.example:sdk:1.0.0".
        /// </remarks>
        public string notation;
    }

    /// <summary>
    /// AndroidManifest 中的 meta-data 条目。
    /// </summary>
    /// <remarks>
    /// AndroidManifest meta-data entry.
    /// </remarks>
    [Serializable]
    internal struct ManifestMetaData
    {
        /// <summary>
        /// meta-data 的 name 属性。
        /// </summary>
        /// <remarks>
        /// The name attribute of the meta-data.
        /// </remarks>
        public string name;

        /// <summary>
        /// meta-data 的 value 属性。
        /// </summary>
        /// <remarks>
        /// The value attribute of the meta-data.
        /// </remarks>
        public string value;
    }

    /// <summary>
    /// AndroidManifest application 标签上的属性。
    /// </summary>
    /// <remarks>
    /// Attribute on the AndroidManifest application tag.
    /// </remarks>
    [Serializable]
    internal class ApplicationAttribute
    {
        /// <summary>
        /// 属性名（不含 android: 前缀）。
        /// </summary>
        /// <remarks>
        /// Attribute name (without android: prefix).
        /// </remarks>
        public string name;

        /// <summary>
        /// 属性值。
        /// </summary>
        /// <remarks>
        /// Attribute value.
        /// </remarks>
        public string value;
    }

    /// <summary>
    /// Android 签名配置。
    /// </summary>
    /// <remarks>
    /// Android signing configuration.
    /// </remarks>
    [Serializable]
    internal class SigningConfig
    {
        /// <summary>
        /// 密钥库文件路径，支持相对路径和绝对路径。
        /// </summary>
        /// <remarks>
        /// Keystore file path, supports relative and absolute paths.
        /// </remarks>
        public string storeFile;

        /// <summary>
        /// 密钥库密码。
        /// </summary>
        /// <remarks>
        /// Keystore password.
        /// </remarks>
        public string storePassword;

        /// <summary>
        /// 密钥别名。
        /// </summary>
        /// <remarks>
        /// Key alias.
        /// </remarks>
        public string keyAlias;

        /// <summary>
        /// 密钥密码。
        /// </summary>
        /// <remarks>
        /// Key password.
        /// </remarks>
        public string keyPassword;
    }

    /// <summary>
    /// Google Play Asset Delivery 的 Asset Pack 配置。
    /// </summary>
    /// <remarks>
    /// Asset pack configuration for Google Play Asset Delivery.
    /// </remarks>
    [Serializable]
    internal struct AssetPackConfig
    {
        /// <summary>
        /// Asset Pack 的模块名称，同时也是 Gradle 子项目目录名。
        /// </summary>
        /// <remarks>
        /// Asset pack module name, also used as the Gradle sub-project directory name.
        /// </remarks>
        public string name;

        /// <summary>
        /// 投递类型：install-time、fast-follow 或 on-demand。
        /// </summary>
        /// <remarks>
        /// Delivery type: install-time, fast-follow, or on-demand.
        /// </remarks>
        public string deliveryType;

        /// <summary>
        /// Unity StreamingAssets 下的子目录名称。
        /// 构建后，该子目录的内容会被移动到 Asset Pack 模块中。
        /// </summary>
        /// <remarks>
        /// Subdirectory under Unity's StreamingAssets.
        /// After build, the contents of this subdirectory are moved into the asset pack module.
        /// </remarks>
        public string streamingAssetsPath;
    }
}

#endif
