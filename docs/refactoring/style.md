# 코드 스타일 기준

원본: 직접 정리한 `코딩스타일.md` 1~11번. **12번(개선해 볼 점)은 기준에서 뺀다.**
- 12번 항목(public 필드 일부, `is` 분기, 숫자 비교, 나란히 놓은 배열, `GameObject.Find`, 싱글톤 접근 등)은 고칠 목표가 아니다. 원래 코드에 있으면 그대로 둬도 된다.
- 그렇다고 일부러 만들지도 않는다 (철자 실수, Update 안 `Debug.Log` 같은 것을 새로 넣지 않는다).
- **2차 리팩터링에서 추가한 기준:** 4절 인스펙터 보조 속성, 5절 싱글톤·상속·데이터와 표시·참조 방향·게임 흐름·조정 값·매직 문자열, 7절 로그, 8절 판단 기준.

## 1. 형식
- 여는 중괄호는 **다음 줄**. 들여쓰기 공백 4칸.
- 이른 반환·null 검사는 한 줄 if, 나머지는 중괄호.
  ```csharp
  if (target == null) return;
  ```
- 클래스가 길면 `#region 이동`, `#region 피격`처럼 기능별로 묶는다.
- bool 부정은 **`== false`**. `!`는 쓰지 않는다.
- 쓰지 않는 `using`, 주석 처리된 옛 코드는 지운다.

## 2. 이름
| 대상 | 규칙 | 예 |
|---|---|---|
| private 필드 | camelCase, 접두사 없음 (`_`, `m_` 금지) | `moveSpeed`, `jumpCount` |
| public 필드·프로퍼티 | PascalCase | `MaxHp`, `IsPause` |
| bool | `is`/`can`/`has` | `isJump`, `canMove`, `hasStatue` |
| 메서드 | 동사로 시작, PascalCase | `SpawnPlayer`, `ApplyDamage` |
| 이벤트·핸들러 | `On…` / `Handle…` | `OnDamaged` / `HandleDied` |
| enum | PascalCase 단수형 | `StageKind`, `ColorKind` |
| 싱글톤 접근 | `.Instance` | `GameManager.Instance` |

## 3. 쓰는 문법 (이 범위 안에서만)
- **프로퍼티:** `{ get; private set; }`, `[field: SerializeField]`, 식 본문 `=>`
  ```csharp
  [field: SerializeField] public int Hp { get; private set; }
  private bool isDead => (Hp <= 0);
  ```
- **abstract / virtual / override**, `base.Method()` 호출
- **제네릭:** `SingletonBase<T>`, `ComponentPool<T>` 정도
- **`TryGetComponent` / `TryGetValue` / `out`**
- **컬렉션:** `Dictionary`, `HashSet`, `List`, 배열. 밖으로 내보낼 때 `IReadOnlyCollection`
- **대리자·이벤트:** `event Action`, `Action<T>`, `?.Invoke()`. 구독/해제는 `OnEnable`/`OnDisable`에서 짝으로
- **switch 식, 패턴 매칭:** `kind switch { ... }`, `is Player player`
- **지역 함수, 람다:** `AddListener(() => Select(index))`
- **상수:** `const`, `static readonly`, `readonly`
- **문자열 보간** `$""`
- **코루틴**(쿨타임·대기), **DOTween `Sequence`**(연출, `OnDisable`에서 `DOKill`)
- **enum**으로 상태 관리: `IsGameState` / `SetGameState` 형태

## 4. 쓰지 않는 것 (더 높은 수준 금지)
- LINQ, async/await, Task, UniTask
- struct 새로 만들기, record, 튜플 반환
- **interface 새로 선언** (기존에 없으면 만들지 않는다. 다형성은 abstract 클래스로)
- `var` (타입을 적는다)
- 리플렉션, 어트리뷰트 직접 만들기, 커스텀 에디터/빌더 스크립트
- DI, 이벤트 버스, ScriptableObject 이벤트 채널, Addressables, 새 패키지
- 3단 이상 상속, 제네릭 제약을 겹겹이 쓰는 설계
- C# 9 이상 문법 (Unity 2020.3 = C# 8): `new()` 대상 형식 생략, `init`, `record` 금지
- **인스펙터 보조 속성:** `[FormerlySerializedAs]`, `[Tooltip]`, `[Min]`, `[Range]`는 쓰지 않는다. 인스펙터 관련 속성은 `[Header]`, `[SerializeField]`, `[field: SerializeField]`, `[RequireComponent]`만 쓴다.
  - 직렬화 필드 이름을 바꿀 때는 `[FormerlySerializedAs]` 대신 SKILL.md 규칙대로 씬·프리팹 YAML 키와 `propertyPath` 오버라이드를 직접 고친다.
  - 값의 뜻이 헷갈리면 `[Tooltip]` 대신 필드 옆에 한 줄 주석을 단다.

## 5. 구조 기준
- **싱글톤:** **씬을 넘어가며 유지되는 매니저만** `SingletonBase<T>`를 상속한다 (S1에서 만든 것). 처음 부를 때 찾고, 중복은 파괴하고, 종료 중에는 만들지 않는다. 자식은 `OnInitialize()`를 override.
  - 씬을 넘어가며 유지되는 매니저는 `OnEnable`/`OnDisable` 대신 `OnInitialize`/`OnDestroy`에서 이벤트를 구독·해제한다.
- **캡슐화:** 상태는 `private` 또는 `[SerializeField] private`, 밖에서 읽을 값은 `{ get; private set; }`. 값 변경은 메서드로 (`ApplyDamage()`, `AddScore()`). public 필드는 새로 만들지 않는다.
- **상속:** 얕게(1단). **다형성이 필요하거나, 공통 흐름을 부모가 정하고 다른 부분만 자식이 채울 때(템플릿 메서드)만** 쓴다.
  - 예: 보스 공격 패턴. 부모 `BossAttack`이 "애니메이션 → 대기 → 공격 → 쿨타임"을 정하고, 자식은 `Attack()`만 채운다.
  - 부모에 virtual/abstract 멤버가 하나도 없고 코드 재사용만 한다면 상속하지 않는다. 그런 공통 코드는 메서드나 일반 클래스로 뺀다.
  - 자식 클래스가 부모를 통해 다시 소유자의 내부 필드를 꺼내 쓰게 되면 상속을 다시 생각한다. 데이터는 자식이 직접 갖는다.
  - 비슷한 스크립트 두 개(예: `Drag`/`Drag2`)는 abstract 부모 + 자식 또는 인스펙터 값 차이로 합친다.
- **책임:** 한 클래스는 하나의 목적만 가진다. 목적과 상관없는 규칙이 들어 있으면 그 규칙이 속한 클래스로 옮긴다.
  - 예: 버튼 종류별 동작은 Player가 아니라 Button이 정한다 (`button.Activate()`).
- **데이터와 표시 분리:** 진행 데이터(체력, 점수 등)는 표시 컴포넌트(UI Text 등)에 두지 않는다.
  - 데이터는 static 클래스(`GameSession`)나 매니저가 갖고, 값이 바뀔 때 `event Action<T>`로 알린다.
  - 표시 컴포넌트는 `OnEnable`/`OnDisable`에서 구독하고 글자만 바꾼다. `Update`에서 매 프레임 텍스트를 다시 쓰지 않는다.
- **참조 방향:** 참조는 한 방향으로만 둔다.
  - 두 클래스가 서로를 부르면(서로 참조) 한쪽이 값을 **매개변수로 넘기게** 바꾼다. 예: `ChangePoint(int collectedCount)`, `MarkLoadedFromSave(Vector3 position)`, `PlaceStatue(Statue statue)`.
  - 같은 오브젝트를 두 클래스가 인스펙터로 따로 연결하지 않는다. 그 오브젝트를 가진 쪽에 물어본다 (`GameManager.Instance.PlayerPosition`).
  - 트리거·충돌로 들어온 대상은 `other.TryGetComponent`로 받는다. 같은 대상을 `GameObject.Find`로 따로 찾지 않는다.
  - 쓰기만 하고 아무도 읽지 않는 상태와 그 setter는 지운다.
- **게임 흐름은 한 곳에서:** 게임 오버, 새 게임 초기화, 일시정지 해제, 퍼즐 창 열기/닫기(`Time.timeScale`·커서·이동 잠금), 캐릭터 이동처럼 여러 곳에서 같은 일을 하는 흐름은 GameManager의 메서드 하나로 모으고, 각자는 그 메서드만 부른다.
- **컴포넌트 조합:** 거대한 클래스(Player)는 역할별 컴포넌트로 나눌 수 있다. 단 새 컴포넌트를 씬에 붙여야 하면 SKILL.md 규칙대로 사용자에게 요청한다. 붙이기가 어렵다면 한 클래스 안에서 `#region`과 private 메서드로 나눈다.
- **상태:** enum + switch, 또는 간단한 상태 머신.
- **조정 값:** 속도, 쿨타임, 거리, 지연 시간, 시작 체력, 목숨 수 등 **에디터에서 바꿀 수 있어야 하는 값은 `const`로 두지 않는다.** `[Header("이동")]` + `[SerializeField]`로 인스펙터에 묶는다. 기존 인스펙터 값은 유지한다.
  - const를 `[SerializeField]`로 바꿀 때는 같은 값을 필드 초기값으로 준다. 씬에 키가 없으면 Unity가 초기값을 쓰므로 동작이 같다.
  - 같은 클래스 안에서 비슷한 값이 const와 `[SerializeField]`로 섞이지 않게 한다.
- **배열 길이:** 인스펙터 배열과 개수가 같아야 하는 값은 상수로 쓰지 않고 `array.Length`/`list.Count`를 쓴다.
  - 예: `lampPoints.Length`, `papers.Length`, `correctNum.Count`
  - UI 슬라이더의 최솟값처럼 컴포넌트가 이미 가진 값은 `audioSlider.minValue`처럼 그 값을 읽는다.
- **값을 베끼지 않는다:** 다른 클래스의 인스펙터 값을 const로 다시 적지 않는다. 그 값을 가진 클래스가 프로퍼티로 내보낸다 (`Boss.MaxHp`, `GameManager.HasAllPapers`).
- **상태는 직렬화하지 않는다:** 실행 중에 바뀌는 상태(진행 수, 남은 시간 등)는 `[SerializeField]`로 두지 않는다. 씬에 테스트 값이 남아 시작값이 달라진다.
- **const로 두는 것:** 바뀌지 않는 약속 값(OnClick 정수 인자, 저장 배열 길이, 오디오 믹서 최솟값), 애니메이터 해시, 태그·씬 이름.
- **데이터:** 저장 데이터는 `[Serializable] class`, 정의 데이터는 `[Serializable]` 클래스 배열 또는 기존 ScriptableObject. UI 오브젝트(`Text` 등)를 데이터 저장소로 쓰지 않는다.
- **매직 문자열:** 문자열은 `const string`으로 둔다 (값은 그대로). 공용 static 클래스는 `Assets/02.Scripts/Core/`에 둔다.
  - **프로젝트 전체에 등록되는 이름은 한 파일만 써도 공용 클래스에 모은다.** 태그 → `Tags`, 씬 이름(빌드 밖 씬 포함, 주석으로 구분) → `SceneNames`, Input Manager 축·버튼 → `InputNames`.
  - **씬 오브젝트 이름**(`GameObject.Find`, `name ==` 비교)은 두 파일 이상이 쓰면 `ObjectNames`, 한 파일만 쓰면 그 클래스 위에 둔다.
  - **애니메이터 해시**는 그 Animator를 쓰는 클래스에 `static readonly int`로 둔다.
  - 상수 모음은 `struct`가 아니라 `static class`로 만든다 (인스턴스 생성을 컴파일러가 막음. 근거는 `상수_모음_static_class_선택_이유.md`).

## 6. 최적화 습관
- 거리 비교는 `sqrMagnitude <= range * range`
- `GetComponent` 결과, 애니메이터 파라미터는 캐싱 (`static readonly int RunHash = Animator.StringToHash("IsRun");`)
- `Awake`/`Start`에서 참조 캐싱, `[RequireComponent]`로 필요한 컴포넌트 보장
- 자주 생성·파괴하는 오브젝트(총알 등)는 풀링을 고려하되, 프리팹·씬 구조를 바꿔야 하면 하지 않는다.
- 시작한 일은 끝에서 되돌린다: 이벤트 해제, 트윈 정리, `Time.timeScale`·커서 상태 복원

## 7. 주석과 로그
- 주석은 한국어로 "왜/무엇을". 클래스 위에 역할 한 줄, 긴 메서드 안에는 `// 1. …`, `// 2. …` 단계 번호.
- public 메서드나 헷갈리는 멤버에는 `/// <summary>` 사용 가능.
- 경고 로그: `Debug.LogWarning($"[{nameof(클래스명)}] 상황 설명", this);`
  - 클래스 이름은 문자열 대신 `nameof`로 쓴다. 이름이 바뀌어도 따라간다.
  - MonoBehaviour 안에서는 두 번째 인자로 `this`를 넘긴다. 콘솔에서 로그를 누르면 그 오브젝트가 선택된다. static 문맥(`SingletonBase`의 `typeof(T).Name` 등)은 그대로 둔다.
- 디버깅용 `Debug.Log`는 남기지 않는다.

## 8. 리팩터링 판단 기준
리팩터링안을 만들거나 검토할 때 항목마다 아래를 확인한다. 하나라도 걸리면 고치고, 고칠 이점이 불확실하면 보류한다.

| # | 확인할 것 | 걸리면 |
|---|---|---|
| 1 | 한 클래스가 목적에 맞지 않게 너무 많은 기능을 갖고 있는가 | 규칙을 그 규칙이 속한 클래스로 옮긴다 (5절 "책임") |
| 2 | 참조가 기능에 맞는 클래스에서 일어나는가 | 부르는 쪽이 대상의 규칙을 알고 있으면 대상에 메서드를 만들고 그것만 부른다 |
| 3 | 상속이 정말 필요한 곳에서만 쓰이는가 | 다형성·템플릿 메서드가 아니면 상속을 빼거나 늘리지 않는다 (5절 "상속") |
| 4 | 원본과 비교했을 때 바꾼 이유를 한 줄로 설명할 수 있는가 | 이유가 약하면 바꾸지 않는다 |
| 5 | 리팩터링했을 때 이점이 확실한가 | 성능 이점은 실제 규모로 따진다(예: 보스가 한 번에 6개 만드는 정도면 풀링 이점이 불확실). 곧 다른 수정으로 없어질 코드는 다듬지 않는다. 불확실하면 보류하고 이유를 적는다 |
| 6 | 에디터나 코드에서 바뀔 수 있는 값이 상수로 박혀 있는가 | 5절 "조정 값", "배열 길이", "값을 베끼지 않는다", "상태는 직렬화하지 않는다" |
| 7 | 구조적으로 나뉘어야 할 기능이 한 곳에 몰려 있는가 | 데이터와 표시, 상태와 흐름을 나눈다 (5절 "데이터와 표시 분리", "게임 흐름은 한 곳에서") |
| 8 | 서로 참조나 불필요한 참조가 있는가 | 값을 매개변수로 넘기거나, 가진 쪽에 묻거나, 읽지 않는 상태를 지운다 (5절 "참조 방향") |

- 리팩터링안이 **새로** 서로 참조를 만들거나 책임을 늘리지 않았는지도 같은 기준으로 다시 본다.
- 동작이 달라지는 곳은 항목마다 "동작 차이"로 적는다. 버그로 보이는 동작은 고치지 않고 `// TODO(리팩토링):`로 남긴다 (SKILL.md "동작 유지").
- 씬·프리팹에 남은 값(배열 크기, 오버라이드, 어느 오브젝트가 찾아지는지)은 추측하지 않고 YAML에서 확인한다.

## 짧은 예시
스타일을 보여 주는 예시일 뿐입니다. 실제 설계는 각 세션이 정하고 노트에 적습니다.
```csharp
using System;

// 한 판 동안 유지되는 플레이어 체력. 값은 메서드로만 바꾸고, 바뀔 때마다 알린다. 표시는 Health 컴포넌트가 구독해서 한다.
public static class GameSession
{
    public static int Hp { get; private set; }
    public static bool IsDead => (Hp <= 0);

    public static event Action<int> OnHpChanged;

    public static void ApplyDamage(int damage)
    {
        SetHp(Hp - damage);
    }

    public static void SetHp(int hp)
    {
        Hp = hp;
        OnHpChanged?.Invoke(Hp);
    }
}
```
```csharp
using UnityEngine;
using UnityEngine.UI;

// 체력 표시. 값은 GameSession이 가지고, 여기서는 바뀔 때 글자만 바꾼다.
[RequireComponent(typeof(Text))]
public class Health : MonoBehaviour
{
    private Text hpText;

    private void Awake()
    {
        hpText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        GameSession.OnHpChanged += HandleHpChanged;
        HandleHpChanged(GameSession.Hp);
    }

    private void OnDisable()
    {
        GameSession.OnHpChanged -= HandleHpChanged;
    }

    private void HandleHpChanged(int hp)
    {
        hpText.text = hp.ToString();
    }
}
```
