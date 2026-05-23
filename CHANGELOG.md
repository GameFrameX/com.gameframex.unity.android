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
