using System.Collections;
using System.Collections.Generic; // Added for using List
using System.Linq; // Added for using LINQ functions like .Min(), .FirstOrDefault()
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [Header("Data Source")]
    public CharacterStatsSO baseStats;

    [Header("Runtime Stats")]
    [HideInInspector]
    public float currentHP;
    public float currentMoveSpeed;
    public float currentDamageMultiplier;

    private bool isDead = false;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    // --- Slow Effect (Integrated from SpeedEffectController) ---
    private class SpeedModifier
    {
        public object Source; // The source that applied the slow (e.g., a ZoneLogic instance)
        public float SpeedPercentage; // 100-based (e.g., 70 = 30% slow)
    }
    // List of all currently active slow effects
    private readonly List<SpeedModifier> activeSpeedModifiers = new List<SpeedModifier>();
    // Flag to check if a speed recalculation is needed
    private bool needsSpeedRecalculation = false;
    // The original base move speed from the SO
    private float baseMoveSpeed;
    // --- End of Slow Effect ---

    void Awake()
    {
        // Get components
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Initialize stats (especially for non-pooled objects like Player)
        InitializeStats();
    }

    void Start()
    {
        // [FIX] Move the UI logic here.
        // Start() runs AFTER all Awake() methods (including UIManager.Awake()) are complete.
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
            }
            else
            {
                Debug.LogError("UIManager.Instance is STILL null, even in Start()! Check the UIManager object.");
            }
        }
    }

    // Added Update for slow recalculation
    void Update()
    {
        // Run only when a recalculation is needed
        if (needsSpeedRecalculation)
        {
            RecalculateSpeed();
            needsSpeedRecalculation = false; // Reset flag after processing
        }
    }

    void OnEnable()
    {
        // ... (Re-enable components: isDead, col, rb) ...
        // Added component re-activation logic (was not in the original StatsController.cs)
        isDead = false;
        if (col != null) col.enabled = true;
        if (rb != null) rb.simulated = true;

        InitializeStats(); // OK to re-initialize stats

        // [FIX] We also need to update UI when re-enabled (e.g. Player respawns)
        // But we must check if UIManager.Instance is ready.
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
        }
    }

    public void InitializeStats()
    {
        if (baseStats == null)
        {
            // (This might be the *other* cause, see Error 2 fix)
            Debug.LogError(gameObject.name + " is missing BaseStats SO!");
            return; // Stop here if baseStats is null
        }

        currentHP = baseStats.baseMaxHealth;
        currentMoveSpeed = baseStats.baseMoveSpeed;
        currentDamageMultiplier = baseStats.baseDamageMultiplier;

        // --- Slow Logic Initialization ---
        baseMoveSpeed = baseStats.baseMoveSpeed; // Store the original speed
        activeSpeedModifiers.Clear(); // Remove all slow effects
        needsSpeedRecalculation = true; // Set flag to restore speed to base on OnEnable
        // ---
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage < 0) return;

        currentHP -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (gameObject.CompareTag("Player"))
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
        }

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;

        StartCoroutine(DieAndDisable(baseStats.deathAnimationLength));
    }

    private IEnumerator DieAndDisable(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // --- [New] Function to Apply Slow Effect (called by ZoneLogic.cs) ---
    public void ApplySpeedModifier(object source, float percentage)
    {
        // Check if this effect is already active
        var existingMod = activeSpeedModifiers.FirstOrDefault(m => m.Source == source);

        if (existingMod != null)
        {
            // If the effect exists, just update its value (accounts for per-frame calls)
            if (existingMod.SpeedPercentage != percentage)
            {
                existingMod.SpeedPercentage = percentage;
                needsSpeedRecalculation = true;
            }
        }
        else
        {
            // Add the new effect
            activeSpeedModifiers.Add(new SpeedModifier { Source = source, SpeedPercentage = percentage });
            needsSpeedRecalculation = true;
        }
    }

    // --- [New] Function to Remove Slow Effect (called by ZoneLogic.cs) ---
    public void RemoveSpeedModifier(object source)
    {
        // Remove all effects registered by this 'source'
        int removedCount = activeSpeedModifiers.RemoveAll(m => m.Source == source);
        if (removedCount > 0)
        {
            needsSpeedRecalculation = true; // Need to recalculate speed if an effect was removed
        }
    }

    // --- [New] Speed Recalculation Function ---
    private void RecalculateSpeed()
    {
        if (activeSpeedModifiers.Count == 0)
        {
            // If no effects are active, restore to base speed
            currentMoveSpeed = baseMoveSpeed;
            return;
        }

        // Find the effect that slows the most (lowest percentage) and apply it
        // (e.g., If 80% slow and 70% slow overlap, the 70% slow is applied)
        float slowestPercentage = activeSpeedModifiers.Min(m => m.SpeedPercentage);
        currentMoveSpeed = baseMoveSpeed * (slowestPercentage / 100f);
    }
}