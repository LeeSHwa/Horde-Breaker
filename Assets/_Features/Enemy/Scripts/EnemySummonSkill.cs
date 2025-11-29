using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for List

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemySummonSkill : MonoBehaviour
{
    [Header("Summon Settings")]
    [Tooltip("Assign the enemy prefabs (e.g., 3 types of Slimes) here.")]
    public List<GameObject> minionPrefabs; // List to support multiple minion types

    public int summonCount = 3;      // Number of minions to spawn at once
    public float spawnRadius = 2.0f; // Radius around the boss

    public float triggerRange = 8f;  // Skill activation range
    public float castTime = 1.0f;    // Time to freeze before spawning
    public float skillCooldown = 10.0f; // Cooldown between uses

    [Header("Visual Settings")]
    public bool useColorChange = true;
    public Color castColor = Color.magenta;

    [Header("Animation Settings")]
    [Tooltip("Trigger name for the summon animation.")]
    public string animSummonTrigger = "doSummon";
    [Tooltip("Trigger name for returning to idle.")]
    public string animIdleTrigger = "doIdle";
    [Tooltip("Bool parameter for movement state.")]
    public string animMoveBool = "isMoving";

    // Internal Components
    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Transform playerTarget;
    private Color originalColor;

    // State Flags
    private bool isSkillAvailable = true;
    private bool isCasting = false;

    void Start()
    {
        // Cache components
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr != null) originalColor = sr.color;

        // Find Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        // 1. Basic Checks: Skill availability, Casting state, Player existence
        if (!isSkillAvailable || isCasting || playerTarget == null) return;

        // 2. State Check: Do not cast if the enemy is disabled (e.g., Knockback/Stun)
        if (movementScript.enabled == false) return;

        // 3. Range Check
        float distance = Vector2.Distance(transform.position, playerTarget.position);

        if (distance <= triggerRange)
        {
            StartCoroutine(SummonRoutine());
        }
    }

    private IEnumerator SummonRoutine()
    {
        isCasting = true;
        isSkillAvailable = false;

        // 1. Stop Movement (Start Casting)
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero; // Stop physics sliding

        // 2. Play Animation & Visual Effects
        if (anim != null)
        {
            anim.SetBool(animMoveBool, false);
            anim.SetTrigger(animSummonTrigger);
        }

        if (useColorChange && sr != null) sr.color = castColor;

        // 3. Wait for Cast Time
        yield return new WaitForSeconds(castTime);

        // 4. EXECUTE SPAWN
        SpawnMinions();

        // 5. Restore State (End Casting)
        if (useColorChange && sr != null) sr.color = originalColor;

        // [NOTE] Ensure 'doIdle' trigger exists in your Animator Controller!
        if (anim != null) anim.SetTrigger(animIdleTrigger);

        if (movementScript != null) movementScript.enabled = true;
        isCasting = false;

        // 6. Start Cooldown
        yield return new WaitForSeconds(skillCooldown);
        isSkillAvailable = true;
    }

    private void SpawnMinions()
    {
        // Safety Check
        if (PoolManager.Instance == null) return;
        if (minionPrefabs == null || minionPrefabs.Count == 0)
        {
            Debug.LogWarning("EnemySummonSkill: No Minion Prefabs assigned!");
            return;
        }

        for (int i = 0; i < summonCount; i++)
        {
            // A. Select a random prefab from the list
            GameObject selectedPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Count)];
            string poolTag = selectedPrefab.name;

            // B. Try to get from PoolManager
            GameObject minion = PoolManager.Instance.GetFromPool(poolTag);

            // C. [Fallback] If not found in Pool, Instantiate it directly
            // This prevents the "Pool not found" error from stopping the game.
            if (minion == null)
            {
                minion = Instantiate(selectedPrefab);
                minion.name = poolTag; // Keep name consistent for potential future pooling
            }

            // D. Set Position (Random position inside radius)
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = (Vector2)transform.position + randomOffset;

            minion.transform.position = spawnPos;
            minion.transform.rotation = Quaternion.identity;

            // E. Reset Stats / Enable Object
            // Important: If the enemy was dead/disabled, we must ensure it wakes up correctly.
            minion.SetActive(true);
        }
    }
}