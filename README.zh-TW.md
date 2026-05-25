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

通用的 Android 構建自動設定系統。透過 JSON 資料檔案驅動，自動發現專案中所有 `AndroidBuildConfig.json` 並在構建時注入 Maven 倉庫、Gradle 依賴、AndroidManifest 設定、string 資源和本地化配置。

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

所有欄位均為可選。專案中可放置多個設定檔，內容自動合併去重。

### 欄位說明

#### 全域欄位

| 欄位 | 類型 | 合併策略 | 說明 |
|------|------|---------|------|
| `providerName` | string | 後寫覆蓋 | 配置提供者名稱，僅用於構建日誌標識 |
| `mavenRepositories` | `[{name, url}]` | 按 URL 去重 | Maven 倉庫列表，注入到根 `settings.gradle` 的 `dependencyResolutionManagement.repositories` |
| `gradleWrapper` | `{key: value}` | 按 key 覆蓋 | 鍵值對注入到 `gradle-wrapper.properties` |
| `gradleProperties` | `{key: value}` | 按 key 覆蓋 | 鍵值對注入到 `gradle.properties` |
| `fileCopies` | `{源路徑: 目標路徑}` | 按 source 覆蓋 | 檔案複製映射。相對源路徑基於配置檔案目錄解析，相對目標路徑基於 Gradle 根目錄解析 |
| `directoryCopies` | `{源路徑: 目標路徑}` | 按 source 覆蓋 | 目錄複製映射，路徑解析同 `fileCopies` |
| `assetPacksEnabled` | bool | 後寫覆蓋 | 是否啟用 Google Play Asset Delivery 功能，預設 `false` |
| `assetPacks` | `[{name, deliveryType, streamingAssetsPath}]` | 按 name 去重 | Asset Pack 配置列表，每個 pack 對應一個獨立的 Gradle 子專案 |
| `localizedStringResources` | `{locale: {key: value}}` | 按 locale 分組，每組內按 key 後寫覆蓋 | 本地化 string 資源，注入到 launcher 的 `res/values-<locale>/strings.xml` |

#### 模組欄位（launcher / unityLibrary）

| 欄位 | 類型 | 合併策略 | 說明 |
|------|------|---------|------|
| `compileSdkVersion` | string | 取最大值 | 編譯 SDK 版本，注入到 `android {}` 區塊 |
| `buildToolsVersion` | string | 後寫覆蓋 | 建構工具版本 |
| `minSdkVersion` | string | 取最大值 | 最低 SDK 版本 |
| `targetSdkVersion` | string | 取最大值 | 目標 SDK 版本 |
| `applicationId` | string | 後寫覆蓋 | 應用程式包名（僅 launcher 有效） |
| `versionName` | string | 後寫覆蓋 | 版本名稱 |
| `dependencies` | `[{configuration, notation}]` | 按 `configuration:notation` 去重 | Gradle 依賴列表。`configuration` 可為 `implementation`、`api` 等 |
| `permissions` | `[string]` | 按名稱去重 | AndroidManifest `<uses-permission>` 權限列表 |
| `metaData` | `[{name, value}]` | 按 name 去重 | AndroidManifest `<application>` 下的 `<meta-data>` 條目 |
| `applicationAttributes` | `[{name, value}]` | 按名稱去重 | AndroidManifest `<application>` 標籤上的屬性（不含 `android:` 前綴） |
| `signingConfig` | `{storeFile, storePassword, keyAlias, keyPassword}` | 後寫覆蓋 | 簽名配置，注入到 `signingConfigs.release` 和 `buildTypes.release` |
| `stringResources` | `{key: value}` | 按 key 後寫覆蓋 | （舊版）string 資源，注入到 `res/values/strings.xml`。自動合併到 `resources.string` |
| `resources` | `{type: {key: value/object}}` | 按 type 分組，每組內按 key 後寫覆蓋 | 多型別 values 資源，注入到 `res/values/strings.xml`。參見下方「資源值格式」 |

## 配置參考

### 資源值格式

`resources` 欄位使用嵌套字典：外層 key = XML 元素型別，內層 key = 資源名，內層 value = 文字或物件。

**純字串值**（無額外 XML 屬性）：
```json
"resources": {
  "string": { "app_name": "My App" },
  "integer": { "max_retry_count": "3" }
}
```
產出：`<string name="app_name">My App</string>` 和 `<integer name="max_retry_count">3</integer>`

**物件值**（帶額外 XML 屬性）：
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
產出：`<string name="facebook_app_id" translatable="false">724358863922328</string>`

**支援的資源型別**：`string`、`integer`、`bool`、`color`、`dimen`（以及其他 XML 元素型別 — 標籤名直接使用）。

**向後相容**：`stringResources` 會自動合併到 `resources.string`。如果兩者宣告了相同的 key，`resources` 優先。

### stringResources 常用 Key

| Key | 所屬 SDK / 服務 | 說明 |
|-----|----------------|------|
| `app_name` | Android | 應用顯示名稱（桌面圖示名稱） |
| `facebook_app_id` | Facebook SDK | Facebook 應用 ID |
| `facebook_client_token` | Facebook SDK | Facebook 客戶端令牌 |
| `facebook_app_scheme` | Facebook SDK | Facebook URL Scheme，格式：`fb` + facebook_app_id |
| `google_app_id` | Google Services | Google 應用 ID（格式：`1:數字:android:哈希`） |
| `default_web_client_id` | Google Sign-In | OAuth 2.0 Web 客戶端 ID（以 `.apps.googleusercontent.com` 結尾） |
| `firebase_database_url` | Firebase | Realtime Database URL |
| `gcm_defaultSenderId` | Firebase Cloud Messaging | FCM 發送者 ID |
| `project_id` | Firebase | Firebase 專案 ID |
| `google_api_key` | Google | Google API Key（以 `AIzaSy` 開頭） |
| `google_crash_reporting_api_key` | Firebase Crashlytics | Crash Reporting API Key |
| `google_storage_bucket` | Firebase Storage | Cloud Storage Bucket（`專案ID.appspot.com`） |

> 註：Google/Firebase 相關的 key 通常由 `google-services.json` 的 Gradle 外掛自動產生。僅在無法使用 Gradle 外掛時才需透過 `stringResources` 手動注入。

### localizedStringResources

本地化 string 資源注入到 `launcher/src/main/res/values-<locale>/strings.xml`。Android 會根據裝置語言自動選擇對應的資源檔案，找不到匹配語言時回退到 `res/values/strings.xml`。

大多數遊戲只需要本地化 `app_name`，其餘 SDK token/ID 在各語言下相同，放在 `stringResources` 中即可。

### Asset Pack deliveryType

| 值 | 說明 | 大小限制 |
|----|------|---------|
| `install-time` | 隨應用安裝時一起下載 | 單個 pack ≤ 150MB |
| `fast-follow` | 安裝後自動在背景下載 | 單個 pack ≤ 150MB |
| `on-demand` | 應用按需請求下載 | 單個 pack ≤ 150MB |

### 常用 locale 編碼

| locale | 語言 | locale | 語言 |
|--------|------|--------|------|
| `en` | English | `de` | Deutsch |
| `zh-rCN` | 简体中文 | `fr` | Français |
| `zh-rTW` | 繁體中文 | `es` | Español |
| `ja` | 日本語 | `pt` | Português |
| `ko` | 한국어 | `ru` | Русский |
| `th` | ไทย | `ar` | العربية |
| `vi` | Tiếng Việt | `tr` | Türkçe |
| `id` | Bahasa Indonesia | | |

## 依賴

- `com.gameframex.unity >= 1.1.1`

## 更新日誌

詳見 [CHANGELOG.md](CHANGELOG.md)。

## 開源協議

本專案基於 [Apache 協議 2.0](https://www.apache.org/licenses/LICENSE-2.0) 開源。詳見 [LICENSE.md](LICENSE.md)。

GameFrameX 組織下的以及組織衍生的專案的版權、商標、專利和其他相關權利均受相應法律法規的保護。不得利用本專案從事危害國家安全、擾亂社會秩序、侵犯他人合法權益等法律法規禁止的活動！
