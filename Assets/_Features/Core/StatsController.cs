using System.Collections;
using System.Collections.Generic; // Added for using List
using System.Linq; // Added for using LINQ functions like .Min(), .FirstOrDefault()
using UnityEngine;
using System; // [new-for-ShieldSkill] Added for Func

public class StatsController : MonoBehaviour
{
    public event Action OnPlayerLevelUp; // (Player-Only) Event for Level Up

    [Header("Data Source")]
    public CharacterStatsSO baseStats;

    [Header("Runtime Stats")]
    [HideInInspector]
    public float currentHP;
    private float runtimeMaxHP;
    public float currentMoveSpeed;
    public float currentDamageMultiplier;

    // Critical Stats
    public float currentCritChance;
    public float currentCritMultiplier;

    // --- [for-ShieldSkill] Added Event for Damage Interception ---
    // Returns true if the damage was blocked by a skill (like Shield)
    public Func<float, bool> OnDamageProcess;
    // ---------------------------------------------------------------

    private bool isDead = false;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    // --- References (found automatically) ---
    private PlayerStatsSO playerStats; // (Player-Only) Ref to the casted SO
    private PlayerPickup playerPickup; // (Player-Only) Ref to the child component
    private int currentLevel = 1;      // (Player-Only)
    private int currentExp = 0;        // (Player-Only)
    private int expNeededForNextLevel; // (Player-Only)

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

    // --- [new-for-ShieldSkill] Speed Buff Variable ---
    // Stores percentage increase (e.g., 0.5f = 50% boost). Default is 0.
    private float activeSpeedBuff = 0f;
    // -------------------------------------------------

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
        // Start() runs AFTER all Awake() methods (including UIManager.Awake()) are complete.
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
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
        isDead = false;
        if (col != null) col.enabled = true;
        if (rb != null) rb.simulated = true;

        InitializeStats();


    }

    public void InitializeStats()
    {
        if (baseStats == null)
        {
            // (This might be the *other* cause, see Error 2 fix)
            Debug.LogError(gameObject.name + " is missing BaseStats SO!");
            return; // Stop here if baseStats is null
        }

        runtimeMaxHP = baseStats.baseMaxHealth;
        currentHP = runtimeMaxHP;

        currentMoveSpeed = baseStats.baseMoveSpeed;
        currentDamageMultiplier = baseStats.baseDamageMultiplier;

        // Initialize Critical Stats
        currentCritChance = baseStats.baseCritChance;
        currentCritMultiplier = baseStats.baseCritMultiplier;

        // --- Slow Logic Initialization ---
        baseMoveSpeed = baseStats.baseMoveSpeed; // Store the original speed
        activeSpeedModifiers.Clear(); // Remove all slow effects

        // --- [new-for-ShieldSkill] Reset Speed Buff ---
        activeSpeedBuff = 0f;
        // ----------------------------------------------

        needsSpeedRecalculation = true; // Set flag to restore speed to base on OnEnable
        // ---

        if (gameObject.CompareTag("Player"))
        {
            // Cast to PlayerStatsSO
            playerStats = baseStats as PlayerStatsSO;
            if (playerStats == null)
            {
                Debug.LogError("Player's StatsController is NOT using a PlayerStatsSO!");
            }

            // Init Level & Exp
            currentLevel = 1;
            currentExp = 0;
            UpdateExpNeeded();

            // Init UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
                UIManager.Instance.UpdateExp(currentExp, expNeededForNextLevel);
                UIManager.Instance.UpdateLevel(currentLevel);
            }

            // Init Pickup Radius using SO data
            if (playerPickup != null && playerStats != null)
            {
                playerPickup.InitializeRadius(playerStats.basePickupRadius);
            }
        }
        else
        {
            playerStats = null; // Ensure this is null for enemies
        }


    }

    public void TakeDamage(float damage, bool isCritical = false)
    {
        if (isDead || damage < 0) return;

        if (OnDamageProcess != null)
        {
            bool isBlocked = OnDamageProcess.Invoke(damage);
            if (isBlocked) return;
        }

        currentHP -= damage;

        GameObject popup = PoolManager.Instance.GetFromPool("DamageText");
        if (popup != null)
        {
            popup.transform.position = transform.position + new Vector3(0, 0.5f, 0);

            DamagePopup dp = popup.GetComponent<DamagePopup>();
            if (dp != null)
            {
                // [MODIFIED] Pass isCritical flag to Popup
                dp.Setup(damage, isCritical);
            }
        }

        Debug.Log(transform.name + " takes " + damage + " damage (Crit: " + isCritical + ")");

        if (gameObject.CompareTag("Player"))
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;
            Die();
        }
    }

    // Helper to boost Crit Stats (for Passive Items)
    public void ApplyCritStats(float chanceBonus, float multiplierBonus)
    {
        currentCritChance += chanceBonus;
        currentCritMultiplier += multiplierBonus;
    }

    private void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;

        if (!gameObject.CompareTag("Player"))
        {
            EnemyStatsSO enemyStats = baseStats as EnemyStatsSO;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddKillCount();
            }

            if (enemyStats != null)
            {
                SpawnExpOrb(enemyStats.expValue);
            }
        }

        StartCoroutine(DieAndDisable(baseStats.deathAnimationLength));
    }

    private IEnumerator DieAndDisable(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!gameObject.CompareTag("Player"))
        {
            if (EnemySpawnerTemp.Instance != null)
            {
                EnemySpawnerTemp.Instance.ReturnEnemy(this.gameObject);
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                gameObject.SetActive(false);
                GameManager.Instance.GameOver();
            }
        }
    }

    private void SpawnExpOrb(int expAmount)
    {
        GameObject expOrb = PoolManager.Instance.GetFromPool("ExpOrb");
        if (expOrb != null)
        {
            expOrb.transform.position = transform.position;

            ExpOrb orbComponent = expOrb.GetComponent<ExpOrb>();
            if (orbComponent != null)
            {
                orbComponent.Initialize(expAmount);
            }
        }
    }

    // --- Player-Only Functions ---

    public void AddExp(int amount)
    {
        if (!gameObject.CompareTag("Player") || playerStats == null) return;

        currentExp += amount;

        while (currentExp >= expNeededForNextLevel)
        {
            currentExp -= expNeededForNextLevel;
            LevelUp();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateExp(currentExp, expNeededForNextLevel);
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        Debug.Log($"LEVEL UP! New Level: {currentLevel}");

        OnPlayerLevelUp?.Invoke();

        UpdateExpNeeded();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLevel(currentLevel);
        }
    }

    private void UpdateExpNeeded()
    {
        if (playerStats == null || playerStats.expToNextLevel == null || playerStats.expToNextLevel.Length == 0)
        {
            if (playerStats != null) Debug.LogError("expToNextLevel array is not set in Player's StatsSO!");
            expNeededForNextLevel = 999999;
            return;
        }

        int index = currentLevel - 1;

        if (index < playerStats.expToNextLevel.Length)
        {
            expNeededForNextLevel = playerStats.expToNextLevel[index];
        }
        else
        {
            expNeededForNextLevel = playerStats.expToNextLevel[playerStats.expToNextLevel.Length - 1];
        }
    }

    //-------------------------passive skill API-------------------------
    // Adds a flat bonus to the damage multiplier
    public void ApplyDamageMultiplier(float multiplierBonus)
    {
        currentDamageMultiplier += multiplierBonus;
        Debug.Log($"Damage Multiplier updated to {currentDamageMultiplier}");
    }

    // Adds a flat bonus to max health
    public void ApplyMaxHealth(float healthBonus)
    {
        runtimeMaxHP += healthBonus;
        currentHP += healthBonus;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }
    }

    public void ApplySpeedModifier(object source, float percentage)
    {
        var existingMod = activeSpeedModifiers.FirstOrDefault(m => m.Source == source);

        if (existingMod != null)
        {
            if (existingMod.SpeedPercentage != percentage)
            {
                existingMod.SpeedPercentage = percentage;
                needsSpeedRecalculation = true;
            }
        }
        else
        {
            activeSpeedModifiers.Add(new SpeedModifier { Source = source, SpeedPercentage = percentage });
            needsSpeedRecalculation = true;
        }
    }

    public void RemoveSpeedModifier(object source)
    {
        int removedCount = activeSpeedModifiers.RemoveAll(m => m.Source == source);
        if (removedCount > 0)
        {
            needsSpeedRecalculation = true;
        }
    }

    public void SetSpeedBuff(float buffPercent)
    {
        if (activeSpeedBuff != buffPercent)
        {
            activeSpeedBuff = buffPercent;
            needsSpeedRecalculation = true;
        }
    }

    private void RecalculateSpeed()
    {
        float speedAfterSlow = baseMoveSpeed;

        if (activeSpeedModifiers.Count > 0)
        {
            float slowestPercentage = activeSpeedModifiers.Min(m => m.SpeedPercentage);
            speedAfterSlow = baseMoveSpeed * (slowestPercentage / 100f);
        }

        currentMoveSpeed = speedAfterSlow * (1f + activeSpeedBuff);
    }
    public int Level => currentLevel;
    public int CurrentExp => currentExp;
    public int MaxExp => expNeededForNextLevel;
    public float MaxHP => runtimeMaxHP;
}