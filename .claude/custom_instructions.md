# Unity 프로젝트 커스텀 지시사항

## 프로젝트 개요
- Unity 기반 게임 프로젝트
- Backend 서버 연동 (뒤끝 서버 사용)
- 언어: C# (.NET Framework 4.x)

## 코드 작성 시 지켜야 할 규칙

### 1. 명명 규칙
- 클래스/메서드: PascalCase
- 변수: camelCase (private 필드), PascalCase (public 필드/프로퍼티)
- **중요**: public 메서드는 반드시 PascalCase 사용 (예: `AddGem()`, `LoadData()`)

#### 메서드 매개변수 규칙
- **무조건 소문자로만 작성**
- 예: `void AddItem(int itemid, string itemname)`

#### 변수명 헝가리안 표기법
변수명은 자료형의 앞글자를 접두사로 붙입니다.

**기본 규칙:**
- 자료형이 한 단어: 첫 글자만 소문자로 (예: `int` → `i`)
- 자료형이 두 단어 이상: 각 단어의 첫 글자를 소문자로 (예: `GameObject` → `go`)

**예시:**
```csharp
// 기본 자료형
int inum = 10;
float fvalue = 3.14f;
bool bflag = true;
double drate = 0.5;
long lcount = 1000L;

// Unity/복합 자료형
GameObject goCube;
BigInteger biNumber;
Transform tfPlayer;
RectTransform rtPanel;
Rigidbody rbPlayer;

// 예외 자료형 (특별 규칙)
string strName;          // str (string의 약어)
Image imgCircle;         // img (Image의 약어)
Sprite sprPicture;       // spr (Sprite의 약어)
Text txtLabel;           // txt (Text의 약어)
TextMeshProUGUI tmpLabel; // tmp (TextMeshPro의 약어)
Button btnStart;         // btn (Button의 약어)
```

**추가 예외 자료형:**
- `List<T>` → `list` (예: `listItems`)
- `Dictionary<K,V>` → `dict` (예: `dictData`)
- `Array` → `arr` (예: `arrNumbers`)
- `Coroutine` → `co` (예: `coLoading`)

### 2. Unity 패턴
- MonoBehaviour 클래스에서 사용하지 않는 생명주기 메서드(Awake, Start, Update 등)는 작성하지 않음
- 싱글톤 패턴 사용 시 프로젝트의 기존 패턴 따름:
  ```csharp
  private static ClassName instance;
  public static ClassName Instance { get { ... } }
  ```

### 3. 비동기 처리
- 코루틴 활용: `IEnumerator` 반환
- Backend 호출 시 초기화 확인: `yield return new WaitUntil(() => IsInitialized)`
- 콜백 패턴 사용

### 4. 디버깅
- 의미 있는 Debug 메시지 사용
- 성공/실패 명확히 구분 (`Debug.Log` vs `Debug.LogError`)
- 한글 메시지 허용

### 5. 에러 처리
- null 체크 철저히 수행
- 초기화 여부 확인
- 실패 시 명확한 에러 메시지와 함께 early return

### 6. 코드 스타일
- 들여쓰기: 4 스페이스
- 중괄호: 새 줄에 작성 (Allman 스타일)
- 주석: 한글 사용 가능, 중요 로직에는 설명 추가

### 7. 데이터 구조
- DTO 클래스는 `DataFormat` 네임스페이스 사용
- 프로퍼티 활용: `{ get; private set; }` 패턴

## 코드 생성 시 자동으로 적용할 사항

1. **항상 CODING_GUIDELINES.md 참조**: 새 코드 작성 또는 수정 시 가이드라인 준수
2. **기존 코드 스타일 유지**: 프로젝트의 기존 패턴과 일관성 유지
3. **에러 처리 포함**: 모든 외부 API 호출에 에러 처리 추가
4. **주석 작성**: 복잡한 로직에는 설명 주석 필수
5. **Unity 최적화**: 불필요한 메서드, GC 유발 코드 최소화

## 코드 리뷰 체크리스트

코드 작성 후 자동으로 다음을 확인:
- [ ] 명명 규칙 준수 (특히 public 메서드 PascalCase)
- [ ] 메서드 매개변수는 모두 소문자
- [ ] 변수명에 자료형 접두사 적용 (헝가리안 표기법)
- [ ] null 체크 및 에러 처리
- [ ] Debug 로그 메시지 포함
- [ ] 사용하지 않는 using 문 제거
- [ ] 빈 메서드 제거
- [ ] 주석이 필요한 복잡한 로직에 설명 추가

## 금지 사항
- ❌ camelCase로 public 메서드 작성 (예: `addGem` → `AddGem`)
- ❌ 매개변수명에 대문자 사용 (예: `int ItemId` → `int itemid`)
- ❌ 변수명에 자료형 접두사 미적용 (예: `count` → `icount`)
- ❌ 빈 Update() 메서드 남겨두기
- ❌ 에러 처리 없이 Backend API 호출
- ❌ 초기화 확인 없이 싱글톤 사용
- ❌ 매직 넘버 직접 사용 (상수로 정의)

## 참고 문서
- 상세한 코딩 규칙: [CODING_GUIDELINES.md](./CODING_GUIDELINES.md)
