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

Generic Android build configuration system. Auto-discovers `AndroidBuildConfig.json` files across the project and injects Maven repositories, Gradle dependencies, and AndroidManifest entries during build.

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

All fields are optional. You can place multiple config files — contents are automatically merged and deduplicated.

## Dependencies

- `com.gameframex.unity >= 1.1.1`

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for details.

## License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md) for details.
