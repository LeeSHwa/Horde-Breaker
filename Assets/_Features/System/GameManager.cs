using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어디서든 GameManager.Instance로 접근할 수 있게 해주는 싱글톤 구현
    public static GameManager Instance { get; private set; }

    [Header("Map Settings")]
    // 이 Rect 변수가 게임의 맵 크기를 결정하는 '원본 데이터'가 됩니다.
    public Rect mapBounds = new Rect(-25f, -15f, 50f, 30f);

    void Awake()
    {
        // --- 싱글톤 패턴: 게임 내에 GameManager가 단 하나만 존재하도록 보장 ---
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // 이미 인스턴스가 있다면 스스로를 파괴
        }
        else
        {
            Instance = this; // 없다면 스스로를 인스턴스로 지정
            DontDestroyOnLoad(this.gameObject); // (선택사항) 씬이 바뀌어도 파괴되지 않음
        }
    }
}