<div align="center">

  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />

  # Game Frame X Android

  [![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.android?label=version)](https://github.com/GameFrameX/com.gameframex.unity.android/releases)
  [![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.android?label=license)](https://github.com/GameFrameX/com.gameframex.unity.android/blob/main/LICENSE.md)
  [![Documentation](https://img.shields.io/badge/documentation-docs-blue)](https://gameframex.doc.alianblank.com)

  獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

  [文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始)

  [English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## 項目簡介

通用的 Android 構建自動設定系統。透過 JSON 資料檔案驅動，自動發現專案中所有 `AndroidBuildConfig.json` 並在構建時注入 Maven 倉庫、Gradle 依賴、AndroidManifest 設定。

核心特性：

- **純資料驅動**：SDK 套件只需 JSON 檔案，無需 C# 程式碼
- **多檔案支援**：專案中可存在任意數量的設定檔
- **任意位置**：設定檔可放在 Unity 能識別的任何位置
- **自動合併去重**：多個設定檔按 key 自動合併去重
- **冪等操作**：多次建構安全，已有條目自動跳過

## 快速開始

### 安裝

在專案的 `Packages/manifest.json` 中新增依賴：

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

### 使用範例

在專案的任意位置放置 `AndroidBuildConfig.json` 檔案（如 `Assets/`、`Packages/` 下的任何目錄）：

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
  "fileCopies": {
    "libs/your-sdk.aar": "launcher/libs/your-sdk.aar"
  },
  "directoryCopies": {
    "jniLibs": "launcher/jniLibs"
  }
}
```

所有欄位均為可選。專案中可放置多個設定檔，內容自動合併去重。

- **`launcher`**：作用於 Android 應用殼（`launcher/build.gradle`、`launcher/AndroidManifest.xml`）
- **`unityLibrary`**：作用於 Unity 引擎庫（`unityLibrary/build.gradle`、`unityLibrary/AndroidManifest.xml`）
- **`mavenRepositories`**：注入到根 `settings.gradle`
- **`gradleWrapper`**：鍵值對注入到 `gradle-wrapper.properties`
- **SDK 版本**（`compileSdkVersion`/`buildToolsVersion`/`minSdkVersion`/`targetSdkVersion`）：合併時取所有設定中的最大數值（`buildToolsVersion` 為後寫覆蓋）
- **`applicationId`**：應用標識符，注入到 `defaultConfig {}`，合併策略為後寫覆蓋
- **`versionName`**：版本名稱，注入到 `defaultConfig {}`，合併策略為後寫覆蓋
- **`signingConfig`**：簽名配置（storeFile/storePassword/keyAlias/keyPassword），注入到 `signingConfigs {}` 和 `buildTypes.release {}`。storeFile 支援相對路徑和絕對路徑
- **`fileCopies`**：鍵值對映射（源路徑 → 目標路徑），用於複製檔案到 Gradle 專案。支援相對路徑和絕對路徑
- **`directoryCopies`**：鍵值對映射（源路徑 → 目標路徑），用於複製目錄到 Gradle 專案。支援相對路徑和絕對路徑

## 依賴

- `com.gameframex.unity >= 1.1.1`

## 更新日誌

詳見 [CHANGELOG.md](CHANGELOG.md)。

## 開源協議

本專案基於 [Apache 協議 2.0](https://www.apache.org/licenses/LICENSE-2.0) 開源。詳見 [LICENSE.md](LICENSE.md)。

GameFrameX 組織下的以及組織衍生的專案的版權、商標、專利和其他相關權利均受相應法律法規的保護。不得利用本專案從事危害國家安全、擾亂社會秩序、侵犯他人合法權益等法律法規禁止的活動！
