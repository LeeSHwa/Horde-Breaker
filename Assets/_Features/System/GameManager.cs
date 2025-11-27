using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public float gameTime = 0f; // Current game time in seconds
    public int currentStage = 1;
    public int killCount = 0;

    [Header("Persistent Data")]
    public GameObject selectedWeaponPrefab; // Weapon selected from the lobby

    [Header("Map Settings")]
    //public Rect mapBounds = new Rect(-40f, -25f, 80f, 50f);
    public Renderer mapRenderer;

    [HideInInspector]
    public bool isGameOver = false; // Flag to prevent multiple Game Over calls

    public Rect mapBounds;

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

    private void Start()
    {
        if (mapRenderer != null)
        {
            Bounds bounds = mapRenderer.bounds;

            mapBounds = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);

            Debug.Log($"Map size set : {mapBounds}");
        }
        else
        {
            mapBounds = new Rect(-40f, -25f, 80f, 50f);
        }
    }

    void Update()
    {
        // Increment game time
        gameTime += Time.deltaTime;

         if (UIManager.Instance != null) UIManager.Instance.UpdateTimeUI(gameTime);
    }

    public void AddKillCount()
    {
        killCount++;
    }

    public void GameOver()
    {
        if (isGameOver) return; // Stop if already game over
        isGameOver = true;

        Debug.Log("Game Over!");

        // 1. Pause the game
        Time.timeScale = 0f;

        // 2. Call UIManager to show the result
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(gameTime, killCount);
        }
    }

}