# 1.0.0 (2026-05-27)


### Bug Fixes

* **editor:** 为每个构建步骤添加独立异常捕获 ([734afb0](https://github.com/gameframex/com.gameframex.unity.android/commit/734afb02bd3078c488478d6a9eef8aeb2c4039ab))
* **editor:** 修复 C# 9.0 target-typed new 语法为 C# 8.0 兼容写法 ([4f4162d](https://github.com/gameframex/com.gameframex.unity.android/commit/4f4162de780ca76c55b7f8b7f7702e6fd3c28c01))
* **editor:** 修正 assetPacks 为字符串数组格式 ([8b4199c](https://github.com/gameframex/com.gameframex.unity.android/commit/8b4199c07c4aafadbd0de3f0d9c3b22da28c8714))
* **editor:** 文件和目录复制源不存在时改用 Error 级别日志 ([525b8b7](https://github.com/gameframex/com.gameframex.unity.android/commit/525b8b7507da2e737ede7fa2755ff0921cdd0500))


### Features

* **editor:** 改用 MiniJSON 并添加 gradle.properties 支持 ([1967681](https://github.com/gameframex/com.gameframex.unity.android/commit/196768161271e6007d81e7f746d5da5730eb8ee0))
* **editor:** 添加 Android 构建配置编辑器 ([a8ad875](https://github.com/gameframex/com.gameframex.unity.android/commit/a8ad8751659fb03cf45d7494b2b7ccc1d0aa0719))
* **editor:** 添加 applicationId 和 versionName 配置 ([b4cf2e4](https://github.com/gameframex/com.gameframex.unity.android/commit/b4cf2e415373ac336bbf6500a8b127e6f176bc79))
* **editor:** 添加 Asset Pack 配置支持 ([42abe12](https://github.com/gameframex/com.gameframex.unity.android/commit/42abe1220d05dea63fbc505fbc96c68a05330d26))
* **editor:** 添加 assetPacksEnabled 开关 ([84ed571](https://github.com/gameframex/com.gameframex.unity.android/commit/84ed5711c8abcc8d787d64c40db2015cd86550a1))
* **editor:** 添加 localizedStringResources 本地化注入支持 ([e0842d6](https://github.com/gameframex/com.gameframex.unity.android/commit/e0842d6d86ec1b84d57daae956af0e2d3f18edb1))
* **editor:** 添加 SDK 版本配置支持 ([d6136e9](https://github.com/gameframex/com.gameframex.unity.android/commit/d6136e9957e29dbee1df0bec1f7b951bfc7b5614))
* **editor:** 添加 stringResources 注入支持 ([69213fe](https://github.com/gameframex/com.gameframex.unity.android/commit/69213fe0e7810be4a3924575d11b23741e12ff6d))
* **editor:** 添加文件和目录复制功能 ([f9ecb1f](https://github.com/gameframex/com.gameframex.unity.android/commit/f9ecb1fc75b755c633e1523563c7eb5cbf27b763))
* **editor:** 添加构建配置模板和示例文件 ([b7ac906](https://github.com/gameframex/com.gameframex.unity.android/commit/b7ac9060334938d92077d4235c79772496230085))
* **editor:** 添加签名配置支持 ([8bd24fd](https://github.com/gameframex/com.gameframex.unity.android/commit/8bd24fde72a934ef93dd0c33b09be03996e5fe7e))
* **editor:** 添加通用 resources 多类型资源注入支持 ([00d0187](https://github.com/gameframex/com.gameframex.unity.android/commit/00d01875293320674ed866c65c948a1facc73268))
* **manifest:** 支持 application 标签属性注入 ([1825920](https://github.com/gameframex/com.gameframex.unity.android/commit/18259209e3f0782a9dcc8f669d00e4d9de1c7faa))

# Changelog

## 1.0.0 (2025-05-23)

### Added

- Initial release of Android Build Config package.
- Auto-discovery of `AndroidBuildConfig.json` files across the project.
- Merging and deduplication of multiple config files.
- Automatic injection of Maven repositories into `settings.gradle`.
- Automatic injection of Gradle dependencies into `build.gradle`.
- Automatic injection of permissions and meta-data into `AndroidManifest.xml`.
- `IPostGenerateGradleAndroidProject` integration for Unity standard builds.
- `IBuilderPreHookHandler` integration for GameFrameX builds.
