# 상수 모음에 `struct`가 아니라 `static class`를 쓴 이유

`Tags`, `SceneNames`, `InputNames`, `ObjectNames`(`Assets/02.Scripts/Core/`)는 상수만 담는 타입입니다. "struct가 class보다 가벼우니 struct로 만드는 게 낫지 않나?"라는 질문에 대한 정리입니다.

---

## 1. 결론

상수만 담는 타입은 **struct와 static class 사이에 성능 차이가 없습니다.** 그래서 성능이 아니라 **의도를 컴파일러가 지켜 주는지**를 기준으로 `static class`를 골랐습니다.

---

## 2. 성능 차이가 없는 이유

### 2.1. `const`는 실행 중에 존재하지 않음

- 컴파일러는 `Tags.Player`가 쓰인 자리에 `"Player"` 문자열을 그대로 넣습니다.
- 실행 중에는 `Tags`라는 타입을 만들지도, 읽지도 않습니다. struct로 만들든 class로 만들든 실행되는 코드가 같습니다.

```csharp
// 작성한 코드
if (other.CompareTag(Tags.Player)) { }

// 컴파일된 결과 (의미상)
if (other.CompareTag("Player")) { }
```

### 2.2. "struct가 가볍다"는 인스턴스를 만들 때의 이야기

- struct 값은 힙에 할당되지 않고, class 객체는 힙에 할당되어 GC 대상이 됩니다. 이 차이는 **인스턴스를 만들 때만** 생깁니다.
- static class는 인스턴스를 아예 만들 수 없으므로 할당할 것 자체가 없습니다.

### 2.3. `static readonly` 필드도 같음

- 애니메이터 해시처럼 `const`로 둘 수 없는 `static readonly` 값은 타입당 한 번만 저장됩니다.
- struct의 static 필드든 static class의 static 필드든 저장 위치와 횟수가 같습니다.

---

## 3. `static class`를 고른 이유

| | `static class` | `struct` |
|---|---|---|
| `new Tags()` | 컴파일 오류 | 됨 (쓸모없는 인스턴스가 생김) |
| 인스턴스 필드·메서드 추가 | 컴파일 오류 | 됨 |
| 상속 | 불가 | 불가 |
| 실행 비용 | 없음 | 없음 |

- **의도를 문법이 강제합니다.** static class는 "상수만 모아 둔 곳"이라는 뜻을 컴파일러가 지켜 줍니다. struct는 같은 일을 하면서 인스턴스를 만들거나 인스턴스 멤버를 넣을 여지를 남깁니다.
- **C#의 관례입니다.** 상수·유틸리티 모음은 static class로 만듭니다 (`System.Math`, `System.IO.Path`).
- **Unity의 `Mathf`는 struct입니다.** 오래전에 정한 설계이고, struct라서 빠른 것은 아닙니다. 위 2절과 같은 이유로 `Mathf.PI` 같은 상수 사용은 struct 여부와 상관없이 비용이 같습니다.

---

## 4. 적용 위치

- `Assets/02.Scripts/Core/Tags.cs`, `SceneNames.cs`, `InputNames.cs`, `ObjectNames.cs`
- 규칙: `.claude/skills/jaljayo-refactor/style.md` 5절 "매직 문자열"
