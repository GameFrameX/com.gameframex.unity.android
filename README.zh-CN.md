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

通用的 Android 构建自动配置系统。通过 JSON 数据文件驱动，自动发现项目中所有 `AndroidBuildConfig.json` 并在构建时注入 Maven 仓库、Gradle 依赖、AndroidManifest 配置。

核心特性：

- **纯数据驱动**：SDK 包只需 JSON 文件，无需 C# 代码
- **多文件支持**：项目中可存在任意数量的配置文件
- **任意位置**：配置文件可放在 Unity 能识别的任何位置
- **自动合并去重**：多个配置文件按 key 自动合并去重
- **幂等操作**：多次构建安全，已有条目自动跳过

## 快速开始

### 安装

在项目的 `Packages/manifest.json` 中添加依赖：

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

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
    "signingConfig": {
      "storeFile": "keystore/release.jks",
      "storePassword": "your_store_password",
      "keyAlias": "release",
      "keyPassword": "your_key_password"
    }
  },
  "unityLibrary": {
    "compileSdkVersion": "34",
    "buildToolsVersion": "34.0.0",
    "minSdkVersion": "24",
    "targetSdkVersion": "34",
    "dependencies": [
      { "configuration": "implementation", "notation": "com.example:sdk-core:2.0.0" }
    ],
    "permissions": [ "android.permission.INTERNET" ],
    "metaData": []
  },
  "assetPacks": [
    {
      "name": "additional_assets",
      "deliveryType": "install-time",
      "streamingAssetsPath": "AdditionalAssets"
    },
    {
      "name": "extra_assets",
      "deliveryType": "fast-follow",
      "streamingAssetsPath": "ExtraAssets"
    }
  ],
  "fileCopies": {
    "libs/your-sdk.aar": "launcher/libs/your-sdk.aar"
  },
  "directoryCopies": {
    "jniLibs": "launcher/jniLibs"
  }
}
```

所有字段均为可选。项目中可放置多个配置文件，内容自动合并去重。

- **`launcher`**：作用于 Android 应用壳（`launcher/build.gradle`、`launcher/AndroidManifest.xml`）
- **`unityLibrary`**：作用于 Unity 引擎库（`unityLibrary/build.gradle`、`unityLibrary/AndroidManifest.xml`）
- **`mavenRepositories`**：注入到根 `settings.gradle`
- **`gradleWrapper`**：键值对注入到 `gradle-wrapper.properties`
- **SDK 版本**（`compileSdkVersion`/`buildToolsVersion`/`minSdkVersion`/`targetSdkVersion`）：合并时取所有配置中的最大数值（`buildToolsVersion` 为后写覆盖）
- **`applicationId`**：应用标识符，注入到 `defaultConfig {}`，合并策略为后写覆盖
- **`versionName`**：版本名称，注入到 `defaultConfig {}`，合并策略为后写覆盖
- **`signingConfig`**：签名配置（storeFile/storePassword/keyAlias/keyPassword），注入到 `signingConfigs {}` 和 `buildTypes.release {}`。storeFile 支持相对路径和绝对路径
- **`assetPacks`**：Google Play Asset Delivery 配置。将 StreamingAssets 子目录映射为独立的 Gradle asset pack 模块。每个条目包含：
  - `name`：asset pack 模块名称（作为 Gradle 子项目名称）
  - `deliveryType`：分发模式 — `install-time`（随应用一起分发）、`fast-follow`（安装后自动下载）或 `on-demand`（按需下载）
  - `streamingAssetsPath`：Unity StreamingAssets 文件夹下的子目录名称，映射到该 asset pack 中
  - 未在 `assetPacks` 中列出的 StreamingAssets 子目录将保留在原始位置
- **`fileCopies`**：键值对映射（源路径 → 目标路径），用于复制文件到 Gradle 项目。支持相对路径和绝对路径
- **`directoryCopies`**：键值对映射（源路径 → 目标路径），用于复制目录到 Gradle 项目。支持相对路径和绝对路径

## 依赖

- `com.gameframex.unity >= 1.1.1`

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)。

## 开源协议

本项目基于 [Apache 协议 2.0](https://www.apache.org/licenses/LICENSE-2.0) 开源。详见 [LICENSE.md](LICENSE.md)。

GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！
