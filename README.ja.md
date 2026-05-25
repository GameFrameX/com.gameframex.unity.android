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

汎用 Android ビルド自動設定システム。JSON データファイルで駆動し、プロジェクト内のすべての `AndroidBuildConfig.json` を自動検出して、ビルド時に Maven リポジトリ、Gradle 依存関係、AndroidManifest エントリを注入します。

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

すべてのフィールドは省略可能です。複数の設定ファイルを配置でき、内容は自動的にマージ・重複排除されます。

- **`launcher`**: Android アプリシェルに適用（`launcher/build.gradle`、`launcher/AndroidManifest.xml`）
- **`unityLibrary`**: Unity エンジンライブラリに適用（`unityLibrary/build.gradle`、`unityLibrary/AndroidManifest.xml`）
- **`mavenRepositories`**: ルートの `settings.gradle` に注入
- **`gradleWrapper`**: キーと値のペアを `gradle-wrapper.properties` に注入
- **SDK バージョン**（`compileSdkVersion`/`buildToolsVersion`/`minSdkVersion`/`targetSdkVersion`）: マージ時に全設定中の最大数値を採用（`buildToolsVersion` は後勝ち）
- **`applicationId`**: アプリケーション識別子、`defaultConfig {}` に注入、マージは後勝ち
- **`versionName`**: バージョン名、`defaultConfig {}` に注入、マージは後勝ち
- **`signingConfig`**: 署名設定（storeFile/storePassword/keyAlias/keyPassword）、`signingConfigs {}` と `buildTypes.release {}` に注入。storeFile は相対パスと絶対パスに対応
- **`assetPacks`**: Google Play Asset Delivery 設定。StreamingAssets のサブディレクトリを独立した Gradle asset pack モジュールにマッピングします。各エントリには以下のフィールドが含まれます：
  - `name`: asset pack モジュール名（Gradle サブプロジェクト名として使用）
  - `deliveryType`: 配信モード — `install-time`（アプリと一緒に配信）、`fast-follow`（インストール後に自動ダウンロード）、または `on-demand`（リクエスト時にダウンロード）
  - `streamingAssetsPath`: Unity の StreamingAssets フォルダ以下のサブディレクトリ名。この asset pack にマッピングされます
  - `assetPacks` にリストされていない StreamingAssets サブディレクトリは元の位置に残ります
- **`fileCopies`**: キーと値のマッピング（ソースパス → コピー先パス）、Gradle プロジェクトにファイルをコピー。相対パスと絶対パスに対応
- **`directoryCopies`**: キーと値のマッピング（ソースパス → コピー先パス）、Gradle プロジェクトにディレクトリをコピー。相対パスと絶対パスに対応

## 依存関係

- `com.gameframex.unity >= 1.1.1`

## 変更履歴

詳細は [CHANGELOG.md](CHANGELOG.md) を参照してください。

## ライセンス

[Apache ライセンス 2.0](https://www.apache.org/licenses/LICENSE-2.0) の下で公開されています。詳細は [LICENSE.md](LICENSE.md) を参照してください。

GameFrameX 組織およびその派生プロジェクトの著作権、商標、特許、その他の関連権利は、適用される法律および規則により保護されています。国家安全を脅かす、社会秩序を乱す、他人の合法的な権利を侵害するなど、法律で禁止されている活動に本プロジェクトを使用することは禁じられています。
