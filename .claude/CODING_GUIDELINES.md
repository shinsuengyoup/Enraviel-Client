# Unity 프로젝트 코딩 규칙

## 1. 명명 규칙 (Naming Conventions)

### 클래스 및 구조체
- **PascalCase** 사용
- 예: `GameManager`, `BackendManager`, `UserData`

### 메서드
- **PascalCase** 사용 (public 메서드)
- **camelCase** 사용 (private 메서드 - 프로젝트에서 일부 혼용됨)
- 예: `TryLogin()`, `InitializeBackend()`, `addGem()` → `AddGem()`로 통일 권장

### 메서드 매개변수
- **무조건 소문자로만 작성**
- 헝가리안 표기법 미적용 (매개변수는 순수 소문자)
- 예:
  ```csharp
  void AddItem(int itemid, string itemname, float itemvalue)
  void SetPosition(float xpos, float ypos, float zpos)
  void CreateObject(string objectname, int objectcount)
  ```

### 변수
- **헝가리안 표기법 필수 적용**
- 자료형의 앞글자를 접두사로 사용
- **지역 변수**: 헝가리안 표기법 + camelCase (예: `int icount`, `float fvalue`)
- **private 필드**: 헝가리안 표기법 + camelCase (예: `private int imaxHealth`)
- **public 필드/속성**: PascalCase (헝가리안 표기법 미적용)

#### 헝가리안 표기법 규칙

**기본 자료형:**
```csharp
int inum = 10;
float fvalue = 3.14f;
bool bflag = true;
double drate = 0.5;
long lcount = 1000L;
```

**Unity 자료형:**
```csharp
GameObject goCube;              // GameObject → go
Transform tfPlayer;             // Transform → tf
RectTransform rtPanel;          // RectTransform → rt
Rigidbody rbPlayer;             // Rigidbody → rb
BigInteger biNumber;            // BigInteger → bi
```

**예외 자료형 (특별 약어):**
```csharp
string strName;                 // string → str
Image imgCircle;                // Image → img
Sprite sprPicture;              // Sprite → spr
Text txtLabel;                  // Text → txt
TextMeshProUGUI tmpLabel;       // TextMeshProUGUI → tmp
Button btnStart;                // Button → btn
List<T> listItems;              // List → list
Dictionary<K,V> dictData;       // Dictionary → dict
Array arrNumbers;               // Array → arr
Coroutine coLoading;            // Coroutine → co
```

### 상수
- **PascalCase** 또는 **UPPER_SNAKE_CASE** 사용
- 예: `MaxHealth` 또는 `MAX_HEALTH`

### 네임스페이스
- **PascalCase** 사용
- 예: `DataFormat`, `Backend`

## 2. 코드 구조

### 파일 구조
```csharp
// 1. using 문
using System;
using UnityEngine;

// 2. 네임스페이스 (선택)
namespace ProjectName
{
    // 3. 클래스 정의
    public class ClassName : MonoBehaviour
    {
        // 4. 필드 (public → private 순)
        public static string StaticField;         // public은 헝가리안 미적용
        public int PublicField;                   // public은 헝가리안 미적용
        private int iprivateField;                // private는 헝가리안 적용
        private GameObject goTarget;              // private는 헝가리안 적용

        // 5. 프로퍼티
        public bool IsInitialized { get; private set; }  // 프로퍼티는 헝가리안 미적용

        // 6. Unity 생명주기 메서드
        private void Awake() { }
        private void Start() { }
        private void Update() { }

        // 7. Public 메서드 (매개변수는 소문자)
        public void PublicMethod(int paramvalue, string paramname) { }

        // 8. Private 메서드 (매개변수는 소문자)
        private void PrivateMethod(float xpos, float ypos)
        {
            // 지역 변수는 헝가리안 적용
            int icount = 10;
            string strMessage = "Hello";
        }
    }
}
```

### 싱글톤 패턴
```csharp
private static BackendManager instance;
public static BackendManager Instance
{
    get
    {
        if (instance == null)
            instance = FindObjectOfType<BackendManager>();
        return instance;
    }
}

private void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
```

## 3. 코드 스타일

### 들여쓰기
- **4 스페이스** 사용 (탭 대신)
- 중괄호는 새 줄에 작성 (Allman 스타일)

```csharp
// 좋은 예
if (condition)
{
    DoSomething();
}

// 나쁜 예
if (condition) {
    DoSomething();
}
```

### 주석
- 한글 주석 허용
- 중요한 로직에는 설명 주석 추가
- 한 줄 주석: `// 주석 내용`
- 여러 줄 주석: `/* ... */` 또는 XML 문서 주석

```csharp
/// <summary>
/// 백엔드를 초기화합니다.
/// </summary>
private void InitializeBackend()
{
    // Backend 초기화 대기
    var bro = Backend.Initialize();
}
```

### 공백
- 연산자 양쪽에 공백 추가: `x = y + z`
- 쉼표 뒤에 공백 추가: `Method(a, b, c)`
- 괄호 안쪽에 공백 없음: `if (condition)`

## 4. Unity 특화 규칙

### MonoBehaviour 생명주기
- 사용하지 않는 Unity 메서드는 삭제
- 예: 빈 `Update()` 메서드는 제거

### 코루틴 사용
```csharp
private IEnumerator LoadPlayData()
{
    bool isLoaded = false;

    // 비동기 작업
    yield return new WaitUntil(() => isLoaded);

    // 다음 작업
}
```

### 디버그 로그
- 의미 있는 메시지 작성
- 성공/실패 구분
```csharp
Debug.Log("Backend 초기화 성공!");
Debug.LogError("Backend 초기화 실패!");
Debug.LogWarning("경고 메시지");
```

## 5. 데이터 구조

### 클래스 설계
- DTO (Data Transfer Object)는 별도 네임스페이스로 분리
- 예: `DataFormat` 네임스페이스

```csharp
namespace DataFormat
{
    public class UserData
    {
        public int UserId;
        public string UserCode;
        public string UserName;

        public UserData() { }

        public UserData(int userId, string userCode)
        {
            UserId = userId;
            UserCode = userCode;
        }
    }
}
```

### 프로퍼티 vs 필드
- 외부 접근이 필요한 경우: **프로퍼티** 사용
- 내부에서만 사용: **private 필드** 사용
- 읽기 전용: `{ get; private set; }`

## 6. 에러 처리

### null 체크
```csharp
if (DataHandler.userData == null)
{
    Debug.LogError("유저 데이터 로드 실패");
    yield break;
}
```

### 초기화 확인
```csharp
if (!IsInitialized)
{
    Debug.LogError("Backend가 초기화되지 않았습니다!");
    return false;
}
```

## 7. 비동기 처리

### 콜백 패턴
```csharp
DataHandler.LoadUserDataV2((data) => {
    // 데이터 로드 완료 후 처리
    isLoaded = true;
});

yield return new WaitUntil(() => isLoaded);
```

## 8. 권장 사항

### DO ✅
- 명확하고 의미 있는 변수명 사용
- 한 메서드는 한 가지 일만 수행
- 매직 넘버 대신 상수 사용
- 일관된 명명 규칙 유지
- 주요 로직에 주석 추가

### DON'T ❌
- 전역 변수 남용하지 않기
- 긴 메서드 작성하지 않기 (50줄 이하 권장)
- 중복 코드 작성하지 않기
- 사용하지 않는 using 문 포함하지 않기
- 빈 Unity 생명주기 메서드 남기지 않기

## 9. Git 커밋 메시지

### 형식
```
[타입] 간단한 설명

자세한 설명 (선택)
```

### 타입
- `[ADD]` 새로운 기능 추가
- `[FIX]` 버그 수정
- `[UPDATE]` 기능 개선/변경
- `[REFACTOR]` 코드 리팩토링
- `[DOCS]` 문서 수정
- `[TEST]` 테스트 코드

### 예시
```
[ADD] 백엔드 로그인 기능 구현

- BackendLogin 클래스 추가
- 자동 로그인 로직 구현
```

## 10. 폴더 구조

```
Assets/
├── Scripts/
│   ├── Backend/        # 백엔드 관련
│   ├── Managers/       # 매니저 클래스들
│   ├── Data/          # 데이터 클래스들
│   └── Utils/         # 유틸리티
├── Prefabs/
├── Scenes/
└── Resources/
```

---

이 문서는 프로젝트의 코드 품질과 일관성을 유지하기 위한 가이드라인입니다.
모든 팀원은 이 규칙을 따라 코드를 작성해야 합니다.
