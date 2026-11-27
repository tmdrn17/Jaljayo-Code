---
name: jaljayo-refactor
description: Jaljayo(Unity 2020.3) 스크립트를 본인 코딩 스타일로 리팩토링하는 세션용 기준. "S1 시작", "T1 시작", "리팩토링 T3 진행", "N2 합치기"처럼 세션 번호나 합치기 단계와 함께 요청하면 사용한다.
---

# Jaljayo 리팩토링

사용자가 말한 세션이나 합치기 작업만 한다. 번호가 없으면 [sessions.md](sessions.md)와 `notes/` 폴더를 보고 다음 할 일을 물어본다.
- **1차 (완료):** 세션 S1~S7, 합치기 M2·M3. 스타일 맞추기.
- **2차:** 세션 T1~T7, 합치기 N2. 구조 개선. 무엇을 바꿀지는 [plan-2nd.md](plan-2nd.md)(최종안)에 R1~R12로 정해져 있다.

여러 세션이 **동시에** 진행된다. 아래 "동시 작업 규칙"을 반드시 지킨다.

## 시작할 때 읽을 것 (이 순서로, 필요한 부분만)
1. [sessions.md](sessions.md) — 내 세션 부분만 읽는다. 담당 파일, 단계, 작업 위치, 완료 기준이 있다.
2. **(2차 세션만)** [plan-2nd.md](plan-2nd.md) — 내 세션에 적힌 R 항목 절만 읽는다. 바꿀 코드, 씬·프리팹 작업, 동작 차이가 있다. 8절(동결·약속 API)은 반드시 읽는다.
3. `notes/` 폴더의 **앞 단계** 세션 파일 — 앞 세션이 바꾼 공용 API. 여기 적힌 이름을 그대로 쓴다. 같은 단계 세션의 노트는 아직 없으니 기다리지 않는다. (2차 2단계는 `notes/T1.md`)
4. [style.md](style.md) — 코드 스타일 기준. 반드시 지킨다. 2차는 8절 판단 기준도 지킨다.
5. [inventory.md](inventory.md) — 1차 시작 때 만든 목록이라 파일 이름이 옛 이름이다. 2차에서는 붙어 있는 곳을 `.cs.meta`의 GUID로 grep해서 다시 확인한다.

## 동시 작업 규칙
- **작업 위치:** sessions.md에 "worktree"로 표시된 세션은 worktree에서 연다. 시작하면 `git branch --show-current`로 확인하고, 원래 폴더라면 작업하지 말고 사용자에게 worktree로 다시 열어 달라고 한다. "원래 폴더"로 표시된 세션(S1, M2, M3, S7, T1, N2, T6, T7)은 원래 폴더에서 한다.
- **담당 파일만 고친다.** 담당 밖 `.cs`는 읽기만 한다. 같은 단계의 다른 세션도 같은 파일(특히 `Player.cs`, `GameManager.cs`)의 호출부를 고치려 할 수 있어서 충돌난다.
  - 내 변경 때문에 담당 밖 파일의 호출부가 깨지면, 고치지 말고 노트의 "합칠 때 할 일"에 `파일:줄 — 이전 → 이후`로 적는다. 합치기 단계에서 한 번에 고친다.
  - 예외: 혼자 하는 세션(S1, S7, T1, T6, T7)은 담당 밖 호출부를 직접 최소로 고친다.
- **공개 이름 동결:** sessions.md에 "동결"로 적힌 클래스는 public 멤버와 static 멤버의 이름·형태를 바꾸지 않는다 (같은 단계의 다른 세션이 쓰고 있다). 내부 정리는 해도 된다.
- **약속 API (2차):** plan-2nd.md 8절의 약속 API는 만드는 세션과 부르는 세션이 다르다. 적힌 이름·매개변수·반환형 그대로 만들고 그대로 부른다. 바꿔야 하면 바꾸지 말고 노트의 "합칠 때 할 일"에 적는다.
- **기록은 내 노트 파일에만:** `notes/세션번호.md`(예: `notes/T3.md`) 하나만 만든다. sessions.md, plan-2nd.md, 다른 세션 노트는 고치지 않는다 (합칠 때 충돌난다).
- **컴파일 확인은 worktree에서 하지 않는다.** worktree에는 `Library` 폴더가 없어 Unity가 프로젝트 전체를 다시 가져온다. 대신 고친 파일을 다시 읽어 문법·이름을 꼼꼼히 확인하고, 합치기 단계에서 컴파일한다.
- **끝나면 내 브랜치에 커밋한다.** 커밋 메시지: `refactor(S3): 장애물·보스·발판 스타일 정리`, `refactor(T3): 보스 패턴 상속·발판 참조 정리`. 푸시와 머지는 하지 않는다.

## 절대 규칙

### 동작 유지
- 게임 동작(수치, 타이밍, 판정, 씬 흐름, 저장 형식)은 바꾸지 않는다. 구조와 표현만 바꾼다.
  - **2차 예외:** plan-2nd.md에 "동작 차이"로 적힌 것만 허용한다(예: R6 저장 위치 변경, R5-8 테스트 값이 남은 상태 필드). 그 밖의 차이가 생기면 멈추고 사용자에게 묻는다.
- 버그로 보이는 동작도 고치지 말고 `// TODO(리팩토링):`로 표시하고 노트에 적는다. (2차 기준 버그 목록은 plan-2nd.md 9절)
- 저장 파일(`SaveList`)의 필드 이름은 바꾸지 않는다. 기존 세이브 JSON이 깨진다.

### 씬·프리팹 (.unity / .prefab)
- 맵과 오브젝트 배치(Transform, 오브젝트 추가·삭제, 머티리얼, 라이트 등)는 손대지 않는다.
- 허용: 스크립트 컴포넌트 붙이기/떼기, 스크립트 필드의 인스펙터 연결 값, 이름이 바뀐 필드의 키 수정.
- 씬 YAML을 고칠 때는 해당 스크립트 GUID가 들어 있는 `MonoBehaviour` 블록 안만 고친다. 다른 세션도 같은 씬의 다른 블록을 고치므로, 블록 밖 줄은 절대 건드리지 않는다 (합칠 때 충돌난다). 고친 뒤 `git diff --stat`로 예상한 파일만 바뀌었는지 본다.
- 컴포넌트를 새로 붙이거나 GUID가 새로 생기는 일(새 스크립트를 씬에 붙이기)은 YAML 직접 편집이 위험하다. 그럴 때는 노트의 "에디터에서 할 일"에 적는다.

### 인스펙터 연결이 끊기지 않게
- **클래스/파일 이름을 바꿀 때:** `.cs`와 `.cs.meta`를 같이 `git mv` 한다. GUID가 유지되어 씬 연결이 살아 있다. 클래스 이름 = 파일 이름.
- **직렬화 필드 이름을 바꿀 때:** inventory.md의 "붙어 있는 곳"(2차는 GUID grep 결과) 모든 파일에서 그 스크립트 블록의 키를 새 이름으로 바꾼다 (`  oldName:` → `  newName:`). 바꾸기 전 값이 들어 있었는지 확인하고, 노트에 바꾸기 전후 값을 적는다. `[FormerlySerializedAs]`는 쓰지 않는다 (style.md 4절).
  - 필드를 `[Serializable]` 클래스 안으로 옮길 때(예: R10 보스 패턴)는 키가 들여쓰기 한 단계 아래로 내려가고, 오버라이드의 `propertyPath`는 `jumpAttack.lampPoints.Array.data[0]`처럼 앞에 필드 이름이 붙는다.
  - **프리팹 오버라이드도 같이 바꾼다.** 씬이나 다른 프리팹에 놓인 프리팹 인스턴스는 `m_Modifications` 안에 `propertyPath: 옛이름`으로 값을 덮어쓴다. inventory.md 목록에 없는 파일에도 있을 수 있으니 `grep -rn "propertyPath: 옛이름" Assets --include=*.unity --include=*.prefab`로 찾는다. 단, 같은 이름을 쓰는 다른 스크립트의 오버라이드일 수 있으니 `target`의 프리팹이 내 스크립트를 가진 것인지 확인한다. (M2에서 이것을 놓쳐 Player의 `cameraArm`이 비고 플레이어가 움직이지 않았다.)
- **public 메서드 이름을 바꿀 때:** 먼저 `grep -rn "m_MethodName: 메서드명" Assets`로 버튼 OnClick 연결을 확인한다. 연결돼 있으면 YAML의 `m_MethodName`도 같이 바꾼다. 애니메이션 이벤트(`.anim`의 `functionName`)와 `Invoke("..")`, `StartCoroutine("..")`, `SendMessage("..")` 문자열도 확인한다.
- 이미 쓰고 있는 OnClick 메서드: SetActive, PadNumber, QuitButton, ImageCreat, GameExit, GameContinue, TitleButton, SFXControl, RightClick, NewStartButton, BGMControl, WrongClick, TitleFadeOut, LoadButton, Title, SaveData, RoomRespawn, Respawn, MainHallButton, Exit, Enter, CountinueButton, Close2, Close

### 리소스
- 새 에셋(이미지, 사운드, 패키지, 데이터 파일)을 만들거나 받지 않는다. 프로젝트 안 파일만 쓴다.
- DOTween은 `Assets/Resources/Plugins/Demigiant`에 이미 있으므로 써도 된다.

### 더 이상 쓰지 않는 스크립트 → trash
- 위치: 프로젝트 루트의 `trash/` (Assets 밖이라 컴파일되지 않아 다른 스크립트 변경에 깨지지 않는다).
- 옮기기 조건: (1) 어떤 `.unity`/`.prefab`/`.asset`에도 GUID가 없고 (2) 다른 `.cs`에서 참조하지 않는다. 둘 다 `grep`으로 확인한다.
- 원래 경로를 유지해 옮긴다: `git mv Assets/02.Scripts/X.cs trash/Assets/02.Scripts/X.cs` (`.meta`도 같이).
- 리팩토링으로 합쳐져 필요 없어진 스크립트도 같은 방식으로 옮긴다. 단, 씬에 붙어 있던 스크립트를 합칠 때는 남는 쪽 GUID로 씬 연결을 옮긴 뒤 옮긴다.
- 담당 파일만 옮긴다. 담당 밖 파일이 안 쓰이는 것 같으면 노트에 적어 S7에 넘긴다.

### 인코딩
- inventory.md에 CP949로 표시된 파일은 `iconv -f cp949 -t utf-8 파일`로 읽는다. 깨진 한국어 주석은 내용을 추측해 다시 쓰고, 저장은 UTF-8로 한다.

## 노트 파일 형식 (`notes/S번호.md`, `notes/T번호.md`)
2차 노트에는 "적용한 R 항목"(예: R10, R2)과 "약속 API를 만들었는지/불렀는지" 줄을 바뀐 공용 API 표 위에 더한다.
```markdown
# S3 노트
상태: 완료 / 브랜치: (git branch --show-current 결과)

## 바뀐 공용 API
| 이전 | 이후 | 메모 |
|---|---|---|
| `Wall.WallMove` (static bool) | (동결이라 유지) | |

## 합칠 때 할 일
- (같은 단계 다른 세션 파일에서 고쳐야 할 호출부 등)

## 에디터에서 할 일
- (사용자가 Unity 에디터에서 붙여야 할 컴포넌트 등)

## trash 후보 / TODO
- 
```

## 작업 순서 (S·T 세션 공통)
1. 작업 위치(worktree / 원래 폴더)를 확인한다.
2. 담당 파일을 읽고, 바꿀 공용 API를 먼저 정한다 (동결 대상은 제외). 2차는 plan-2nd.md의 내 R 항목 코드를 기준으로 하되, 실제 파일과 다른 곳이 있으면 실제 파일에 맞춘다.
3. 파일 하나씩 리팩토링 → 씬 YAML 키 수정 → 고쳐도 되는 범위의 호출부 수정.
4. 원래 폴더에서 하는 세션(S1, S7, T1, T6, T7)만 컴파일을 확인한다.
   - 이 PC에는 `C:\Program Files\Unity\Hub\Editor`가 없어 batchmode 경로를 모른다. 1차 M2에서도 batchmode가 라이선스 오류로 실행되지 않았다.
   - 그래서 **사용자에게 Unity 에디터를 열어 콘솔의 컴파일 에러를 알려 달라고 요청한다.**
5. `notes/세션번호.md`를 쓰고 커밋한다.

## 합치기 (M2, M3, N2) 순서
sessions.md의 해당 합치기 절을 따른다. 원래 폴더의 `main`에서 한다.
1. 원래 폴더에 커밋 안 된 변경이 있는지 `git status`로 확인. 리팩토링과 무관한 변경(예: 이미지 `.meta`)은 건드리지 말고 사용자에게 알린다.
2. 합칠 브랜치를 찾는다. 세션 노트는 아직 각 브랜치 안에만 있으므로 main에서는 보이지 않는다.
   - Git Bash에서는 `브랜치:경로` 형식이 경로로 잘못 바뀌므로 명령 앞에 `MSYS_NO_PATHCONV=1`을 붙인다.
   - `git worktree list`와 `git branch`로 브랜치 목록을 보고, 각 브랜치에서 `git show 브랜치:.claude/skills/jaljayo-refactor/notes/S2.md`로 노트를 읽어 어느 세션 브랜치인지 확인한다.
   - 노트가 없거나 "상태: 완료"가 아니면 그 세션은 아직 안 끝난 것이다. 합치지 말고 사용자에게 알린다.
   - 브랜치를 하나씩 `git merge --no-ff`로 합친다.
3. 충돌이 나면:
   - `.unity`/`.prefab`: 양쪽이 서로 다른 MonoBehaviour 블록을 고친 경우가 대부분이다. 두 변경을 모두 살린다. 판단이 안 되면 멈추고 사용자에게 묻는다.
   - `.cs`: 노트의 "바뀐 공용 API"를 기준으로 맞춘다.
4. 모든 노트의 "합칠 때 할 일"을 처리한다.
5. 컴파일 확인(위 명령). 에러는 노트의 API 표를 기준으로 고친다.
6. (N2) plan-2nd.md 8절의 약속 API가 만드는 쪽과 부르는 쪽에서 같은 이름·형태인지 확인한다.
7. 합치기 결과를 `notes/M번호.md`·`notes/N2.md`에 적고 커밋한다. sessions.md 진행 상태 표도 여기서 갱신한다.
