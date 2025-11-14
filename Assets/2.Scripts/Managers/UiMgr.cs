using UnityEngine;

public class UiMgr : MonoBehaviour
{
    public static UiMgr Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("UiMgr의 인스턴스가 이미 존재합니다. 기존 인스턴스를 제거합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬 전환 시에도 유지되도록 설정
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // 인스턴스가 파괴될 때 null로 설정
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 공통 팝업
    #region 
    #endregion
}
