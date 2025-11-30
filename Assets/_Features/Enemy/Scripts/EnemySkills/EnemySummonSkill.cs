using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemySummonSkill : MonoBehaviour
{
    [Header("Summon Settings")]
    [Tooltip("Assign the enemy prefabs (e.g., 3 types of Slimes) here.")]
    public List<GameObject> minionPrefabs;

    public int summonCount = 3;      // Number of minions to spawn at once
    public float spawnRadius = 2.0f; // Radius around the boss
    public float triggerRange = 8f;  // Skill activation range
    public float castTime = 1.0f;    // Time to freeze before spawning
    public float skillCooldown = 10.0f; // Cooldown between uses

    [Header("Visual Settings")]
    public bool useColorChange = true;
    public Color castColor = Color.magenta;

    [Header("Animation Settings")]
    public string animSummonTrigger = "doSummon";
    public string animIdleTrigger = "doIdle";
    public string animMoveBool = "isMoving";

    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Color originalColor;

    private float lastUsedTime = -999f;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr != null) originalColor = sr.color;
    }

    // [Manager Call]
    public bool IsReady(float distanceToPlayer)
    {
        bool isCooldownReady = Time.time >= lastUsedTime + skillCooldown;
        bool isRangeReady = distanceToPlayer <= triggerRange;

        // Also check if movement is enabled (enemy is not stunned/knocked back)
        bool isNotDisabled = (movementScript != null && movementScript.enabled);

        return isCooldownReady && isRangeReady && isNotDisabled;
    }

    // [Manager Call]
    public void Execute(System.Action onComplete)
    {
        lastUsedTime = Time.time;
        StartCoroutine(SummonRoutine(onComplete));
    }

    private IEnumerator SummonRoutine(System.Action onComplete)
    {
        // 1. Stop Movement (Start Casting)
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

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

        if (anim != null) anim.SetTrigger(animIdleTrigger);

        if (movementScript != null) movementScript.enabled = true;

        // 6. Notify Manager
        onComplete?.Invoke();
    }

    private void SpawnMinions()
    {
        if (PoolManager.Instance == null) return;
        if (minionPrefabs == null || minionPrefabs.Count == 0) return;

        for (int i = 0; i < summonCount; i++)
        {
            GameObject selectedPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Count)];
            string poolTag = selectedPrefab.name;

            GameObject minion = PoolManager.Instance.GetFromPool(poolTag);

            if (minion == null)
            {
                minion = Instantiate(selectedPrefab);
                minion.name = poolTag;
            }

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = (Vector2)transform.position + randomOffset;

            minion.transform.position = spawnPos;
            minion.transform.rotation = Quaternion.identity;
            minion.SetActive(true);
        }
    }
}