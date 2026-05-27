<div align="center">

  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />

  # Game Frame X Android

  [![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.android?label=version)](https://github.com/GameFrameX/com.gameframex.unity.android/releases)
  [![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.android?label=license)](https://github.com/GameFrameX/com.gameframex.unity.android/blob/main/LICENSE.md)
  [![Documentation](https://img.shields.io/badge/documentation-docs-blue)](https://gameframex.doc.alianblank.com)

  独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

  [文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始)

  [English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## 项目简介

通用的 Android 构建自动配置系统。通过 JSON 数据文件驱动，自动发现项目中所有 `AndroidBuildConfig.json` 并在构建时注入 Maven 仓库、Gradle 依赖、AndroidManifest 配置、string 资源和本地化配置。

核心特性：

- **纯数据驱动**：SDK 包只需 JSON 文件，无需 C# 代码
- **多文件支持**：项目中可存在任意数量的配置文件
- **任意位置**：配置文件可放在 Unity 能识别的任何位置
- **自动合并去重**：多个配置文件按 key 自动合并去重
- **幂等操作**：多次构建安全，已有条目自动跳过

## 快速开始

### 安装

编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：

```json
{
  "scopedRegistries": [
    {
      "name": "GameFrameX",
      "url": "https://gameframex.upm.alianblank.uk",
      "scopes": [
        "com.gameframex"
      ]
    }
  ],
  "dependencies": {
    "com.gameframex.unity.android": "1.0.0"
  }
}
```

`scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

### 使用示例

在项目的任意位置放置 `AndroidBuildConfig.json` 文件（如 `Assets/`、`Packages/` 下的任何目录）：

```json
{
  "providerName": "My SDK",
  "mavenRepositories": [
    { "name": "MyRepo", "url": "https://maven.example.com/repository/" }
  ],
  "gradleWrapper": {
    "distributionUrl": "https\\://services.gradle.org/distributions/gradle-8.4-bin.zip"
  },
  "gradleProperties": {
    "android.useAndroidX": "true",
    "org.gradle.jvmargs": "-Xmx4096m"
  },
  "fileCopies": {
    "libs/your-sdk.aar": "launcher/libs/your-sdk.aar"
  },
  "directoryCopies": {
    "jniLibs": "launcher/jniLibs"
  },
  "assetPacksEnabled": true,
  "assetPacks": [
    {
      "name": "additional_assets",
      "deliveryType": "install-time",
      "streamingAssetsPath": "AdditionalAssets"
    }
  ],
  "localizedStringResources": {
    "zh-rCN": {
      "app_name": "我的应用"
    },
    "ja": {
      "app_name": "マイアプリ"
    }
  },
  "launcher": {
    "compileSdkVersion": "34",
    "buildToolsVersion": "34.0.0",
    "minSdkVersion": "24",
    "targetSdkVersion": "34",
    "applicationId": "com.example.demo",
    "versionName": "1.0.0",
    "dependencies": [
      { "configuration": "implementation", "notation": "com.example:sdk-analytics:2.0.0" }
    ],
    "permissions": [ "android.permission.INTERNET" ],
    "metaData": [
      { "name": "com.example.APP_ID", "value": "your_app_id" }
    ],
    "applicationAttributes": [
      { "name": "usesCleartextTraffic", "value": "true" }
    ],
    "signingConfig": {
      "storeFile": "keystore/release.jks",
      "storePassword": "your_store_password",
      "keyAlias": "release",
      "keyPassword": "your_key_password"
    },
    "stringResources": {
      "app_name": "My App",
      "facebook_app_id": "your_facebook_app_id",
      "facebook_client_token": "your_facebook_client_token",
      "facebook_app_scheme": "fb_your_facebook_app_id"
    },
    "resources": {
      "string": {
        "facebook_app_id": {
          "value": "your_facebook_app_id",
          "translatable": "false"
        }
      },
      "integer": {
        "max_retry_count": "3"
      },
      "color": {
        "primary": "#FF5722"
      }
    }
  },
  "unityLibrary": {
    "compileSdkVersion": "34",
    "buildToolsVersion": "34.0.0",
    "minSdkVersion": "24",
    "targetSdkVersion": "34",
    "dependencies": [],
    "permissions": [],
    "metaData": [],
    "applicationAttributes": [],
    "stringResources": {},
    "resources": {}
  }
}
```

所有字段均为可选。项目中可放置多个配置文件，内容自动合并去重。

### 字段说明

#### 全局字段

| 字段 | 类型 | 合并策略 | 说明 |
|------|------|---------|------|
| `providerName` | string | 后写覆盖 | 配置提供者名称，仅用于构建日志标识 |
| `mavenRepositories` | `[{name, url}]` | 按 URL 去重 | Maven 仓库列表，注入到根 `settings.gradle` 的 `dependencyResolutionManagement.repositories` |
| `gradleWrapper` | `{key: value}` | 按 key 覆盖 | 键值对注入到 `gradle-wrapper.properties` |
| `gradleProperties` | `{key: value}` | 按 key 覆盖 | 键值对注入到 `gradle.properties` |
| `fileCopies` | `{源路径: 目标路径}` | 按 source 覆盖 | 文件复制映射。相对源路径基于配置文件目录解析，相对目标路径基于 Gradle 根目录解析 |
| `directoryCopies` | `{源路径: 目标路径}` | 按 source 覆盖 | 目录复制映射，路径解析同 `fileCopies` |
| `assetPacksEnabled` | bool | 后写覆盖 | 是否启用 Google Play Asset Delivery 功能，默认 `false` |
| `assetPacks` | `[{name, deliveryType, streamingAssetsPath}]` | 按 name 去重 | Asset Pack 配置列表，每个 pack 对应一个独立的 Gradle 子项目 |
| `localizedStringResources` | `{locale: {key: value}}` | 按 locale 分组，每组内按 key 后写覆盖 | 本地化 string 资源，注入到 launcher 的 `res/values-<locale>/strings.xml` |

#### 模块字段（launcher / unityLibrary）

| 字段 | 类型 | 合并策略 | 说明 |
|------|------|---------|------|
| `compileSdkVersion` | string | 取最大值 | 编译 SDK 版本，注入到 `android {}` 块 |
| `buildToolsVersion` | string | 后写覆盖 | 构建工具版本 |
| `minSdkVersion` | string | 取最大值 | 最低 SDK 版本 |
| `targetSdkVersion` | string | 取最大值 | 目标 SDK 版本 |
| `applicationId` | string | 后写覆盖 | 应用包名（仅 launcher 有效） |
| `versionName` | string | 后写覆盖 | 版本名称 |
| `dependencies` | `[{configuration, notation}]` | 按 `configuration:notation` 去重 | Gradle 依赖列表。`configuration` 可为 `implementation`、`api` 等 |
| `permissions` | `[string]` | 按名称去重 | AndroidManifest `<uses-permission>` 权限列表 |
| `metaData` | `[{name, value}]` | 按 name 去重 | AndroidManifest `<application>` 下的 `<meta-data>` 条目 |
| `applicationAttributes` | `[{name, value}]` | 按名称去重 | AndroidManifest `<application>` 标签上的属性（不含 `android:` 前缀） |
| `signingConfig` | `{storeFile, storePassword, keyAlias, keyPassword}` | 后写覆盖 | 签名配置，注入到 `signingConfigs.release` 和 `buildTypes.release` |
| `stringResources` | `{key: value}` | 按 key 后写覆盖 | （旧版）string 资源，注入到 `res/values/strings.xml`。自动合并到 `resources.string` |
| `resources` | `{type: {key: value/object}}` | 按 type 分组，每组内按 key 后写覆盖 | 多类型 values 资源，注入到 `res/values/strings.xml`。参见下方「资源值格式」 |

## 配置参考

### 资源值格式

`resources` 字段使用嵌套字典：外层 key = XML 元素类型，内层 key = 资源名，内层 value = 文本或对象。

**纯字符串值**（无额外 XML 属性）：
```json
"resources": {
  "string": { "app_name": "My App" },
  "integer": { "max_retry_count": "3" }
}
```
产出：`<string name="app_name">My App</string>` 和 `<integer name="max_retry_count">3</integer>`

**对象值**（带额外 XML 属性）：
```json
"resources": {
  "string": {
    "facebook_app_id": {
      "value": "724358863922328",
      "translatable": "false"
    }
  }
}
```
产出：`<string name="facebook_app_id" translatable="false">724358863922328</string>`

**支持的资源类型**：`string`、`integer`、`bool`、`color`、`dimen`（以及其他 XML 元素类型 — 标签名直接使用）。

**向后兼容**：`stringResources` 会自动合并到 `resources.string`。如果两者声明了相同的 key，`resources` 优先。

### stringResources 常用 Key

| Key | 所属 SDK / 服务 | 说明 |
|-----|----------------|------|
| `app_name` | Android | 应用显示名称（桌面图标名称） |
| `facebook_app_id` | Facebook SDK | Facebook 应用 ID |
| `facebook_client_token` | Facebook SDK | Facebook 客户端令牌 |
| `facebook_app_scheme` | Facebook SDK | Facebook URL Scheme，格式：`fb` + facebook_app_id |
| `google_app_id` | Google Services | Google 应用 ID（格式：`1:数字:android:哈希`） |
| `default_web_client_id` | Google Sign-In | OAuth 2.0 Web 客户端 ID（以 `.apps.googleusercontent.com` 结尾） |
| `firebase_database_url` | Firebase | Realtime Database URL |
| `gcm_defaultSenderId` | Firebase Cloud Messaging | FCM 发送者 ID |
| `project_id` | Firebase | Firebase 项目 ID |
| `google_api_key` | Google | Google API Key（以 `AIzaSy` 开头） |
| `google_crash_reporting_api_key` | Firebase Crashlytics | Crash Reporting API Key |
| `google_storage_bucket` | Firebase Storage | Cloud Storage Bucket（`项目ID.appspot.com`） |

> 注：Google/Firebase 相关的 key 通常由 `google-services.json` 的 Gradle 插件自动生成。仅在无法使用 Gradle 插件时才需通过 `stringResources` 手动注入。

### localizedStringResources

本地化 string 资源注入到 `launcher/src/main/res/values-<locale>/strings.xml`。Android 会根据设备语言自动选择对应的资源文件，找不到匹配语言时回退到 `res/values/strings.xml`。

大多数游戏只需要本地化 `app_name`，其余 SDK token/ID 在各语言下相同，放在 `stringResources` 中即可。

### Asset Pack deliveryType

| 值 | 说明 | 大小限制 |
|----|------|---------|
| `install-time` | 随应用安装时一起下载 | 单个 pack ≤ 150MB |
| `fast-follow` | 安装后自动在后台下载 | 单个 pack ≤ 150MB |
| `on-demand` | 应用按需请求下载 | 单个 pack ≤ 150MB |

### 常用 locale 编码

| locale | 语言 | locale | 语言 |
|--------|------|--------|------|
| `en` | English | `de` | Deutsch |
| `zh-rCN` | 简体中文 | `fr` | Français |
| `zh-rTW` | 繁體中文 | `es` | Español |
| `ja` | 日本語 | `pt` | Português |
| `ko` | 한국어 | `ru` | Русский |
| `th` | ไทย | `ar` | العربية |
| `vi` | Tiếng Việt | `tr` | Türkçe |
| `id` | Bahasa Indonesia | | |

## 依赖

- `com.gameframex.unity >= 1.1.1`

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)。

## 开源协议

本项目基于 [Apache 协议 2.0](https://www.apache.org/licenses/LICENSE-2.0) 开源。详见 [LICENSE.md](LICENSE.md)。

GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！
