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
    // [MODIFIED] Changed to a private backing field
    private static PoolManager _instance;

    // [MODIFIED] Changed 'Instance' to a public property with a 'get' block
    public static PoolManager Instance
    {
        get
        {
            // If the instance hasn't been set yet (e.g., Awake hasn't run)
            if (_instance == null)
            {
                // [FIX] Replaced obsolete 'FindObjectOfType' with modern 'FindFirstObjectByType'
                // This resolves the CS0618 warning
                _instance = FindFirstObjectByType<PoolManager>();

                if (_instance == null)
                {
                    // This will only happen if the PoolManager object is missing or disabled
                    Debug.LogError("PoolManager is not found in the scene! Make sure it exists and is enabled.");
                }
            }
            // Return the instance
            return _instance;
        }
    }


    [Header("Pools")]
    // (3) List of pools to configure in the Inspector.
    public List<Pool> pools;

    // (4) Dictionary to store the actual pools (Tag -> Queue of GameObjects).
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        // (5) Singleton setup.
        // [MODIFIED] Use the private '_instance' field
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this) // Check if another instance already exists
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
        if (poolDictionary == null)
        {
            // This can happen if Awake() hasn't finished.
            Debug.LogError("PoolManager dictionary not initialized. Awake() may not have run yet.");
            return null;
        }

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // (11) Dequeue an object from the queue.
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // [MODIFIED] Added a null-check in case the pooled object was somehow destroyed
        if (objectToSpawn == null)
        {
            // Find the original prefab to re-instantiate
            foreach (var pool in pools)
            {
                if (pool.tag == tag)
                {
                    objectToSpawn = Instantiate(pool.prefab);
                    if (pool.container != null)
                    {
                        objectToSpawn.transform.SetParent(pool.container);
                    }
                    break;
                }
            }
        }


        objectToSpawn.SetActive(true); // Activate it!

        // (12) [Important] Re-enqueue the object immediately to circulate the pool.
        //     This means we're "renting" it, and it stays in the queue.
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}