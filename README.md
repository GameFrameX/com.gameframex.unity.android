# Game Frame X Android Build Config

通用的 Android 构建自动配置系统。通过 JSON 数据文件驱动，自动发现项目中所有 `AndroidBuildConfig.json` 并在构建时注入 Maven 仓库、Gradle 依赖、AndroidManifest 配置。

## 安装

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

## 使用方法

在项目的任意位置放置 `AndroidBuildConfig.json` 文件（如 `Assets/`、`Packages/` 下的任何目录）：

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

所有字段均为可选。项目中可放置多个配置文件，内容自动合并去重。

## 依赖

- `com.gameframex.unity >= 1.1.1`
