<div align="center">

  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />

  # Game Frame X Android

  [![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.android?label=version)](https://github.com/GameFrameX/com.gameframex.unity.android/releases)
  [![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.android?label=license)](https://github.com/GameFrameX/com.gameframex.unity.android/blob/main/LICENSE.md)
  [![Documentation](https://img.shields.io/badge/documentation-docs-blue)](https://gameframex.doc.alianblank.com)

  インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

  [ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート)

  [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

---

## プロジェクト概要

汎用 Android ビルド自動設定システム。JSON データファイルで駆動し、プロジェクト内のすべての `AndroidBuildConfig.json` を自動検出して、ビルド時に Maven リポジトリ、Gradle 依存関係、AndroidManifest エントリ、string リソース、ローカライゼーションを注入します。

主な機能：

- **データ駆動**: SDK パッケージは JSON ファイルのみで C# コード不要
- **マルチファイル対応**: プロジェクト内に任意の数の設定ファイルを配置可能
- **任意の場所**: Unity が認識できる任意の場所に設定ファイルを配置
- **自動マージと重複排除**: 複数の設定をキーベースで自動マージ・重複排除
- **冪等性**: 複数回ビルドしても安全 — 既存のエントリはスキップ

## クイックスタート

### インストール

プロジェクトの `Packages/manifest.json` に追加：

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

### 使用例

プロジェクト内の任意の場所に `AndroidBuildConfig.json` ファイルを配置（例：`Assets/` や `Packages/` 下の任意のディレクトリ）：

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

すべてのフィールドは省略可能です。複数の設定ファイルを配置でき、内容は自動的にマージ・重複排除されます。

### フィールド説明

#### グローバルフィールド

| フィールド | 型 | マージ戦略 | 説明 |
|-----------|-----|-----------|------|
| `providerName` | string | 後勝ち | 設定プロバイダ名、ビルドログでの識別にのみ使用 |
| `mavenRepositories` | `[{name, url}]` | URL で重複排除 | Maven リポジトリリスト、ルート `settings.gradle` の `dependencyResolutionManagement.repositories` に注入 |
| `gradleWrapper` | `{key: value}` | キーで上書き | `gradle-wrapper.properties` に注入するキーと値のペア |
| `gradleProperties` | `{key: value}` | キーで上書き | `gradle.properties` に注入するキーと値のペア |
| `fileCopies` | `{ソース: コピー先}` | ソースで上書き | ファイルコピーマップ。相対ソースは設定ファイルディレクトリから、相対コピー先は Gradle ルートから解決 |
| `directoryCopies` | `{ソース: コピー先}` | ソースで上書き | ディレクトリコピーマップ。パス解決は `fileCopies` と同じ |
| `assetPacksEnabled` | bool | 後勝ち | Google Play Asset Delivery 機能の有効化。デフォルト `false` |
| `assetPacks` | `[{name, deliveryType, streamingAssetsPath}]` | name で重複排除 | Asset Pack 設定リスト。各パックは独立した Gradle サブプロジェクトになります |
| `localizedStringResources` | `{locale: {key: value}}` | locale ごとにグループ化、各グループ内でキーごとに後勝ち | ローカライズされた string リソース、launcher の `res/values-<locale>/strings.xml` に注入 |

#### モジュールフィールド（launcher / unityLibrary）

| フィールド | 型 | マージ戦略 | 説明 |
|-----------|-----|-----------|------|
| `compileSdkVersion` | string | 最大値 | コンパイル SDK バージョン、`android {}` ブロックに注入 |
| `buildToolsVersion` | string | 後勝ち | ビルドツールバージョン |
| `minSdkVersion` | string | 最大値 | 最小 SDK バージョン |
| `targetSdkVersion` | string | 最大値 | ターゲット SDK バージョン |
| `applicationId` | string | 後勝ち | アプリケーションパッケージ名（launcher のみ有効） |
| `versionName` | string | 後勝ち | バージョン名 |
| `dependencies` | `[{configuration, notation}]` | `configuration:notation` で重複排除 | Gradle 依存関係リスト。`configuration` は `implementation`、`api` など |
| `permissions` | `[string]` | 名前で重複排除 | AndroidManifest `<uses-permission>` エントリ |
| `metaData` | `[{name, value}]` | name で重複排除 | AndroidManifest `<application>` 下の `<meta-data>` エントリ |
| `applicationAttributes` | `[{name, value}]` | 名前で重複排除 | AndroidManifest `<application>` タグの属性（`android:` プレフィックスなし） |
| `signingConfig` | `{storeFile, storePassword, keyAlias, keyPassword}` | 後勝ち | 署名設定、`signingConfigs.release` と `buildTypes.release` に注入 |
| `stringResources` | `{key: value}` | キーごとに後勝ち | （旧版）string リソース、`res/values/strings.xml` に注入。自動的に `resources.string` にマージ |
| `resources` | `{type: {key: value/object}}` | type ごとにグループ化、各グループ内でキーごとに後勝ち | マルチタイプ values リソース、`res/values/strings.xml` に注入。下記「リソース値フォーマット」を参照 |

## 設定リファレンス

### リソース値フォーマット

`resources` フィールドはネストされた辞書を使用します：外側のキー = XML 要素タイプ、内側のキー = リソース名、内側の値 = テキストまたはオブジェクト。

**単純な文字列値**（追加の XML 属性なし）：
```json
"resources": {
  "string": { "app_name": "My App" },
  "integer": { "max_retry_count": "3" }
}
```
出力：`<string name="app_name">My App</string>` および `<integer name="max_retry_count">3</integer>`

**オブジェクト値**（追加の XML 属性あり）：
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
出力：`<string name="facebook_app_id" translatable="false">724358863922328</string>`

**対応リソースタイプ**：`string`、`integer`、`bool`、`color`、`dimen`（その他の XML 要素タイプも直接使用可能）。

**後方互換性**：`stringResources` は自動的に `resources.string` にマージされます。同じキーが両方に宣言されている場合、`resources` が優先されます。

### stringResources よく使う Key

| Key | SDK / サービス | 説明 |
|-----|---------------|------|
| `app_name` | Android | アプリケーション表示名（ホーム画面のアイコン名） |
| `facebook_app_id` | Facebook SDK | Facebook アプリケーション ID |
| `facebook_client_token` | Facebook SDK | Facebook クライアントトークン |
| `facebook_app_scheme` | Facebook SDK | Facebook URL Scheme、形式：`fb` + facebook_app_id |
| `google_app_id` | Google Services | Google アプリケーション ID（形式：`1:数値:android:ハッシュ`） |
| `default_web_client_id` | Google Sign-In | OAuth 2.0 Web クライアント ID（`.apps.googleusercontent.com` で終わる） |
| `firebase_database_url` | Firebase | Realtime Database URL |
| `gcm_defaultSenderId` | Firebase Cloud Messaging | FCM 送信者 ID |
| `project_id` | Firebase | Firebase プロジェクト ID |
| `google_api_key` | Google | Google API Key（`AIzaSy` で始まる） |
| `google_crash_reporting_api_key` | Firebase Crashlytics | Crash Reporting API Key |
| `google_storage_bucket` | Firebase Storage | Cloud Storage Bucket（`プロジェクトID.appspot.com`） |

> 注：Google/Firebase 関連の key は通常 `google-services.json` の Gradle プラグインによって自動生成されます。Gradle プラグインが使用できない場合のみ `stringResources` で手動注入してください。

### localizedStringResources

ローカライズされた string リソースは `launcher/src/main/res/values-<locale>/strings.xml` に注入されます。Android はデバイスの言語設定に基づいて適切なリソースファイルを自動的に選択し、一致する言語が見つからない場合は `res/values/strings.xml` にフォールバックします。

ほとんどのゲームでは `app_name` のみをローカライズすれば十分です。その他の SDK トークンや ID は言語に依存しないため、`stringResources` に配置してください。

### Asset Pack deliveryType

| 値 | 説明 | サイズ制限 |
|----|------|-----------|
| `install-time` | アプリのインストール時に一緒に配信 | パックあたり ≤ 150MB |
| `fast-follow` | インストール後に自動的にバックグラウンドでダウンロード | パックあたり ≤ 150MB |
| `on-demand` | アプリからのリクエスト時にダウンロード | パックあたり ≤ 150MB |

### よく使う locale コード

| locale | 言語 | locale | 言語 |
|--------|------|--------|------|
| `en` | English | `de` | Deutsch |
| `zh-rCN` | 简体中文 | `fr` | Français |
| `zh-rTW` | 繁體中文 | `es` | Español |
| `ja` | 日本語 | `pt` | Português |
| `ko` | 한국어 | `ru` | Русский |
| `th` | ไทย | `ar` | العربية |
| `vi` | Tiếng Việt | `tr` | Türkçe |
| `id` | Bahasa Indonesia | | |

## 依存関係

- `com.gameframex.unity >= 1.1.1`

## 変更履歴

詳細は [CHANGELOG.md](CHANGELOG.md) を参照してください。

## ライセンス

[Apache ライセンス 2.0](https://www.apache.org/licenses/LICENSE-2.0) の下で公開されています。詳細は [LICENSE.md](LICENSE.md) を参照してください。

GameFrameX 組織およびその派生プロジェクトの著作権、商標、特許、その他の関連権利は、適用される法律および規則により保護されています。国家安全を脅かす、社会秩序を乱す、他人の合法的な権利を侵害するなど、法律で禁止されている活動に本プロジェクトを使用することは禁じられています。
