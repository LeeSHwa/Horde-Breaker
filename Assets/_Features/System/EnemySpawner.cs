using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;   // The enemy prefab to be spawned.
    public float spawnInterval = 1f; // The time interval in seconds between each spawn.
    public Transform enemyContainer;

    private Rect spawnBounds;

    void Start()
    {
        spawnBounds = GameManager.Instance.SpawnBounds;
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyContainer);

        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Randomly pick a side of the rectangle (0=top, 1=bottom, 2=right, 3=left).
        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;

        switch (side)
        {
            case 0: // Top side
                spawnPosition = new Vector3(Random.Range(spawnBounds.xMin, spawnBounds.xMax), spawnBounds.yMax, 0f);
                break;
            case 1: // Bottom side
                spawnPosition = new Vector3(Random.Range(spawnBounds.xMin, spawnBounds.xMax), spawnBounds.yMin, 0f);
                break;
            case 2: // Right side
                spawnPosition = new Vector3(spawnBounds.xMax, Random.Range(spawnBounds.yMin, spawnBounds.yMax), 0f);
                break;
            case 3: // Left side
                spawnPosition = new Vector3(spawnBounds.xMin, Random.Range(spawnBounds.yMin, spawnBounds.yMax), 0f);
                break;
        }

        return spawnPosition;
    }

}