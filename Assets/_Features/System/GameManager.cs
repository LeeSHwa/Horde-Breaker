using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Map Settings")]
    public Rect mapBounds = new Rect(-40f, -25f, 80f, 50f);
    public float spawnOffset = 2f; // Distance outside the map bounds to spawn enemies
    public Rect SpawnBounds { get; private set; }

    [Header("Game State")]
    public float gameTime = 0f;
    public int currentStage = 1;


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
        }
        else
        {
            Instance = this; 
            DontDestroyOnLoad(this.gameObject); 

            CalculateSpawnBounds();
        }
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateStageUI(currentStage);
        }
    }


    void Update()
    {
        gameTime += Time.deltaTime;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimeUI(gameTime);
        }
    }

    private void CalculateSpawnBounds()
    {
        SpawnBounds = new Rect(
            mapBounds.xMin - spawnOffset,
            mapBounds.yMin - spawnOffset,
            mapBounds.width + spawnOffset * 2,
            mapBounds.height + spawnOffset * 2
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.size);

        if (Application.isPlaying) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(SpawnBounds.center, SpawnBounds.size);
        }
    }


}