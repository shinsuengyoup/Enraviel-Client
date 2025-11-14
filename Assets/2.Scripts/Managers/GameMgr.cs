using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance { get; private set; }

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

    public void LoadScene(string scenename)
    {
        SceneManager.LoadSceneAsync(scenename);
    }
}
