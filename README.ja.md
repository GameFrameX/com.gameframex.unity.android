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
  "dependencies": [
    { "configuration": "implementation", "notation": "com.example:sdk:1.0.0" }
  ],
  "permissions": [
    "android.permission.INTERNET"
  ],
  "metaData": [
    { "name": "com.example.key", "value": "value" }
  ]
}
```

すべてのフィールドは省略可能です。複数の設定ファイルを配置でき、内容は自動的にマージ・重複排除されます。

## 依存関係

- `com.gameframex.unity >= 1.1.1`

## 変更履歴

詳細は [CHANGELOG.md](CHANGELOG.md) を参照してください。

## ライセンス

本プロジェクトは MIT ライセンスの下で公開されています。詳細は [LICENSE.md](LICENSE.md) を参照してください。
