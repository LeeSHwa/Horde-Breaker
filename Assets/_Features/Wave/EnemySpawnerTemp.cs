using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerTemp : MonoBehaviour
{
    [Header("Settings")]
    public List<WaveData> waves;    // List of WaveData assets to use
    public Transform enemyContainer; // Parent transform to organize spawned enemies
    public float spawnPadding = 2f;  // Padding distance outside the camera view

    // Internal variables
    private WaveData currentWave;
    private int currentWaveIndex = 0;
    private List<float> ruleTimers; // Timers for each spawn rule
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        ruleTimers = new List<float>();

        // 1. Sort waves by start time to prevent ordering errors
        waves.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        // 2. Initialize the first wave
        if (waves.Count > 0)
        {
            SetWave(waves[0]);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float currentTime = GameManager.Instance.gameTime;

        // Check if it's time to transition to the next wave
        CheckNextWave(currentTime);

        // Handle spawning based on current wave rules
        HandleSpawning();
    }

    // --- [Logic 1] Wave Management ---
    private void CheckNextWave(float time)
    {
        // Return if there are no more waves
        if (currentWaveIndex >= waves.Count - 1) return;

        WaveData nextWave = waves[currentWaveIndex + 1];

        // Transition to the next wave if the time has come
        if (time >= nextWave.startTime)
        {
            currentWaveIndex++;
            SetWave(nextWave);
        }
    }

    private void SetWave(WaveData newWave)
    {
        currentWave = newWave;

        // Prepare timers for the new wave's rules (all start at 0)
        ruleTimers.Clear();
        if (currentWave.spawnRules != null)
        {
            foreach (var rule in currentWave.spawnRules)
            {
                ruleTimers.Add(0f);
            }
        }
        Debug.Log($"[Wave Changed] {currentWave.waveName} started at {currentWave.startTime}s");
    }

    // --- [Logic 2] Multi-Rule Spawning ---
    private void HandleSpawning()
    {
        if (currentWave == null || currentWave.spawnRules == null) return;

        // Iterate through all rules in the current wave simultaneously
        for (int i = 0; i < currentWave.spawnRules.Count; i++)
        {
            SpawnRule rule = currentWave.spawnRules[i];

            // Skip if interval is 0 or less (e.g., one-time Boss spawn)
            if (rule.interval <= 0) continue;

            ruleTimers[i] += Time.deltaTime;

            if (ruleTimers[i] >= rule.interval)
            {
                // Trigger spawn
                if (Random.value <= rule.chance) // Check spawn probability
                {
                    SpawnEnemy(rule.prefab);
                }
                ruleTimers[i] = 0f; // Reset timer for this rule
            }
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Vector2 spawnPos = GetRandomPositionOutsideCamera();
        Instantiate(prefab, spawnPos, Quaternion.identity, enemyContainer);
    }

    // --- [Logic 3] Calculate Position Outside Camera ---
    private Vector2 GetRandomPositionOutsideCamera()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector2 camPos = mainCamera.transform.position;

        int side = Random.Range(0, 4); // 0: Top, 1: Bottom, 2: Left, 3: Right
        float x = 0, y = 0;

        switch (side)
        {
            case 0: // Top
                x = Random.Range(camPos.x - camWidth, camPos.x + camWidth);
                y = camPos.y + camHeight + spawnPadding;
                break;
            case 1: // Bottom
                x = Random.Range(camPos.x - camWidth, camPos.x + camWidth);
                y = camPos.y - camHeight - spawnPadding;
                break;
            case 2: // Left
                x = camPos.x - camWidth - spawnPadding;
                y = Random.Range(camPos.y - camHeight, camPos.y + camHeight);
                break;
            case 3: // Right
                x = camPos.x + camWidth + spawnPadding;
                y = Random.Range(camPos.y - camHeight, camPos.y + camHeight);
                break;
        }
        return new Vector2(x, y);
    }
}