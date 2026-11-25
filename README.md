# 잘자요 (Jaljayo)

> 꿈속의 공간을 탐험하며 퍼즐과 장애물을 해결하고, 보스에 도전하는 3D 탈출 어드벤처

<img width="1280" height="720" alt="jaljayo_title" src="https://github.com/user-attachments/assets/5d0013f5-ccd9-4cb7-b14c-60236e36937d" />

> **코드 공개용 저장소입니다.** 원본 Unity 프로젝트에서 현재 사용 중인 C# 스크립트를 옮겼습니다. 씬·프리팹·모델·텍스처·사운드 등 프로젝트 에셋과 빌드 파일은 포함하지 않습니다.
> 따라서 이 저장소만으로 Unity에서 게임을 바로 실행할 수는 없습니다.

## 개요

| 항목 | 내용 |
|---|---|
| 장르 | 3D 퍼즐·탐험 / 탈출 어드벤처 |
| 환경 | Unity 2020.3.17f1, C# |
| 저장소 목적 | 게임 플레이 로직과 스크립트 구조 공개 |

## 핵심 기능

| 기능 | 설명 | 코드 |
|---|---|---|
| 3D 플레이어 조작 및 상호작용 | 카메라 방향을 기준으로 이동하고 점프·공격·오브젝트 상호작용을 처리합니다. | [Player.cs](Scripts/Player.cs) · [CameraArm.cs](Scripts/Camera/CameraArm.cs) |
| 스테이지별 퍼즐 | 색상 상자, O/X 퀴즈, 그림 찾기, 키패드, 제단 퍼즐 등 스테이지 진행 요소를 구현합니다. | [ColorBox.cs](Scripts/ColorBox.cs) · [OXQuiz.cs](Scripts/OXQuiz.cs) · [FindPicture.cs](Scripts/FindPicture.cs) · [KeyPad.cs](Scripts/KeyPad.cs) |
| 보스 전투 | 보스가 공격 패턴을 선택해 실행하고, 플레이어의 공격에 반응합니다. | [Boss.cs](Scripts/Boss.cs) · [BossAttacks/](Scripts/BossAttacks/) |
| 진행 상황 저장·불러오기 | 체력·점수·수집 상태·현재 씬·플레이어 위치를 JSON 파일로 저장하고 복원합니다. | [SaveManager.cs](Scripts/SaveManager.cs) · [GameSession.cs](Scripts/Core/GameSession.cs) |
| 게임 진행 및 UI | 씬 전환, 제한 시간, 체력·점수 표시, 일시정지, 페이드 연출 등을 관리합니다. | [GameManager.cs](Scripts/GameManager.cs) · [UI/](Scripts/UI/) |

## 코드 공개 범위

이 저장소에는 원본 프로젝트에서 사용 중인 C# 스크립트만 모았습니다.

- 원본 프로젝트의 `Assets/02.Scripts/`와 `Assets/LJW/Script/`에 있던 활성 스크립트를 `Scripts/` 아래로 정리했습니다.
- 사용되지 않는 스크립트는 제외했습니다.
- DOTween 등 외부 라이브러리의 소스는 복사하지 않고 사용 라이브러리 항목에만 기록했습니다.
- 파일별 개인 작성 비율은 별도로 산정하지 않았으므로, 이 저장소는 개인 기여도 증빙이 아닌 코드 열람용입니다.

## 폴더 구조

```
Scripts/
├── BossAttacks/  (4)
├── Camera/
├── Core/         (6)
├── GameSystem/
├── Platform/
├── Stage1/
├── UI/           (7)
├── LJW/          (11)
└── 플레이어·퍼즐·스테이지 관련 스크립트
images/
└── jaljayo_logo.png
```

현재 사용 중인 C# 스크립트 총 **70개**를 옮겼습니다. 세부 경로와 파일은 저장소에서 확인할 수 있습니다.

## 사용 라이브러리 · 패키지

원본 프로젝트의 패키지 설정을 기준으로 정리했습니다.

- DOTween — 타이틀 UI 애니메이션
- Cinemachine
- Universal Render Pipeline (URP)
- TextMesh Pro
- Unity UI (uGUI)
- Input System

외부 라이브러리와 에셋은 이 코드 공개용 저장소에 포함하지 않았습니다.
