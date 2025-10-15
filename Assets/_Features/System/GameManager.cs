using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Map Settings")]
    public Rect mapBounds = new Rect(-40f, -25f, 80f, 50f);

    public float spawnOffset = 2f; // Distance outside the map bounds to spawn enemies

    public Rect SpawnBounds { get; private set; }

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
        // 맵 경계를 초록색으로 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.size);

        // 스폰 영역을 빨간색으로 표시
        if (Application.isPlaying) // 에디터에서 실행 중일 때만 표시
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(SpawnBounds.center, SpawnBounds.size);
        }
    }


}