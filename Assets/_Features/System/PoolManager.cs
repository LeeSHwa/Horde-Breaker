using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (1) Helper class to define pools in the Inspector.
[System.Serializable]
public class Pool
{
    public string tag;             // The name of this pool (e.g., "Bullet", "Slime")
    public GameObject prefab;      // The prefab to be pooled
    public int size;               // Initial number of objects to create
    public Transform container;    // The parent transform for these objects (e.g., '@Container')
}

public class PoolManager : MonoBehaviour
{
    // (2) Singleton: Allows access from anywhere via PoolManager.Instance.
    public static PoolManager Instance;

    [Header("Pools")]
    // (3) List of pools to configure in the Inspector.
    public List<Pool> pools;

    // (4) Dictionary to store the actual pools (Tag -> Queue of GameObjects).
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        // (5) Singleton setup.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // (6) Initialize the dictionary.
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // (7) Create all defined pools at the start.
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // (8) Instantiate the object and deactivate it.
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);

                // (9) If a container is set, parent the object to it (for scene organization).
                if (pool.container != null)
                {
                    obj.transform.SetParent(pool.container);
                }

                objectPool.Enqueue(obj); // Add to the queue
            }

            // Add the new pool to the dictionary
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // (10) [Core API 1] Get an object from the pool.
    //    (Example: Called by Gun.cs)
    public GameObject GetFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // (11) Dequeue an object from the queue.
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // (Note: If queue is empty, logic for a 'growing pool' (Instantiate new) can be added here)
        // (Skipped for simplicity for now)

        objectToSpawn.SetActive(true); // Activate it!

        // (12) [Important] Re-enqueue the object immediately to circulate the pool.
        //     This means we're "renting" it, and it stays in the queue.
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }


}