using UnityEngine;
using System.Collections.Generic;

// [1] Definition of an individual spawn rule
// (e.g., Slimes spawn every 0.5s, Goblins every 3s)
[System.Serializable]
public class SpawnRule
{
    public string note;             // Developer note (e.g., "Fast Mobs")
    public GameObject prefab;       // Enemy prefab to spawn
    public float interval;          // Spawn interval in seconds
    [Range(0f, 1f)]
    public float chance = 1.0f;     // Spawn probability (1.0 = 100%)
}

// [2] Wave Data
// (Collection of rules active during a specific time period)
[CreateAssetMenu(fileName = "NewWave", menuName = "Scriptable Objects/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Info")]
    public string waveName;          // Name of the wave (e.g., "Early Rush")
    public float startTime;          // Start time in seconds

    [Header("Rules")]
    public List<SpawnRule> spawnRules; // List of spawn rules active in this wave

    [Header("Boss")]
    public bool isBossWave;          // Flag for boss waves (can be used for UI warnings)
}