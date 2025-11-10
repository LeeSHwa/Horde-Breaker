using UnityEngine;

public class EnemyContactAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    // Time interval between consecutive hits. 
    // Set this very low (e.g., 0.1) for rapid "draining" damage like Vampire Survivors.
    public float hitCooldown = 0.5f; // Default is 0.5s, adjust in Inspector for rapid hits.

    private float lastHitTime;
    private StatsController myStats;

    void Start()
    {
        // 1. Get my own StatsController component.
        myStats = GetComponent<StatsController>();
        if (myStats == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing a StatsController!");
        }
    }

    // Called every frame while maintaining collision with another collider.
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Tag Check: Confirm the target is the Player.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Cooldown Check: Has enough time passed for the next hit?
            if (Time.time >= lastHitTime + hitCooldown)
            {
                // 3. Get the player's StatsController.
                StatsController playerStats = collision.gameObject.GetComponent<StatsController>();

                if (playerStats != null)
                {
                    // 4. Calculate final damage from SO data.
                    float finalDamage = GetContactDamage();

                    // 5. Apply damage to the player.
                    playerStats.TakeDamage(finalDamage);
                    lastHitTime = Time.time; // Update last hit time.
                }
            }
        }
    }

    // Helper method to retrieve contact damage from the ScriptableObject.
    private float GetContactDamage()
    {
        // Check if myStats exists and if the assigned baseStats is of type EnemyStatsSO.
        if (myStats != null && myStats.baseStats is EnemyStatsSO enemyStats)
        {
            // Calculate: Base Contact Damage * Current Multiplier (for buffs/debuffs).
            return enemyStats.contactDamage * myStats.currentDamageMultiplier;
        }

        // Fallback: If not properly set up, return a default value to avoid 0 damage.
        // Debug.LogWarning($"{gameObject.name}: EnemyStatsSO not assigned. Using default damage (5).");
        return 5f;
    }
}