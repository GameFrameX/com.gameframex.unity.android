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

所有欄位均為可選。專案中可放置多個設定檔，內容自動合併去重。

## 依賴

- `com.gameframex.unity >= 1.1.1`

## 更新日誌

詳見 [CHANGELOG.md](CHANGELOG.md)。

## 開源協議

本專案基於 MIT 協議開源，詳見 [LICENSE.md](LICENSE.md)。
