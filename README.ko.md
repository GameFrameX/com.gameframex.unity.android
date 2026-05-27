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

범용 Android 빌드 자동 설정 시스템입니다. JSON 데이터 파일로 구동되며, 프로젝트 내의 모든 `AndroidBuildConfig.json` 파일을 자동으로 검색하여 빌드 시 Maven 저장소, Gradle 종속성, AndroidManifest 항목, string 리소스 및 현지화 설정을 주입합니다.

주요 기능:

- **데이터 기반**: SDK 패키지는 JSON 파일만 필요, C# 코드 불필요
- **다중 파일 지원**: 프로젝트에 임의의 수의 설정 파일 배치 가능
- **임의 위치**: Unity가 인식하는 모든 위치에 설정 파일 배치 가능
- **자동 병합 및 중복 제거**: 여러 설정 파일을 키 기반으로 자동 병합 및 중복 제거
- **멱등성**: 여러 번 빌드해도 안전 — 기존 항목은 자동 건너뜀

## 빠른 시작

### 설치

Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션을 추가하세요:

```json
{
  "scopedRegistries": [
    {
      "name": "GameFrameX",
      "url": "https://gameframex.upm.alianblank.uk",
      "scopes": [
        "com.gameframex"
      ]
    }
  ],
  "dependencies": {
    "com.gameframex.unity.android": "1.0.0"
  }
}
```

`scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

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

모든 필드는 선택 사항입니다. 여러 설정 파일을 배치할 수 있으며, 내용은 자동으로 병합 및 중복 제거됩니다.

### 필드 설명

#### 전역 필드

| 필드 | 타입 | 병합 전략 | 설명 |
|------|------|----------|------|
| `providerName` | string | 나중에 쓴 값 우선 | 설정 제공자 이름, 빌드 로그 식별용으로만 사용 |
| `mavenRepositories` | `[{name, url}]` | URL로 중복 제거 | Maven 저장소 목록, 루트 `settings.gradle`의 `dependencyResolutionManagement.repositories`에 주입 |
| `gradleWrapper` | `{key: value}` | 키로 덮어쓰기 | `gradle-wrapper.properties`에 주입할 키-값 쌍 |
| `gradleProperties` | `{key: value}` | 키로 덮어쓰기 | `gradle.properties`에 주입할 키-값 쌍 |
| `fileCopies` | `{소스: 대상}` | 소스로 덮어쓰기 | 파일 복사 맵. 상대 소스는 설정 파일 디렉토리에서, 상대 대상은 Gradle 루트에서 해결 |
| `directoryCopies` | `{소스: 대상}` | 소스로 덮어쓰기 | 디렉토리 복사 맵. 경로 해결은 `fileCopies`와 동일 |
| `assetPacksEnabled` | bool | 나중에 쓴 값 우선 | Google Play Asset Delivery 기능 활성화. 기본값 `false` |
| `assetPacks` | `[{name, deliveryType, streamingAssetsPath}]` | name으로 중복 제거 | Asset Pack 설정 목록. 각 팩은 독립적인 Gradle 하위 프로젝트가 됩니다 |
| `localizedStringResources` | `{locale: {key: value}}` | locale별로 그룹화, 각 그룹 내에서 키별로 나중에 쓴 값 우선 | 현지화된 string 리소스, launcher의 `res/values-<locale>/strings.xml`에 주입 |

#### 모듈 필드 (launcher / unityLibrary)

| 필드 | 타입 | 병합 전략 | 설명 |
|------|------|----------|------|
| `compileSdkVersion` | string | 최대값 | 컴파일 SDK 버전, `android {}` 블록에 주입 |
| `buildToolsVersion` | string | 나중에 쓴 값 우선 | 빌드 도구 버전 |
| `minSdkVersion` | string | 최대값 | 최소 SDK 버전 |
| `targetSdkVersion` | string | 최대값 | 대상 SDK 버전 |
| `applicationId` | string | 나중에 쓴 값 우선 | 애플리케이션 패키지 이름 (launcher에만 적용) |
| `versionName` | string | 나중에 쓴 값 우선 | 버전 이름 |
| `dependencies` | `[{configuration, notation}]` | `configuration:notation`으로 중복 제거 | Gradle 종속성 목록. `configuration`은 `implementation`, `api` 등 |
| `permissions` | `[string]` | 이름으로 중복 제거 | AndroidManifest `<uses-permission>` 항목 |
| `metaData` | `[{name, value}]` | name으로 중복 제거 | AndroidManifest `<application>` 아래의 `<meta-data>` 항목 |
| `applicationAttributes` | `[{name, value}]` | 이름으로 중복 제거 | AndroidManifest `<application>` 태그의 속성 (`android:` 접두사 없음) |
| `signingConfig` | `{storeFile, storePassword, keyAlias, keyPassword}` | 나중에 쓴 값 우선 | 서명 설정, `signingConfigs.release` 및 `buildTypes.release`에 주입 |
| `stringResources` | `{key: value}` | 키별로 나중에 쓴 값 우선 | (레거시) string 리소스, `res/values/strings.xml`에 주입. 자동으로 `resources.string`에 병합 |
| `resources` | `{type: {key: value/object}}` | type별로 그룹화, 각 그룹 내에서 키별로 나중에 쓴 값 우선 | 멀티타입 values 리소스, `res/values/strings.xml`에 주입. 아래 '리소스 값 형식' 참조 |

## 설정 참조

### 리소스 값 형식

`resources` 필드는 중첩 딕셔너리를 사용합니다: 외부 키 = XML 요소 타입, 내부 키 = 리소스 이름, 내부 값 = 텍스트 또는 객체.

**단순 문자열 값** (추가 XML 속성 없음):
```json
"resources": {
  "string": { "app_name": "My App" },
  "integer": { "max_retry_count": "3" }
}
```
출력: `<string name="app_name">My App</string>` 및 `<integer name="max_retry_count">3</integer>`

**객체 값** (추가 XML 속성 포함):
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
출력: `<string name="facebook_app_id" translatable="false">724358863922328</string>`

**지원 리소스 타입**: `string`, `integer`, `bool`, `color`, `dimen` (및 기타 XML 요소 타입 - 태그 이름이 직접 사용됨).

**하위 호환성**: `stringResources`는 자동으로 `resources.string`에 병합됩니다. 동일한 키가 양쪽에 선언된 경우 `resources`가 우선합니다.

### stringResources 자주 사용하는 Key

| Key | SDK / 서비스 | 설명 |
|-----|-------------|------|
| `app_name` | Android | 애플리케이션 표시 이름 (홈 화면 아이콘 이름) |
| `facebook_app_id` | Facebook SDK | Facebook 애플리케이션 ID |
| `facebook_client_token` | Facebook SDK | Facebook 클라이언트 토큰 |
| `facebook_app_scheme` | Facebook SDK | Facebook URL Scheme, 형식: `fb` + facebook_app_id |
| `google_app_id` | Google Services | Google 애플리케이션 ID (형식: `1:숫자:android:해시`) |
| `default_web_client_id` | Google Sign-In | OAuth 2.0 웹 클라이언트 ID (`.apps.googleusercontent.com`으로 끝남) |
| `firebase_database_url` | Firebase | Realtime Database URL |
| `gcm_defaultSenderId` | Firebase Cloud Messaging | FCM 발신자 ID |
| `project_id` | Firebase | Firebase 프로젝트 ID |
| `google_api_key` | Google | Google API Key (`AIzaSy`로 시작) |
| `google_crash_reporting_api_key` | Firebase Crashlytics | Crash Reporting API Key |
| `google_storage_bucket` | Firebase Storage | Cloud Storage Bucket (`프로젝트ID.appspot.com`) |

> 참고: Google/Firebase 관련 key는 일반적으로 `google-services.json` Gradle 플러그인에 의해 자동 생성됩니다. Gradle 플러그인을 사용할 수 없는 경우에만 `stringResources`로 수동 주입하세요.

### localizedStringResources

현지화된 string 리소스는 `launcher/src/main/res/values-<locale>/strings.xml`에 주입됩니다. Android는 기기 언어 설정에 따라 적절한 리소스 파일을 자동으로 선택하며, 일치하는 언어를 찾을 수 없는 경우 `res/values/strings.xml`로 대체됩니다.

대부분의 게임은 `app_name`만 현지화하면 됩니다. 다른 SDK 토큰/ID는 언어에 관계없이 동일하므로 `stringResources`에 배치하세요.

### Asset Pack deliveryType

| 값 | 설명 | 크기 제한 |
|----|------|----------|
| `install-time` | 앱 설치 시 함께 제공 | 팩당 ≤ 150MB |
| `fast-follow` | 설치 후 자동으로 백그라운드에서 다운로드 | 팩당 ≤ 150MB |
| `on-demand` | 앱에서 요청할 때 다운로드 | 팩당 ≤ 150MB |

### 자주 사용하는 locale 코드

| locale | 언어 | locale | 언어 |
|--------|------|--------|------|
| `en` | English | `de` | Deutsch |
| `zh-rCN` | 简体中文 | `fr` | Français |
| `zh-rTW` | 繁體中文 | `es` | Español |
| `ja` | 日本語 | `pt` | Português |
| `ko` | 한국어 | `ru` | Русский |
| `th` | ไทย | `ar` | العربية |
| `vi` | Tiếng Việt | `tr` | Türkçe |
| `id` | Bahasa Indonesia | | |

## 종속성

- `com.gameframex.unity >= 1.1.1`

## 변경 로그

자세한 내용은 [CHANGELOG.md](CHANGELOG.md)를 참조하세요.

## 라이선스

[Apache 라이선스 2.0](https://www.apache.org/licenses/LICENSE-2.0)에 따라 배포됩니다. 자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.

GameFrameX 조직 및 그 파생 프로젝트의 저작권, 상표, 특허 및 기타 관련 권리는 관련 법률 및 규정에 의해 보호됩니다. 국가 안전을 위협하거나 사회 질서를 문란하게 하거나 타인의 합법적인 권리를 침해하는 등 법률로 금지된 활동에 본 프로젝트를 사용해서는 안 됩니다.
