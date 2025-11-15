using UnityEngine;
using System.Collections.Generic;
// [REMOVED] System.Linq is no longer needed

[RequireComponent(typeof(CircleCollider2D))]
public class AuraLogic : MonoBehaviour
{
    // Stats (injected via Initialize)
    private float damagePerSecond;
    private float durationTimer;
    private float speedDebuffPercent;
    private Transform playerToFollow;

    private CircleCollider2D zoneCollider;

    // A reference to the child 'Visuals' transform
    private Transform visualsTransform;

    // [MODIFIED] Switched back to a List to track enemies in the zone.
    private List<StatsController> enemiesInZone = new List<StatsController>();

    // [MODIFIED] Re-introduced the global timer for damage ticks.
    private float damageTickTimer;

    void Awake()
    {
        zoneCollider = GetComponent<CircleCollider2D>();
        zoneCollider.isTrigger = true;

        // Find the 'Visuals' child object on Awake
        visualsTransform = transform.Find("Visuals");
        if (visualsTransform == null)
        {
            // This error helps if the prefab structure is wrong
            Debug.LogError("'Visuals' child object not found on " + gameObject.name);
        }

        // [REMOVED] Dictionary key list is no longer needed
    }

    // Initialization function called on skill cast
    public void Initialize(float damage, float duration, float area, float debuff, Transform player)
    {
        this.damagePerSecond = damage;
        this.durationTimer = duration;
        this.speedDebuffPercent = debuff;
        this.playerToFollow = player;

        // 1. Set the physics collider radius
        this.zoneCollider.radius = area;

        // 2. Set the visual scale to match the collider radius
        if (visualsTransform != null)
        {
            float visualScale = area * 2f;
            visualsTransform.localScale = new Vector3(visualScale, visualScale, 1f);
        }

        // [MODIFIED] Set the global timer for the *first* tick.
        // Set to 0f to apply damage immediately on spawn,
        // or 1f to apply damage 1 second *after* spawn. Let's use 0f.
        damageTickTimer = 0f;
    }

    // Called when reactivated from the pool
    void OnEnable()
    {
        // Clear the enemy list for reuse
        enemiesInZone.Clear();

        // [MODIFIED] Reset the global damage timer
        damageTickTimer = 0f;
    }

    // [Important] When disabled (returned to pool), remove slow from all enemies
    void OnDisable()
    {
        // [MODIFIED] Iterate the List
        foreach (StatsController enemyStats in enemiesInZone)
        {
            if (enemyStats != null && enemyStats.gameObject.activeInHierarchy)
            {
                enemyStats.RemoveSpeedModifier(this);
            }
        }
        enemiesInZone.Clear();
    }

    void Update()
    {
        // --- [MODIFIED] Logic Order is Critical ---

        // 1. Follow the player
        if (playerToFollow != null)
        {
            transform.position = playerToFollow.position;
        }
        else
        {
            gameObject.SetActive(false);
            return;
        }

        // 2. Global Damage Tick Logic
        damageTickTimer -= Time.deltaTime;
        if (damageTickTimer <= 0f)
        {
            // Timer expired, apply damage to all enemies in the zone
            ApplyDamageTick();

            // Reset timer for the next 1-second pulse
            damageTickTimer += 1f;
        }

        // 3. Duration check (pool return logic) (Must be LAST)
        durationTimer -= Time.deltaTime;
        if (durationTimer <= 0)
        {
            gameObject.SetActive(false); // Return to pool
            return;
        }
    }

    // [NEW] Function to apply damage to all enemies in the list
    private void ApplyDamageTick()
    {
        // Iterate backwards. If an enemy dies and is removed,
        // this prevents skipping the next enemy in the list.
        for (int i = enemiesInZone.Count - 1; i >= 0; i--)
        {
            StatsController enemyStats = enemiesInZone[i];

            // Check if enemy is still valid and active
            if (enemyStats != null && enemyStats.gameObject.activeInHierarchy)
            {
                // Apply the full damagePerSecond value at once.
                enemyStats.TakeDamage(damagePerSecond);
            }
            // List cleanup: If enemy is null or inactive, remove it
            else if (enemyStats == null || !enemyStats.gameObject.activeInHierarchy)
            {
                enemiesInZone.RemoveAt(i);
            }
        }
    }


    // --- Slow and Damage Logic ---

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<StatsController>(out var enemyStats))
            {
                enemyStats.ApplySpeedModifier(this, speedDebuffPercent);

                // [MODIFIED] Add to List
                if (!enemiesInZone.Contains(enemyStats))
                {
                    enemiesInZone.Add(enemyStats);
                }
            }
        }
    }

    // Note: OnTriggerStay2D is no longer used for damage.

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<StatsController>(out var enemyStats))
            {
                enemyStats.RemoveSpeedModifier(this);
                // [MODIFIED] Remove from List
                enemiesInZone.Remove(enemyStats);
            }
        }
    }
}