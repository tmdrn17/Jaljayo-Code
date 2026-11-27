# 세션 나누기 (동시 작업)

## 나눈 기준
- **결합도가 큰 것부터.** 공용 API(매니저·static 값)를 바꾸는 S1을 먼저 혼자 하고, 그 결과를 노트로 남긴다. 뒤 세션은 노트만 보고 따라가므로 앞 코드를 다시 읽지 않아 토큰이 줄어든다.
- **모델은 판단의 양에 맞춘다.**
  - Opus 5.5: 공용 설계를 정해야 하는 세션 (S1 하나만)
  - Sonnet 5.5: 정해진 설계를 따라 중간 크기·결합된 코드를 고치는 세션, 충돌을 풀어야 하는 합치기
  - Haiku 5.5: 작고 독립적인 스크립트, 확인·정리 작업
- **세션 하나 = 약 400~900줄.** 한 세션 안에서 컨텍스트가 넘치지 않게 했다.
- **동시에 돌릴 묶음은 코드 참조로 정했다.** 세션 사이 참조는 대부분 `Player`를 거친다. 그래서 Player(S2)가 끝난 뒤 S3·S4·S5를 동시에 돌린다. 남는 참조 두 곳은 "동결"로 막았다.
  - S5 `SecFloorClearKey` → S4 `LastDoor` : **LastDoor 동결**
  - S6 `ButtonManager` → S3 `Wall` : **Wall 동결**

## 단계
| 단계 | 세션 | 모델 | 작업 위치 | 시작 조건 |
|---|---|---|---|---|
| 1 | S1 기반·코어 매니저 | Opus 5.5 | 원래 폴더 (`main`) | 바로 |
| 2 | S2 플레이어·카메라 | Sonnet 5.5 | worktree | S1 커밋 후 |
| 2 | S6 UI·연출 | Haiku 5.5 | worktree | S1 커밋 후 |
| 2 끝 | M2 합치기 (S2+S6) | Sonnet 5.5 | 원래 폴더 (`main`) | S2, S6 커밋 후 |
| 3 | S3 장애물·보스·발판 | Sonnet 5.5 | worktree | M2 커밋 후 |
| 3 | S4 1층 퍼즐 | Sonnet 5.5 | worktree | M2 커밋 후 |
| 3 | S5 2층 | Sonnet 5.5 | worktree | M2 커밋 후 |
| 3 끝 | M3 합치기 (S3+S4+S5) | Sonnet 5.5 | 원래 폴더 (`main`) | S3, S4, S5 커밋 후 |
| 4 | S7 정리·trash·컴파일 | Haiku 5.5 | 원래 폴더 (`main`) | M3 커밋 후 |

- worktree는 **시작 조건이 된 뒤에** 만든다. worktree는 그 시점의 `main`에서 갈라지므로, 너무 일찍 만들면 앞 단계 결과가 들어 있지 않다.
- worktree 세션을 열 때는 Claude 앱에서 새 세션의 worktree 옵션을 켠다.
- 진행 상황은 `notes/` 폴더에 어떤 노트가 있는지로 본다. 아래 표는 합치기 세션(M2, M3)과 S7만 갱신한다.

## 시작 방법
새 세션에서 모델과 작업 위치를 고르고 이렇게 입력한다:
```
/jaljayo-refactor S1
/jaljayo-refactor M2
```

## 진행 상태
| 단계 | 상태 |
|---|---|
| 1 (S1) | 완료 |
| 2 (S2, S6) → M2 | 완료 (컴파일은 에디터에서 확인 필요) |
| 3 (S3, S4, S5) → M3 | 완료 (컴파일은 에디터에서 확인 필요) |
| 4 (S7) | 완료 (컴파일 오류 0개, 에디터 플레이 테스트 필요) |

---

## S1 기반·코어 매니저 — Opus 5.5, 원래 폴더 (약 560줄)
**왜 Opus:** 다른 모든 스크립트가 쓰는 static 값(`Health.HealthPoint`, `Score.coinAmount`, `CoinManager.CoinOnoff`, `GameManager.Instance`)의 새 형태를 정한다. 여기서 정한 것을 뒤 세션이 그대로 따른다.

**담당 파일**
- 새로 만들기: `Assets/02.Scripts/Core/SingletonBase.cs` (style.md 5번 기준)
- `GameManager`, `SaveManager`(+`SaveList`), `UI/Health`, `UI/Score`, `UI/TotalScore`, `CoinManager`, `Coin`, `UI/SceneChanger`, `GameSystem/StageClear`, `Clear`, `GameSystem/tmp`

**할 일**
- 매니저들을 `SingletonBase<T>`로 통일. `GameManager.Manager` 같은 중복 static 정리.
- `OnLevelWasLoaded`의 숫자 빌드 인덱스는 동작을 유지한 채 `const`로 이름을 붙인다.
- `tmp`는 하는 일에 맞는 이름으로 바꾼다 (`.meta` 같이 `git mv`).
- 혼자 하는 세션이므로 담당 밖 파일의 호출부도 컴파일되게 직접 최소로 고친다.
- **`notes/S1.md`를 반드시 채운다.** 바뀐 static/싱글톤 접근 방식, public 메서드 이름을 표로 남긴다. 뒤 세션은 이것만 본다.

**완료 기준:** 컴파일 에러 없음, `notes/S1.md` 작성, `main`에 커밋. 진행 상태 표의 1단계를 `완료`로 바꾼다.

---

## S2 플레이어·카메라 — Sonnet 5.5, worktree (약 720줄)
**담당 파일:** `Player.cs` (633줄, CP949), `Camera/Camera1.cs`

**할 일**
- `Player`를 `#region`(이동, 점프, 달리기, 공격, 상호작용, 피격)으로 나누고 긴 `OnTriggerEnter/Stay/Exit`는 대상별 private 메서드로 뺀다.
- 퍼즐용 static bool(`RedPush`, `BluePush`, `GreenPush`, `PurplePush`, `Wrong`, `HasStatue`, `ArrayNum`, `IsMove`, `IsBordera`)의 새 위치·이름을 정한다.
  - 새 위치가 Player 밖(예: `Button`)이면 그 파일은 S4 담당이다. 이번에는 Player 안에서 이름·형태만 정리하고, 옮길 곳은 노트에 제안으로 적는다.
- 애니메이터 파라미터 해시 캐싱, 태그 문자열 `const` 모으기.
- 새 컴포넌트로 나누면 씬에 붙여야 하므로, 한 클래스 안에서 나누는 것을 기본으로 한다.
- `Camera1`은 역할에 맞게 이름 변경 검토 (`git mv`로 GUID 유지).
- Player의 public/static 멤버를 쓰는 다른 파일(S1 매니저, S3·S4·S5 파일)은 고치지 않는다. **`notes/S2.md`의 "합칠 때 할 일"에 모두 적는다.** M2가 고친다.

---

## S6 UI·연출 — Haiku 5.5, worktree (약 470줄)
**담당 파일:** `UI/ButtonManager`, `UI/StageSelect`, `UI/CountDown`, `LJW/Fade`, `LJW/FadeOut`, `LJW/TitleUI`, `GameSystem/AnimationScript`

**할 일**
- 형식·이름·캡슐화를 style.md에 맞춘다. 설계를 크게 바꾸지 않는다.
- 이 파일들은 버튼 OnClick 연결이 많다 (`NewStartButton`, `LoadButton`, `QuitButton`, `TitleButton`, `BGMControl`, `SFXControl` 등). **public 메서드 이름은 되도록 유지.** 바꾸면 YAML `m_MethodName`도 같이 바꾼다.
- `Fade`/`FadeOut`은 공통 부분이 있으면 하나로 합치는 것을 검토 (S5의 Drag 합치기 방식 참고).
- `Wall`(S3)은 쓰기만 하고 그대로 둔다.
- 판단이 어려운 설계 문제가 나오면 `// TODO(리팩토링):`로 남기고 넘어간다.

---

## M2 합치기 — Sonnet 5.5, 원래 폴더
- `notes/S2.md`, `notes/S6.md`의 브랜치를 `main`에 합친다 (SKILL.md "합치기 순서").
- `notes/S2.md`의 "합칠 때 할 일"을 처리한다. 이때 S3·S4·S5 담당 파일의 Player 호출부도 고친다 (아직 아무도 작업하지 않으므로 안전).
- 컴파일 에러 0개를 확인하고 `notes/M2.md`에 결과를 적은 뒤 커밋한다.
- 이 커밋 뒤에 S3·S4·S5 worktree를 만들라고 사용자에게 알린다.

---

## S3 장애물·보스·발판 — Sonnet 5.5, worktree (약 880줄)
**담당 파일:** `Bullet`, `Spike`, `SpikeLauncher`, `Wall`, `Cube`, `Boss`, `LJW/BossHealth`, `Assets/Throwingstar`, `Platform/FallingPlatform`, `Platform/MoveFloor`, `Platform/PlatformAttach`, `LJW/MovePanel`, `LJW/WindPanel`

**동결:** `Wall`의 public/static 멤버 (S6 `ButtonManager`, S1 `GameManager`가 쓴다)

**할 일**
- 비슷한 이동 발판(`MoveFloor`, `MovePanel`, `WindPanel`, `FallingPlatform`)은 공통 흐름이 있으면 abstract 부모로 묶는다 (템플릿 메서드). 묶기 애매하면 각자 정리만.
- `Boss`의 공격 패턴은 enum + switch로 정리, 쿨타임은 코루틴.
- `Bullet`/`Spike` 거리 판정은 제곱 거리, 컴포넌트 캐싱.
- 루트에 있는 `Throwingstar.cs`는 `02.Scripts` 아래로 옮겨도 된다 (`.meta` 같이).
- `Bullet`, `Spike` 등의 공개 이름을 바꿔 `Player.cs`가 깨지면 노트에 적는다 (Player는 고치지 않음).

---

## S4 1층 퍼즐 — Sonnet 5.5, worktree (약 860줄)
**담당 파일:** `Button`, `ColorBox`, `ColorBoxAnswer`, `KeyPad`, `KeyPadAnswer`, `JukeBox`, `DoorAnimation`, `LastDoor`, `MazeKey`, `NewBehaviourScript`, `Paper`, `Invelope`, `Statue`, `Stage1/AnswerArea`, `Stage1/Altar`, `MainHallDoor`, `Skybox`, `LJW/DoorController`, `LJW/KeyBoard`

**동결:** `LastDoor`의 public/static 멤버 (S5 `SecFloorClearKey`가 쓴다)

**할 일**
- 색 버튼·정답 판정은 enum(`ColorKind` 등)으로 정리.
- `notes/S2.md`에 퍼즐 static 값을 옮길 제안이 있으면 따른다. `Player.cs` 쪽 호출부 변경은 노트에 적는다.
- `NewBehaviourScript`는 하는 일에 맞는 이름으로 변경 (`git mv`).
- `KeyPad`의 `PadNumber`는 버튼 OnClick에 연결돼 있으므로 이름을 바꾸면 YAML `m_MethodName`도 같이 바꾼다.
- `Altar`, `MainHallDoor`, `Invelope`는 빌드 씬에 없다 (MainHall 씬이 빌드 목록 밖). 정리는 하되, 코드 참조가 없으면 노트의 trash 후보에 적어 S7에 넘긴다.

---

## S5 2층 — Sonnet 5.5, worktree (약 580줄)
**담당 파일:** `SecondFloorManager`, `OXQuiz`, `FindPicture`, `SecFloorClearKey`, `LJW/Drag`, `LJW/Drag2`, `LJW/Stage2UI`, `LJW/Log`

**할 일**
- `Drag`와 `Drag2`(각 90줄)는 중복이 크다. 하나로 합치거나 abstract 부모 + 자식으로 나눈다.
  - 하나로 합치면: 남는 스크립트 GUID로 `2Floor.unity`의 컴포넌트 `m_Script`를 바꾸고, 차이 나는 값은 인스펙터 필드로 뺀 뒤 YAML에 값을 넣는다. 안 쓰게 된 쪽은 trash로.
- `Log`는 `Debug.Log`와 이름이 겹쳐 헷갈리므로 역할에 맞게 이름 변경 검토.
- CP949 파일 3개(`Drag`, `Drag2`, `Stage2UI`)의 주석을 다시 쓴다.
- `LastDoor`(S4)는 쓰기만 하고 그대로 둔다.

---

## M3 합치기 — Sonnet 5.5, 원래 폴더
- `notes/S3.md`, `notes/S4.md`, `notes/S5.md`의 브랜치를 차례로 `main`에 합친다. 씬 파일 충돌 가능성이 가장 큰 단계다.
  - 세 세션이 같이 고친 씬·프리팹: `RunningStage.unity`, `MazeStage.unity`, `PortalStage.unity`, `2Floor.unity`, `Canvas.prefab`
- 세 노트의 "합칠 때 할 일"을 처리한다 (주로 `Player.cs`, `GameManager.cs` 호출부).
- 컴파일 에러 0개를 확인하고 `notes/M3.md`에 결과를 적은 뒤 커밋한다.

---

## S7 정리·trash·컴파일 — Haiku 5.5, 원래 폴더 (약 390줄 + 전체 확인)
**trash 후보** (inventory.md 기준 빌드 씬에 없음. 옮기기 전에 SKILL.md 조건을 다시 grep으로 확인):
`ClearSceneDoor`, `Enemy`, `ImgBlinkAnim`, `Portal`, `TutorialFall`, `UI/Timer`, `UI/Title`, `LJW/CameraCollision`, `LJW/CameraTest`, `LJW/TestPlayer`, `LJW/Timer_4`, `Assets/SoundManager`
+ 모든 노트의 "trash 후보"

**할 일**
1. 후보마다: GUID가 어떤 씬·프리팹에 있는지, 다른 `.cs`가 클래스 이름을 쓰는지 확인.
   - 빌드 밖 씬(Test 씬, `2Stage C`, `Past_Title`, 옛 `StageClear` 등)에만 붙어 있으면 trash로 옮기고 사용자에게 보고한다 (그 씬에서는 Missing Script가 된다).
   - 빌드 씬에 붙어 있거나 코드 참조가 있으면 옮기지 않고 style.md에 맞춰 정리만 한다.
2. 남은 스크립트 전체에서 style.md 위반을 grep으로 점검: `!` 부정, `var `, `using System.Linq`, `_` 접두사 필드, 같은 줄 여는 중괄호.
3. 전체 컴파일 (SKILL.md 명령). 에러가 많거나 설계 판단이 필요하면 **Sonnet 5.5로 바꿔서** 다시 시작하라고 사용자에게 알린다.
4. Missing Script 점검: 모든 `.unity`/`.prefab`의 `m_Script` GUID가 `Assets` 안 `.cs.meta`에 있는지 확인하고 목록을 보고한다.
5. 모든 노트의 "에디터에서 할 일"과 플레이 테스트할 씬을 모아 사용자에게 보고한다.
6. `notes/S7.md`를 쓰고 커밋, 진행 상태 표를 `완료`로 바꾼다.


---
---

# 2차 리팩터링 (구조 개선)

무엇을 바꿀지는 [plan-2nd.md](plan-2nd.md)에 R1~R12로 정해져 있다. 각 세션은 아래 "적용 항목"의 R 절만 읽는다. 사전 확인 P1~P4는 끝났다(plan-2nd.md 1절).

## 나눈 기준 (1차와 같은 원칙)
- **결합도가 큰 것부터.** 공용 API(`GameSession`, `GameManager`, `SaveManager`, `SceneNames`, `Tags`)를 바꾸는 T1을 먼저 혼자 하고 `notes/T1.md`에 남긴다. 뒤 세션은 노트만 보고 따라간다.
- **세션 사이 호출은 "약속 API"로 막는다** (plan-2nd.md 8절). 부르는 쪽과 만드는 쪽이 따로 작업하고 N2에서 맞춘다.
- **같은 씬·프리팹을 두 세션이 고치지 않는다.** T3은 Boss.prefab·BossRoom, T4는 PortalStage·MazeStage·동물 프리팹·EscapeStage만 고친다.
- 모델: 공용 설계는 Opus 5.5, 정해진 설계를 따르는 결합 코드·합치기는 Sonnet 5.5, 확인·정리는 Haiku 5.5.

## 단계
| 단계 | 세션 | 모델 | 작업 위치 | 시작 조건 |
|---|---|---|---|---|
| 1 | T1 공용 기반·매니저 | Opus 5.5 | 원래 폴더 (`main`) | 바로 |
| 2 | T2 플레이어·카메라 | Sonnet 5.5 | worktree | T1 커밋 후 |
| 2 | T3 보스·장애물·발판 | Sonnet 5.5 | worktree | T1 커밋 후 |
| 2 | T4 1층 퍼즐 | Sonnet 5.5 | worktree | T1 커밋 후 |
| 2 | T5 2층·UI | Sonnet 5.5 | worktree | T1 커밋 후 |
| 2 끝 | N2 합치기 (T2~T5) | Sonnet 5.5 | 원래 폴더 (`main`) | T2~T5 커밋 후 |
| 3 | T6 OXQuiz 데이터 (선택) | Sonnet 5.5 | 원래 폴더 (`main`) | N2 커밋 후 |
| 4 | T7 정리·확인 | Haiku 5.5 | 원래 폴더 (`main`) | N2(또는 T6) 커밋 후 |

- worktree는 **T1 커밋 뒤에** 만든다.
- 시작: `/jaljayo-refactor T1`, `/jaljayo-refactor N2` 처럼 입력한다.

## 진행 상태 (2차)
| 단계 | 상태 |
|---|---|
| 0 (사전 확인 P1~P4) | 완료 |
| 1 (T1) | 완료 |
| 2 (T2, T3, T4, T5) → N2 | 완료 |
| 3 (T6, 선택) | 완료 (퀴즈 플레이 확인 필요) |
| 4 (T7) | 완료 (컴파일 에러 0개 확인, 에디터 플레이 테스트 필요) |

## 동결 (2단계 동안)
`GameSession` 전체, `GameManager`의 public 멤버 전체, `SaveManager`, `SceneNames`, `Tags` (T1이 만든 것). 2단계 세션은 public 이름·형태를 바꾸지 않는다.

## 약속 API (2단계)
| API | 만드는 세션 | 부르는 세션 |
|---|---|---|
| `Button.Activate()` | T4 | T2 |
| `AnswerArea.PlaceStatue(Statue statue)` | T4 | T2 |
| `Statue.Kind`, `Statue.Value` (읽기 전용 프로퍼티) | T4 | T2 |
| `Stage2UI.ChangePoint(int collectedCount)` (늘린 뒤의 개수) | T5 | T2 |
| `Stage2UI.ShowQuest(int)` | 그대로 | T2 |
| `Player.SetMovePanel(bool)`, `Player.Rig`, `Player.ResetJump()`, `Player.ItemIndex` | T2가 유지 | T3, T5 |

---

## T1 공용 기반·매니저 — Opus 5.5, 원래 폴더 (약 900줄)
**왜 Opus:** 다른 모든 세션이 쓰는 공용 API(`GameSession`, GameManager의 흐름 메서드, 이동 잠금)의 실제 형태를 정한다.

**적용 항목:** R1, R3, R4, R5, R6 (+ 담당 파일의 R9)

**담당 파일**
- 새로 만들기: `Core/SceneNames.cs`, `Core/Tags.cs`, `Core/GameSession.cs`
- `GameManager`, `SaveManager`, `UI/Health`, `UI/Score`, `UI/TotalScore`, `UI/CountDown`, `UI/ButtonManager`, `UI/StageSelect`, `UI/SceneChanger`, `Coin`, `CoinManager`

**씬·프리팹:** 없음. GameManager `paperNum`·`remainTime`, SaveManager `player`, Health `startHp`는 죽은 키로 남긴다.

**담당 밖 호출부 (혼자 하는 세션이라 직접 최소로 고친다)**
- `Health.*`·`Score.*`·`TotalScore.*` → `GameSession.*` (Player, Bullet, Button, KeyPad, OXQuiz, FindPicture, Cube, AnswerArea)
- `Player.IsMove`/`SetMove` → `GameManager.IsMoveLocked`/`EnterPuzzle`/`ExitPuzzle`/`UnlockMove` (Player, CameraArm, OXQuiz, FindPicture, KeyPad, LJW Stage2UI)
- `GameManager.AddHealthCoin` → `CollectCoin` (Coin), Player 낙사 → `GameOver`
- 그 밖에 R1 상수로 바꾸는 것은 담당 파일 안에서만 한다. 다른 파일은 각 세션이 바꾼다.

**완료 기준:** 사용자에게 에디터 콘솔 컴파일 에러 0개를 확인받는다. `notes/T1.md`(바뀐 공용 API 표, 약속 API 확정)를 쓰고 `main`에 커밋한다. 진행 상태 표의 1단계를 `완료`로 바꾼다.

---

## T2 플레이어·카메라 — Sonnet 5.5, worktree (약 630줄)
**적용 항목:** R7-1~R7-4 (Player 쪽), R5-7 (T1이 하지 않은 Player 퍼즐 창 호출), R9

**담당 파일:** `Player`, `Camera/CameraArm`

**할 일**
- `StayButton` → `button.Activate()`
- `PlaceStatue` → `answerArea.PlaceStatue(statue)`, `ClearNearObj` 삭제
- `EnterDocument` → `ChangePoint(ItemIndex)`, `items` 삭제
- `IsPaused`·`IsUIButtonClick`과 setter 삭제
- 조정 값 const → `[SerializeField]`, `AttackPaperCount` → `GameManager.Instance.HasAllPapers`
- R1 상수(`Tags`, `SceneNames`)로 바꾼다

**씬·프리팹:** 없음 (`items`는 죽은 키).
**합칠 때 할 일에 적을 것:** Stage2UI의 `SetPaused`·`SetUIButtonClick` 호출은 T5가 지운다.

---

## T3 보스·장애물·발판 — Sonnet 5.5, worktree (약 850줄 + YAML)
**적용 항목:** R10, R2, R7-6, R9

**담당 파일:** `Boss`, 새로 만들기 `BossAttacks/BossAttack.cs`·`LampDropAttack.cs`·`FileFireAttack.cs`·`TileStrikeAttack.cs`, `Bullet`, `Spike`, `SpikeLauncher`, `Wall`, `Cube`, `Throwingstar`, `Skybox`, `Platform/PlatformAttach`, `Platform/FallingPlatform`, (LJW) `BossHealth`, `MovePanel`, `WindPanel`

**씬·프리팹:**
- `Assets/03.Prefabs/Enemy/Boss.prefab`의 Boss 블록을 `jumpAttack`/`yellAttack`/`strikeAttack`으로 다시 짠다.
- `Assets/01.Scenes/Stage1/5.Boss/BossRoom.unity`의 오버라이드 `propertyPath` 약 120줄(`lampPrefab`, `lampPoints`, `tiles`)을 고친다.
- 바꾸기 전후 값을 노트에 적는다.

**완료 기준:** `notes/T3.md`, 브랜치 커밋. N2 뒤 BossRoom 플레이 확인 항목을 노트의 "에디터에서 할 일"에 적는다.

---

## T4 1층 퍼즐 — Sonnet 5.5, worktree (약 550줄 + YAML)
**적용 항목:** R7-1·R7-2 (Button·AnswerArea 쪽), R11, R8, R9

**담당 파일:** `Button`, `Statue`, `Stage1/AnswerArea`, `Stage1/Altar`, `ColorBox`, `ColorBoxAnswer`, `DoorAnimation`, `LastDoor`, `Paper`, `MazeKey`, `SecFloorClearKey`, `KeyPad`

**할 일:**
- 약속 API `Button.Activate()`, `AnswerArea.PlaceStatue(Statue)`, `Statue.Kind`/`Value`를 만든다.
- Button `MoveSpace`는 `GameManager.MoveCharacter`를 쓴다.
- ColorBox·DoorAnimation·LastDoor를 이벤트·메서드로 바꾼다.
- `correctNum.Count`를 쓴다 (P2 확인 완료).

**씬·프리팹:**
- `PortalStage.unity`의 Button 블록 12개를 고친다 (`ButtonKind`→`kind`, `OpenSound`→`openSound`).
- Button 대상 오버라이드를 고친다 (PortalStage 1줄, MazeStage 1줄).
- 동물 프리팹 4개의 Statue 블록을 고친다 (`type`→`kind`, `Value`→`value`).
- `EscapeStage.unity`의 동물 프리팹 대상 오버라이드 12줄을 고친다.

**완료 기준:** `notes/T4.md`(키 변경 전후 값), 브랜치 커밋.

---

## T5 2층·UI — Sonnet 5.5, worktree (약 400줄)
**적용 항목:** R7-3·R7-5 (Stage2UI 쪽), R8, R9

**담당 파일:** `OXQuiz`, `FindPicture`, `SecondFloorManager`, `GameSystem/VolumeControl`, (LJW) `Stage2UI`

**할 일:**
- 약속 API `ChangePoint(int collectedCount)`를 만든다.
- Stage2UI의 Player 참조를 모두 없앤다 (`RoomRespawn` → `GameManager.MovePlayer`, `SetPaused`·`SetUIButtonClick` 호출 삭제).
- SecondFloorManager를 이벤트로 바꾼다.
- OXQuiz 목숨 표시는 값이 바뀔 때만 갱신한다.
- 조정 값을 인스펙터로 옮긴다 (`startLife`, `startChance`, `clearMissionCount`).
- `audioSlider.minValue`를 쓴다.

**씬·프리팹:** 없음.

---

## N2 합치기 — Sonnet 5.5, 원래 폴더
- T2~T5 브랜치를 차례로 `--no-ff`로 합친다 (SKILL.md "합치기 순서").
- 각 노트의 "합칠 때 할 일"을 처리하고, 약속 API가 양쪽에서 같은지 확인한다.
- 사용자에게 에디터 콘솔 컴파일 에러 0개를 확인받는다. `notes/N2.md`를 쓰고 커밋한다.

---

## T6 OXQuiz 데이터 (선택) — Sonnet 5.5, 원래 폴더
**적용 항목:** R12.
- `Canvas.prefab` 오버라이드에서 문제·정답 20쌍을 뽑는다.
- **표를 사용자에게 먼저 보여 주고 확인받은 뒤** `O_XQuiz.prefab`의 OXQuiz 블록에 기록한다.
- 퀴즈 10문제 플레이 확인 항목을 노트에 적는다.

---

## T7 정리·확인 — Haiku 5.5, 원래 폴더
1. style.md 위반을 grep으로 점검한다: `!`, `var `, LINQ, `_` 접두사, `.tag ==`, `Update` 안 텍스트 갱신, `[FormerlySerializedAs]`·`[Tooltip]`·`[Min]`.
2. 모든 노트의 죽은 키 목록을 모은다.
3. `리팩토링_결과.md`에 2차 절을 추가한다.
4. 사용자에게 에디터 컴파일 확인을 받는다. Missing Script를 점검한다.
5. plan-2nd.md 10절의 플레이 테스트 목록과 모든 노트의 "에디터에서 할 일"을 모아 사용자에게 보고한다.
6. `notes/T7.md`를 쓰고 커밋한다. 진행 상태 표를 `완료`로 바꾼다.
