using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class ZoneLogic : MonoBehaviour
{
    // Stats (injected via Initialize)
    private float damagePerSecond;
    private float durationTimer;
    private float speedDebuffPercent;
    private Transform playerToFollow;

    private CircleCollider2D zoneCollider;

    // [NEW] A reference to the child 'Visuals' transform
    private Transform visualsTransform;

    // [FIX] This line was accidentally deleted in the previous version.
    // This list tracks enemies currently in the zone.
    private List<StatsController> enemiesInZone = new List<StatsController>();

    void Awake()
    {
        zoneCollider = GetComponent<CircleCollider2D>();
        zoneCollider.isTrigger = true;

        // [NEW] Find the 'Visuals' child object on Awake
        visualsTransform = transform.Find("Visuals");
        if (visualsTransform == null)
        {
            // This error helps if the prefab structure is wrong
            Debug.LogError("'Visuals' child object not found on " + gameObject.name);
        }
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

        // 2. [NEW] Set the visual scale to match the collider radius
        if (visualsTransform != null)
        {
            // This assumes the base sprite (like Unity's default "Circle")
            // has a visual radius of 0.5 units when its scale is (1, 1, 1).
            // To make the visual radius match the collider radius 'area',
            // the localScale must be set to 'area * 2'.
            float visualScale = area * 2f;
            visualsTransform.localScale = new Vector3(visualScale, visualScale, 1f);
        }
    }

    // Called when reactivated from the pool
    void OnEnable()
    {
        // Clear the enemy list for reuse
        enemiesInZone.Clear();
    }

    // [Important] When disabled (returned to pool), remove slow from all enemies
    void OnDisable()
    {
        foreach (StatsController enemyStats in enemiesInZone)
        {
            if (enemyStats != null && enemyStats.gameObject.activeInHierarchy)
            {
                // Call the new function in the modified StatsController
                enemyStats.RemoveSpeedModifier(this);
            }
        }
        enemiesInZone.Clear();
    }

    void Update()
    {
        // 1. Duration check (pool return logic)
        durationTimer -= Time.deltaTime;
        if (durationTimer <= 0)
        {
            gameObject.SetActive(false); // Return to pool instead of Destroy
            return;
        }

        // 2. Follow the player
        if (playerToFollow != null)
        {
            transform.position = playerToFollow.position;
        }
        else
        {
            // If player is lost (e.g., died), return to pool
            gameObject.SetActive(false);
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
                if (!enemiesInZone.Contains(enemyStats))
                {
                    enemiesInZone.Add(enemyStats);
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<StatsController>(out var enemyStats))
            {
                enemyStats.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<StatsController>(out var enemyStats))
            {
                enemyStats.RemoveSpeedModifier(this);
                enemiesInZone.Remove(enemyStats);
            }
        }
    }
}