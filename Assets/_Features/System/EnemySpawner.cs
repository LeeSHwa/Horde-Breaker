using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;   // The enemy prefab to be spawned.
    public float spawnInterval = 1f; // The time interval in seconds between each spawn.

    [Header("Map Boundaries")]
    // Defines the map boundaries (x, y, width, height) where enemies will spawn.
    public Rect mapBounds = new Rect(-20f, -10f, 40f, 20f);

    [Header("Hierarchy & References")]
    // A parent object for the spawned enemies to keep the Hierarchy clean.
    public Transform enemyContainer;

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Kicks off the spawning routine as soon as the game starts.
        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// <summary>
    /// A coroutine that continuously spawns enemies in an infinite loop.
    /// A coroutine can pause its execution, which is perfect for timed actions.
    /// </summary>
    private IEnumerator SpawnEnemiesRoutine()
    {
        // An infinite loop to keep the spawning process running.
        while (true)
        {
            // Pause the execution of this routine for 'spawnInterval' seconds.
            yield return new WaitForSeconds(spawnInterval);

            // Calculate a random position on the edge of the map.
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // Create a new enemy instance from the prefab at the calculated position, with no rotation,
            // and as a child of the enemyContainer transform.
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyContainer);

        }
    }

    /// <summary>
    /// Calculates a random position along the perimeter of the mapBounds Rect.
    /// </summary>
    /// <returns>A Vector3 position for the new enemy.</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        // Randomly pick a side of the rectangle (0=top, 1=bottom, 2=right, 3=left).
        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;

        switch (side)
        {
            case 0: // Top side
                spawnPosition = new Vector3(Random.Range(mapBounds.xMin, mapBounds.xMax), mapBounds.yMax, 0f);
                break;
            case 1: // Bottom side
                spawnPosition = new Vector3(Random.Range(mapBounds.xMin, mapBounds.xMax), mapBounds.yMin, 0f);
                break;
            case 2: // Right side
                spawnPosition = new Vector3(mapBounds.xMax, Random.Range(mapBounds.yMin, mapBounds.yMax), 0f);
                break;
            case 3: // Left side
                spawnPosition = new Vector3(mapBounds.xMin, Random.Range(mapBounds.yMin, mapBounds.yMax), 0f);
                break;
        }

        return spawnPosition;
    }

    /// <summary>
    /// This is a special Unity function that is called in the editor to draw visual aids.
    /// It helps visualize the spawn area without needing a visible object.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Draw a red wireframe box that matches the mapBounds rectangle.
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.size);
    }
}