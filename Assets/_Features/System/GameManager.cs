using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public float gameTime = 0f; // Current game time in seconds
    public int currentStage = 1;

    [Header("Persistent Data")]
    public GameObject selectedWeaponPrefab; // Weapon selected from the lobby

    [Header("Map Settings")]
    public Rect mapBounds = new Rect(-40f, -25f, 80f, 50f);

    void Awake()
    {
        // Singleton Pattern Implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Update()
    {
        // Increment game time
        gameTime += Time.deltaTime;

         if (UIManager.Instance != null) UIManager.Instance.UpdateTimeUI(gameTime);
    }
}