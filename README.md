<div align="center">

  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />

  # Game Frame X Android

  [![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.android?label=version)](https://github.com/GameFrameX/com.gameframex.unity.android/releases)
  [![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.android?label=license)](https://github.com/GameFrameX/com.gameframex.unity.android/blob/main/LICENSE.md)
  [![Documentation](https://img.shields.io/badge/documentation-docs-blue)](https://gameframex.doc.alianblank.com)

  All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

  [Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start)

  **English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## Project Overview

Generic Android build configuration system. Auto-discovers `AndroidBuildConfig.json` files across the project and injects Maven repositories, Gradle dependencies, AndroidManifest entries, string resources, and localization during build.

Key features:

- **Data-driven**: SDK packages only need a JSON file — no C# code required
- **Multi-file support**: Any number of config files across the project
- **Any location**: Place config files anywhere Unity can recognize
- **Auto-merge & dedup**: Multiple configs are merged with key-based deduplication
- **Idempotent**: Safe to build multiple times — existing entries are skipped

## Quick Start

### Installation

Add the package to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

### Usage Examples

Place an `AndroidBuildConfig.json` file anywhere in your project (e.g., under `Assets/` or `Packages/`):

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

All fields are optional. You can place multiple config files — contents are automatically merged and deduplicated.

### Field Descriptions

#### Global Fields

| Field | Type | Merge Strategy | Description |
|-------|------|----------------|-------------|
| `providerName` | string | Last-writer-wins | Config provider name, used for logging only |
| `mavenRepositories` | `[{name, url}]` | Deduplicate by URL | Maven repositories, injected into root `settings.gradle` |
| `gradleWrapper` | `{key: value}` | Key override | Key-value pairs injected into `gradle-wrapper.properties` |
| `gradleProperties` | `{key: value}` | Key override | Key-value pairs injected into `gradle.properties` |
| `fileCopies` | `{source: dest}` | Source override | File copy map. Relative source resolved from config file directory, relative dest from Gradle root |
| `directoryCopies` | `{source: dest}` | Source override | Directory copy map. Same path resolution as `fileCopies` |
| `assetPacksEnabled` | bool | Last-writer-wins | Enable Google Play Asset Delivery feature. Default `false` |
| `assetPacks` | `[{name, deliveryType, streamingAssetsPath}]` | Deduplicate by name | Asset pack configurations. Each pack becomes an independent Gradle sub-project |
| `localizedStringResources` | `{locale: {key: value}}` | Per-locale, per-key last-writer-wins | Localized string resources, injected into launcher's `res/values-<locale>/strings.xml` |

#### Module Fields (launcher / unityLibrary)

| Field | Type | Merge Strategy | Description |
|-------|------|----------------|-------------|
| `compileSdkVersion` | string | Max value | Compile SDK version, injected into `android {}` block |
| `buildToolsVersion` | string | Last-writer-wins | Build tools version |
| `minSdkVersion` | string | Max value | Minimum SDK version |
| `targetSdkVersion` | string | Max value | Target SDK version |
| `applicationId` | string | Last-writer-wins | Application package name (launcher only) |
| `versionName` | string | Last-writer-wins | Version name string |
| `dependencies` | `[{configuration, notation}]` | Deduplicate by `configuration:notation` | Gradle dependencies. `configuration` can be `implementation`, `api`, etc. |
| `permissions` | `[string]` | Deduplicate by name | AndroidManifest `<uses-permission>` entries |
| `metaData` | `[{name, value}]` | Deduplicate by name | AndroidManifest `<meta-data>` entries under `<application>` |
| `applicationAttributes` | `[{name, value}]` | Deduplicate by name | Attributes on the `<application>` tag (without `android:` prefix) |
| `signingConfig` | `{storeFile, storePassword, keyAlias, keyPassword}` | Last-writer-wins | Signing config for `signingConfigs.release` and `buildTypes.release` |
| `stringResources` | `{key: value}` | Per-key last-writer-wins | (Legacy) String resources injected into `res/values/strings.xml`. Automatically merged into `resources.string` |
| `resources` | `{type: {key: value/object}}` | Per-type, per-key last-writer-wins | Multi-type values resources injected into `res/values/strings.xml`. See [Resource Value Format](#resource-value-format) |

## Configuration Reference

### Resource Value Format

The `resources` field uses a nested dictionary: outer key = XML element type, inner key = resource name, inner value = text or object.

**Simple string value** (no extra XML attributes):
```json
"resources": {
  "string": { "app_name": "My App" },
  "integer": { "max_retry_count": "3" }
}
```
Produces: `<string name="app_name">My App</string>` and `<integer name="max_retry_count">3</integer>`

**Object value** (with extra XML attributes):
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
Produces: `<string name="facebook_app_id" translatable="false">724358863922328</string>`

**Supported resource types**: `string`, `integer`, `bool`, `color`, `dimen` (and any other XML element type — the tag name is used directly).

**Backward compatibility**: `stringResources` is automatically merged into `resources.string`. If both declare the same key, `resources` takes precedence.

### stringResources Common Keys

| Key | SDK / Service | Description |
|-----|---------------|-------------|
| `app_name` | Android | Application display name (home screen icon label) |
| `facebook_app_id` | Facebook SDK | Facebook Application ID |
| `facebook_client_token` | Facebook SDK | Facebook Client Token |
| `facebook_app_scheme` | Facebook SDK | Facebook URL Scheme, format: `fb` + facebook_app_id |
| `google_app_id` | Google Services | Google Application ID (format: `1:number:android:hash`) |
| `default_web_client_id` | Google Sign-In | OAuth 2.0 Web Client ID (ends with `.apps.googleusercontent.com`) |
| `firebase_database_url` | Firebase | Realtime Database URL |
| `gcm_defaultSenderId` | Firebase Cloud Messaging | FCM Sender ID |
| `project_id` | Firebase | Firebase Project ID |
| `google_api_key` | Google | Google API Key (starts with `AIzaSy`) |
| `google_crash_reporting_api_key` | Firebase Crashlytics | Crash Reporting API Key |
| `google_storage_bucket` | Firebase Storage | Cloud Storage Bucket (`projectId.appspot.com`) |

> Note: Google/Firebase keys are typically auto-generated by the `google-services.json` Gradle plugin. Only use `stringResources` for manual injection when the plugin cannot be used.

### localizedStringResources

Localized string resources are injected into `launcher/src/main/res/values-<locale>/strings.xml`. Android automatically selects the appropriate resource file based on device language, falling back to `res/values/strings.xml` when no match is found.

Most games only need to localize `app_name`. Other SDK tokens/IDs are language-independent and should be placed in `stringResources`.

### Asset Pack deliveryType

| Value | Description | Size Limit |
|-------|-------------|------------|
| `install-time` | Delivered with the app installation | ≤ 150MB per pack |
| `fast-follow` | Downloaded automatically shortly after install | ≤ 150MB per pack |
| `on-demand` | Downloaded only when requested by the app | ≤ 150MB per pack |

### Common Locale Codes

| Locale | Language | Locale | Language |
|--------|----------|--------|----------|
| `en` | English | `de` | Deutsch |
| `zh-rCN` | 简体中文 (Simplified Chinese) | `fr` | Français |
| `zh-rTW` | 繁體中文 (Traditional Chinese) | `es` | Español |
| `ja` | 日本語 (Japanese) | `pt` | Português |
| `ko` | 한국어 (Korean) | `ru` | Русский |
| `th` | ไทย (Thai) | `ar` | العربية |
| `vi` | Tiếng Việt (Vietnamese) | `tr` | Türkçe |
| `id` | Bahasa Indonesia | | |

## Dependencies

- `com.gameframex.unity >= 1.1.1`

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for details.

## License

Licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0). See [LICENSE.md](LICENSE.md) for details.
