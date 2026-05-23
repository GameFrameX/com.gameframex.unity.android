<div align="center">

  <img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160" />

  # Game Frame X Android

  [![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.android?label=version)](https://github.com/GameFrameX/com.gameframex.unity.android/releases)
  [![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.android?label=license)](https://github.com/GameFrameX/com.gameframex.unity.android/blob/main/LICENSE.md)
  [![Documentation](https://img.shields.io/badge/documentation-docs-blue)](https://gameframex.doc.alianblank.com)

  인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

  [문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작)

  [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

---

## 프로젝트 개요

범용 Android 빌드 자동 설정 시스템입니다. JSON 데이터 파일로 구동되며, 프로젝트 내의 모든 `AndroidBuildConfig.json` 파일을 자동으로 검색하여 빌드 시 Maven 저장소, Gradle 종속성, AndroidManifest 항목을 주입합니다.

주요 기능:

- **데이터 기반**: SDK 패키지는 JSON 파일만 필요, C# 코드 불필요
- **다중 파일 지원**: 프로젝트에 임의의 수의 설정 파일 배치 가능
- **임의 위치**: Unity가 인식하는 모든 위치에 설정 파일 배치 가능
- **자동 병합 및 중복 제거**: 여러 설정 파일을 키 기반으로 자동 병합 및 중복 제거
- **멱등성**: 여러 번 빌드해도 안전 — 기존 항목은 자동 건너뜀

## 빠른 시작

### 설치

프로젝트의 `Packages/manifest.json`에 추가:

```json
{
  "dependencies": {
    "com.gameframex.unity.android": "https://github.com/gameframex/com.gameframex.unity.android.git"
  }
}
```

### 사용 예시

프로젝트 내 임의의 위치에 `AndroidBuildConfig.json` 파일을 배치합니다 (예: `Assets/` 또는 `Packages/` 하위 디렉토리):

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
    "dependencies": [
      { "configuration": "implementation", "notation": "com.example:sdk-analytics:2.0.0" }
    ],
    "permissions": [ "android.permission.INTERNET" ],
    "metaData": [
      { "name": "com.example.APP_ID", "value": "your_app_id" }
    ]
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
  "fileCopies": {
    "libs/your-sdk.aar": "launcher/libs/your-sdk.aar"
  },
  "directoryCopies": {
    "jniLibs": "launcher/jniLibs"
  }
}
```

모든 필드는 선택 사항입니다. 여러 설정 파일을 배치할 수 있으며, 내용은 자동으로 병합 및 중복 제거됩니다.

- **`launcher`**: Android 앱 셸에 적용 (`launcher/build.gradle`, `launcher/AndroidManifest.xml`)
- **`unityLibrary`**: Unity 엔진 라이브러리에 적용 (`unityLibrary/build.gradle`, `unityLibrary/AndroidManifest.xml`)
- **`mavenRepositories`**: 루트 `settings.gradle`에 주입
- **`gradleWrapper`**: 키-값 쌍을 `gradle-wrapper.properties`에 주입
- **SDK 버전** (`compileSdkVersion`/`buildToolsVersion`/`minSdkVersion`/`targetSdkVersion`): 병합 시 모든 설정에서 최대 숫자값을 채택 (`buildToolsVersion`은 마지막 작성자 우선)
- **`fileCopies`**: 키-값 매핑 (소스 경로 → 대상 경로), Gradle 프로젝트에 파일 복사. 상대 경로 및 절대 경로 지원
- **`directoryCopies`**: 키-값 매핑 (소스 경로 → 대상 경로), Gradle 프로젝트에 디렉토리 복사. 상대 경로 및 절대 경로 지원

## 종속성

- `com.gameframex.unity >= 1.1.1`

## 변경 로그

자세한 내용은 [CHANGELOG.md](CHANGELOG.md)를 참조하세요.

## 라이선스

[Apache 라이선스 2.0](https://www.apache.org/licenses/LICENSE-2.0)에 따라 배포됩니다. 자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.

GameFrameX 조직 및 그 파생 프로젝트의 저작권, 상표, 특허 및 기타 관련 권리는 관련 법률 및 규정에 의해 보호됩니다. 국가 안전을 위협하거나 사회 질서를 문란하게 하거나 타인의 합법적인 권리를 침해하는 등 법률로 금지된 활동에 본 프로젝트를 사용해서는 안 됩니다.
