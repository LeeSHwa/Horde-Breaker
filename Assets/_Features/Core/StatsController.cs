using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    // Event triggered when the player levels up
    public event Action OnPlayerLevelUp;

    [Header("Data Source")]
    public CharacterStatsSO baseStats;

    [Header("Runtime Stats")]
    [HideInInspector]
    public float currentHP;
    private float runtimeMaxHP;
    public float currentMoveSpeed;
    public float currentDamageMultiplier;

    [Header("Passive Bonuses (Weapon Stats)")]
    public float bonusCooldownReduction = 0f;
    public int bonusProjectileCount = 0;
    public float bonusArea = 0f;
    public float bonusDuration = 0f;
    public float bonusPickupRange = 0f;

    [Header("Passive Stats (Logic)")]
    public float hpRecoveryRate = 0f;   // HP recovered per second
    public float armor = 0f;            // Flat damage reduction
    public int revivalCount = 0;        // Number of extra lives
    public float expGainMultiplier = 1f; // 1.0 = 100% (Normal), 1.1 = +10%

    // Critical Hit Stats
    public float currentCritChance;
    public float currentCritMultiplier;

    // Timer for passive health regeneration
    private float recoveryTimer = 0f;

    // --- [Damage Interception] ---
    // Delegate that returns true if damage was blocked by a skill (e.g., Shield)
    public Func<float, bool> OnDamageProcess;

    // --- [Passive Level Tracking] ---
    // Dictionary to track the current level of each acquired passive skill
    // Key: Passive Name, Value: Current Level
    private Dictionary<string, int> passiveLevels = new Dictionary<string, int>();

    // State flags and references
    private bool isDead = false;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    // Player-specific references
    private PlayerStatsSO playerStats;
    private PlayerPickup playerPickup;
    private int currentLevel = 1;
    private int currentExp = 0;
    private int expNeededForNextLevel;

    // --- Slow Effect Logic ---
    private class SpeedModifier
    {
        public object Source; // The source of the slow (e.g., ZoneLogic)
        public float SpeedPercentage; // 100-based (e.g., 70 = 30% slow)
    }

    // List of active slow modifiers
    private readonly List<SpeedModifier> activeSpeedModifiers = new List<SpeedModifier>();

    // Flag to trigger speed recalculation
    private bool needsSpeedRecalculation = false;

    // Original base speed from Scriptable Object
    private float baseMoveSpeed;

    // Speed Buff (Percentage, e.g., 0.5f = 50% boost)
    private float activeSpeedBuff = 0f;

    void Awake()
    {
        // Cache components
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerPickup = GetComponentInChildren<PlayerPickup>();

        // Initialize stats
        InitializeStats();
    }

    void Start()
    {
        // Ensure UI is updated after initialization
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
            }
            else
            {
                Debug.LogError("UIManager.Instance is null in Start().");
            }
        }
    }

    void Update()
    {
        // Recalculate speed if modifiers changed
        if (needsSpeedRecalculation)
        {
            RecalculateSpeed();
            needsSpeedRecalculation = false;
        }

        // Handle HP Regeneration
        if (gameObject.CompareTag("Player") && !isDead && hpRecoveryRate > 0)
        {
            if (currentHP < runtimeMaxHP)
            {
                recoveryTimer += Time.deltaTime;
                if (recoveryTimer >= 1f) // Tick every 1 second
                {
                    recoveryTimer = 0f;
                    Heal(hpRecoveryRate);
                }
            }
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
            Debug.LogError(gameObject.name + " is missing BaseStats SO!");
            return;
        }

        // Reset basic stats
        runtimeMaxHP = baseStats.baseMaxHealth;
        currentHP = runtimeMaxHP;
        currentMoveSpeed = baseStats.baseMoveSpeed;
        currentDamageMultiplier = baseStats.baseDamageMultiplier;

        // Reset Speed Logic
        baseMoveSpeed = baseStats.baseMoveSpeed;
        activeSpeedModifiers.Clear();
        activeSpeedBuff = 0f;
        needsSpeedRecalculation = true;

        // Reset Passive Bonuses
        bonusCooldownReduction = 0f;
        bonusProjectileCount = 0;
        bonusArea = 0f;
        bonusDuration = 0f;
        bonusPickupRange = 0f;

        hpRecoveryRate = 0f;
        armor = 0f;
        revivalCount = 0;
        expGainMultiplier = 1f;

        // Reset Passive Level Tracker
        passiveLevels.Clear();

        if (gameObject.CompareTag("Player"))
        {
            playerStats = baseStats as PlayerStatsSO;

            if (playerStats != null)
            {
                currentCritChance = playerStats.baseCritChance;
                currentCritMultiplier = playerStats.baseCritMultiplier;

                currentLevel = 1;
                currentExp = 0;
                UpdateExpNeeded();

                // Initialize UI
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
                    UIManager.Instance.UpdateExp(currentExp, expNeededForNextLevel);
                    UIManager.Instance.UpdateLevel(currentLevel);
                }

                // Initialize Pickup Radius
                if (playerPickup != null)
                {
                    playerPickup.InitializeRadius(playerStats.basePickupRadius);
                }
            }
            else
            {
                Debug.LogError("Player's StatsController is NOT using a PlayerStatsSO!");
            }
        }
        else
        {
            playerStats = null;
        }
    }

    // --- [Core Logic: Health & Damage] ---

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHP += amount;
        if (currentHP > runtimeMaxHP) currentHP = runtimeMaxHP;

        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }
    }

    public void TakeDamage(float damage, bool isCritical = false)
    {
        if (isDead || damage < 0) return;

        // Check for damage interception (e.g., Shield)
        if (OnDamageProcess != null)
        {
            bool isBlocked = OnDamageProcess.Invoke(damage);
            if (isBlocked) return;
        }

        // Show Damage Popup
        GameObject popup = PoolManager.Instance.GetFromPool("DamageText");
        if (popup != null)
        {
            popup.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            DamagePopup dp = popup.GetComponent<DamagePopup>();
            if (dp != null)
            {
                dp.Setup(damage, isCritical);
            }
        }

        Debug.Log($"{transform.name} takes {damage} damage (Crit: {isCritical})");

        // Apply Armor Reduction
        float reducedDamage = damage - armor;
        if (reducedDamage < 1) reducedDamage = 1; // Minimum damage is 1

        currentHP -= reducedDamage;

        // Update UI
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }

        // Check Death
        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        // Handle Revival (Player Only)
        if (gameObject.CompareTag("Player") && revivalCount > 0)
        {
            revivalCount--;
            currentHP = runtimeMaxHP * 0.5f; // Revive with 50% HP
            isDead = false;

            if (UIManager.Instance != null)
                UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);

            Debug.Log($"Player Revived! Lives left: {revivalCount}");
            return;
        }

        // Actual Death
        if (anim != null) anim.SetTrigger("Die");
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;

        // Handle Drops (Enemy Only)
        EnemyStatsSO enemyStats = baseStats as EnemyStatsSO;
        if (enemyStats != null)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddKillCount();
            SpawnExpOrb(enemyStats.expValue);
        }

        StartCoroutine(DieAndDisable(baseStats.deathAnimationLength));
    }

    private IEnumerator DieAndDisable(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!gameObject.CompareTag("Player"))
        {
            if (EnemySpawnerTemp.Instance != null)
                EnemySpawnerTemp.Instance.ReturnEnemy(this.gameObject);
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

    // --- [Player-Only: Experience & Level] ---

    public void AddExp(int amount)
    {
        if (!gameObject.CompareTag("Player") || playerStats == null) return;

        int finalExp = Mathf.FloorToInt(amount * expGainMultiplier);
        currentExp += finalExp;

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
            Debug.LogError("expToNextLevel array is not set in Player's StatsSO!");
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

    // --- [Passive Skill System] ---

    // Helper: Returns the current level of a passive skill (Used by LevelUpManager)
    public int GetPassiveLevel(string passiveName)
    {
        if (passiveLevels.ContainsKey(passiveName))
            return passiveLevels[passiveName];
        return 0;
    }

    // Main API: Apply a passive upgrade and track its level
    public void ApplyPassive(PassiveUpgradeSO data)
    {
        // 1. Apply Stats
        switch (data.type)
        {
            case PassiveUpgradeSO.UpgradeType.DamageMultiplier:
                ApplyDamageMultiplier(data.value);
                break;
            case PassiveUpgradeSO.UpgradeType.MaxHealth:
                ApplyMaxHealth(data.value);
                break;
            case PassiveUpgradeSO.UpgradeType.MoveSpeed:
                ApplyMoveSpeed(data.value);
                break;
            case PassiveUpgradeSO.UpgradeType.Cooldown:
                bonusCooldownReduction += data.value;
                bonusCooldownReduction = Mathf.Min(bonusCooldownReduction, 0.8f); // Cap at 80%
                break;
            case PassiveUpgradeSO.UpgradeType.ProjectileCount:
                bonusProjectileCount += (int)data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.Area:
                bonusArea += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.Duration:
                bonusDuration += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.PickupRange:
                bonusPickupRange += data.value;
                if (playerPickup != null)
                    playerPickup.InitializeRadius(playerStats.basePickupRadius + bonusPickupRange);
                break;
            case PassiveUpgradeSO.UpgradeType.HealthRecovery:
                hpRecoveryRate += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.Armor:
                armor += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.Revival:
                revivalCount += (int)data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.ExpGain:
                expGainMultiplier += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.CritChance:
                currentCritChance += data.value;
                break;
            case PassiveUpgradeSO.UpgradeType.CritDamage:
                currentCritMultiplier += data.value;
                break;
        }

        // 2. Track Level
        if (passiveLevels.ContainsKey(data.upgradeName))
        {
            passiveLevels[data.upgradeName]++;
        }
        else
        {
            passiveLevels.Add(data.upgradeName, 1);
        }

        Debug.Log($"Passive {data.upgradeName} Applied! Current Level: {passiveLevels[data.upgradeName]}");
    }

    // --- [Stat Application Helpers] ---

    public void ApplyDamageMultiplier(float multiplierBonus)
    {
        currentDamageMultiplier += multiplierBonus;
    }

    public void ApplyMaxHealth(float healthBonus)
    {
        runtimeMaxHP += healthBonus;
        currentHP += healthBonus; // Heal the amount increased

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }
    }

    public void ApplyMoveSpeed(float speedBonus)
    {
        baseMoveSpeed += speedBonus;
        needsSpeedRecalculation = true;
    }

    public void ApplyCritStats(float chanceBonus, float multiplierBonus)
    {
        currentCritChance += chanceBonus;
        currentCritMultiplier += multiplierBonus;
    }

    // --- [Speed Calculation Logic] ---

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

    // Public Properties for external access
    public int Level => currentLevel;
    public int CurrentExp => currentExp;
    public int MaxExp => expNeededForNextLevel;
    public float MaxHP => runtimeMaxHP;
}