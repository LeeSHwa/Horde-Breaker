using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerTemp : MonoBehaviour
{
    public static EnemySpawnerTemp Instance { get; private set; }

    [Header("Settings")]
    public List<WaveData> waves;    // List of WaveData assets to use
    public Transform enemyContainer; // Parent transform to organize spawned enemies
    public float spawnPadding = 2f;  // Padding distance outside the camera view

    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();

    // Internal variables
    private WaveData currentWave;
    private int currentWaveIndex = 0;
    private List<float> ruleTimers; // Timers for each spawn rule
    private Camera mainCamera;

    private bool warningTriggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
        ruleTimers = new List<float>();

        waves.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        InitializeEnemyPools();
    }

    void Start()
    {
        if (waves.Count > 0)
        {
            SetWave(waves[0]);
        }
    }

    private void InitializeEnemyPools()
    {
        foreach (var wave in waves)
        {
            if (wave.spawnRules == null) continue;

            foreach (var rule in wave.spawnRules)
            {
                if (rule.prefab == null) continue;

                string key = rule.prefab.name;

                if (!enemyPools.ContainsKey(key))
                {
                    enemyPools.Add(key, new Queue<GameObject>());

                    for (int i = 0; i < 10; i++)
                    {
                        CreateNewEnemy(rule.prefab, key);
                    }
                }
            }
        }
    }

    private GameObject CreateNewEnemy(GameObject prefab, string key)
    {
        GameObject obj = Instantiate(prefab, enemyContainer);
        obj.name = key;
        obj.SetActive(false);

        enemyPools[key].Enqueue(obj);
        return obj;
    }



    void Update()
    {
        if (GameManager.Instance == null) return;

        float currentTime = GameManager.Instance.gameTime;

        // Check if it's time to transition to the next wave
        CheckNextWave(currentTime);

        if (!GameManager.Instance.isTimerPaused)
        {
            HandleSpawning();
        }
    }

    private void CheckNextWave(float time)
    {
        // Return if there are no more waves
        if (currentWaveIndex >= waves.Count - 1) return;

        WaveData nextWave = waves[currentWaveIndex + 1];
        float timeToNextWave = nextWave.startTime - time;

        if (nextWave.isBossWave && timeToNextWave <= 3.0f && timeToNextWave > 0f && !warningTriggered)
        {
            warningTriggered = true;
            // TODO: UIManager alignment for boss warning
            Debug.LogWarning("⚠️ WARNING! BOSS APPROACHING! ⚠️");

            // ex: if(UIManager.Instance != null) UIManager.Instance.ShowBossWarning();
        }

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
        warningTriggered = false;

        if (currentWave.isBossWave)
        {
            GameManager.Instance.PauseGameTimer();

            ClearAllEnemies();
        }


        // Prepare timers for the new wave's rules
        ruleTimers.Clear();
        if (currentWave.spawnRules != null)
        {
            foreach (var rule in currentWave.spawnRules)
            {
                // Boss Logic: If interval is 0 or less, spawn ONCE immediately
                if (rule.interval <= 0)
                {
                    SpawnEnemyFromPool(rule.prefab); // Spawn Immediately
                    ruleTimers.Add(-1f); // Mark as ignored (-1) for the Update loop
                }
                else
                {
                    ruleTimers.Add(0f); // Normal repeating spawn starts at 0
                }
            }
        }
        Debug.Log($"[Wave Changed] {currentWave.waveName} started at {currentWave.startTime}s");
    }
    private void ClearAllEnemies()
    {
        int childCount = enemyContainer.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = enemyContainer.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                ReturnEnemy(child.gameObject);
            }
        }
        Debug.Log("Field Cleared for Boss!");
    }

    private void HandleSpawning()
    {
        if (currentWave == null || currentWave.spawnRules == null) return;

        // Iterate through all rules in the current wave simultaneously
        for (int i = 0; i < currentWave.spawnRules.Count; i++)
        {
            SpawnRule rule = currentWave.spawnRules[i];

            // [Modified] Skip one-time spawns (marked as -1f)
            if (ruleTimers[i] < 0f) continue;

            // Skip if interval is 0 or less (Double check safety)
            if (rule.interval <= 0) continue;

            ruleTimers[i] += Time.deltaTime;

            if (ruleTimers[i] >= rule.interval)
            {
                // Trigger spawn
                if (Random.value <= rule.chance) // Check spawn probability
                {
                    SpawnEnemyFromPool(rule.prefab);
                }
                ruleTimers[i] = 0f; // Reset timer for this rule
            }
        }
    }

    private void SpawnEnemyFromPool(GameObject prefab)
    {
        string key = prefab.name;

        if (!enemyPools.ContainsKey(key))
        {
            enemyPools.Add(key, new Queue<GameObject>());
        }

        GameObject enemyObj = null;

        if (enemyPools[key].Count > 0)
        {
            enemyObj = enemyPools[key].Dequeue();
        }
        else
        {
            enemyObj = CreateNewEnemy(prefab, key);
            enemyPools[key].Dequeue();
        }

        enemyObj.transform.position = GetRandomPositionOutsideCamera();
        enemyObj.SetActive(true);

    }

    public void ReturnEnemy(GameObject enemy)
    {
        string key = enemy.name;

        enemy.SetActive(false);

        if (enemyPools.ContainsKey(key))
        {
            enemyPools[key].Enqueue(enemy);
        }
        else
        {
            Destroy(enemy);
        }
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