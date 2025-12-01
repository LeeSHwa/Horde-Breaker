using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public event Action OnStatsChanged;
    public event Action OnPlayerLevelUp;

    [Header("Data Source")]
    public CharacterStatsSO baseStats;

    [Header("Runtime Stats")]
    [HideInInspector]
    public float currentHP;
    private float runtimeMaxHP;
    public float currentMoveSpeed;
    public float currentDamageMultiplier;

    public List<PassiveUpgradeSO> learnedPassives = new List<PassiveUpgradeSO>();

    [Header("Passive Bonuses (Weapon Stats)")]
    public float bonusCooldownReduction = 0f;
    public int bonusProjectileCount = 0;
    public float bonusArea = 0f;
    public float bonusDuration = 0f;
    public float bonusPickupRange = 0f;

    [Header("Passive Stats (Logic)")]
    public float hpRecoveryRate = 0f;
    public float armor = 0f;
    public int revivalCount = 0;
    public float expGainMultiplier = 1f;

    public float currentCritChance;
    public float currentCritMultiplier;

    private float recoveryTimer = 0f;

    public Func<float, bool> OnDamageProcess;

    // Passive Level Dictionary
    private Dictionary<string, int> passiveLevels = new Dictionary<string, int>();

    private bool isDead = false;
    private bool isInvincible = false;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer; // rivival use

    private PlayerStatsSO playerStats;
    private PlayerPickup playerPickup;
    private int currentLevel = 1;
    private int currentExp = 0;
    private int expNeededForNextLevel;

    [Header("Drop Settings")]
    [Range(0f, 1f)] private float expDropChance = 1f;
    [Range(0f, 1f)] private float meatDropChance = 0.005f;
    [Range(0f, 1f)] private float magnetDropChance = 0.0005f;

    public string meatPrefabName = "Item_Meat";
    public string magnetPrefabName = "Item_Magnet";

    private class SpeedModifier
    {
        public object Source;
        public float SpeedPercentage;
    }

    private readonly List<SpeedModifier> activeSpeedModifiers = new List<SpeedModifier>();
    private bool needsSpeedRecalculation = false;
    private float baseMoveSpeed;
    private float activeSpeedBuff = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerPickup = GetComponentInChildren<PlayerPickup>();

        InitializeStats();
    }

    void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
            }
        }
    }

    void Update()
    {
        if (needsSpeedRecalculation)
        {
            RecalculateSpeed();
            needsSpeedRecalculation = false;
            OnStatsChanged?.Invoke();
        }

        if (gameObject.CompareTag("Player") && !isDead && hpRecoveryRate > 0)
        {
            if (currentHP < runtimeMaxHP)
            {
                recoveryTimer += Time.deltaTime;
                if (recoveryTimer >= 1f)
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
        isInvincible = false;
        if (col != null) col.enabled = true;
        if (rb != null) rb.simulated = true;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        InitializeStats();
    }

    public void InitializeStats()
    {
        if (baseStats == null) return;

        runtimeMaxHP = baseStats.baseMaxHealth;
        currentHP = runtimeMaxHP;
        currentMoveSpeed = baseStats.baseMoveSpeed;
        currentDamageMultiplier = baseStats.baseDamageMultiplier;

        baseMoveSpeed = baseStats.baseMoveSpeed;
        activeSpeedModifiers.Clear();
        activeSpeedBuff = 0f;
        needsSpeedRecalculation = true;

        bonusCooldownReduction = 0f;
        bonusProjectileCount = 0;
        bonusArea = 0f;
        bonusDuration = 0f;
        bonusPickupRange = 0f;

        hpRecoveryRate = 0f;
        armor = 0f;
        revivalCount = 0;
        expGainMultiplier = 1f;

        passiveLevels.Clear();
        learnedPassives.Clear();

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

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
                    UIManager.Instance.UpdateExp(currentExp, expNeededForNextLevel);
                    UIManager.Instance.UpdateLevel(currentLevel);
                }

                if (playerPickup != null)
                {
                    playerPickup.InitializeRadius(playerStats.basePickupRadius);
                }
            }
        }
        else
        {
            playerStats = null;
        }

        OnStatsChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHP += amount;
        if (currentHP > runtimeMaxHP) currentHP = runtimeMaxHP;

        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }

        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(float damage, bool isCritical = false)
    {
        if (isDead || isInvincible || damage < 0) return;

        if (OnDamageProcess != null)
        {
            bool isBlocked = OnDamageProcess.Invoke(damage);
            if (isBlocked) return;
        }

        GameObject popup = PoolManager.Instance.GetFromPool("DamageText");
        if (popup != null)
        {
            popup.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            DamagePopup dp = popup.GetComponent<DamagePopup>();
            if (dp != null) dp.Setup(damage, isCritical);
        }

        float reducedDamage = damage - armor;
        if (reducedDamage < 0.1f) reducedDamage = 0.1f;

        currentHP -= reducedDamage;

        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);
        }

        OnStatsChanged?.Invoke();

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        if (gameObject.CompareTag("Player") && revivalCount > 0)
        {
            isDead = true;
            Time.timeScale = 0f;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowRevivalPopup(revivalCount);
            }
            return;
        }

        isDead = true;
        if (anim != null) anim.SetTrigger("Die");
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;

        EnemyStatsSO enemyStats = baseStats as EnemyStatsSO;
        if (enemyStats != null)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddKillCount();
            SpawnLoot(enemyStats.expValue);
        }

        StartCoroutine(DieAndDisable(baseStats.deathAnimationLength));
    }

    public void RevivePlayer()
    {
        if (revivalCount <= 0) return;

        revivalCount--;

        currentHP = runtimeMaxHP * 0.5f;
        isDead = false;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHP((int)currentHP, (int)runtimeMaxHP);

        OnStatsChanged?.Invoke();

        Time.timeScale = 1f;
        StartCoroutine(InvincibilityRoutine(3.0f));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        float timer = 0f;
        float blinkInterval = 0.1f;

        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = (c.a == 1f) ? 0.3f : 1f;
                spriteRenderer.color = c;
            }
            yield return new WaitForSecondsRealtime(blinkInterval);
            timer += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        isInvincible = false;
        Debug.Log("Player Invincibility Ended");
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

    // --- Dropping System ---
    private void SpawnLoot(int expAmount)
    {
        if (UnityEngine.Random.value <= expDropChance)
        {
            SpawnExpOrb(expAmount);
        }

        float randomValue = UnityEngine.Random.value;
        float currentChanceCheck = 0f;

        if (CheckDrop(ref currentChanceCheck, randomValue, magnetDropChance))
        {
            SpawnItem(magnetPrefabName);
            return;
        }

        if (CheckDrop(ref currentChanceCheck, randomValue, meatDropChance))
        {
            SpawnItem(meatPrefabName);
            return;
        }
    }
    private bool CheckDrop(ref float currentCheckSum, float randomValue, float dropChance)
    {
        currentCheckSum += dropChance;
        return randomValue <= currentCheckSum;
    }

    private void SpawnExpOrb(int expAmount)
    {
        GameObject expOrb = PoolManager.Instance.GetFromPool("ExpOrb");
        if (expOrb != null)
        {
            expOrb.transform.position = transform.position;
            ExpOrb orbComponent = expOrb.GetComponent<ExpOrb>();
            if (orbComponent != null) orbComponent.Initialize(expAmount);
        }
    }

    private void SpawnItem(string prefabName)
    {
        GameObject item = PoolManager.Instance.GetFromPool(prefabName);
        if (item != null)
        {
            item.transform.position = transform.position;
        }
    }

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
        OnStatsChanged?.Invoke();
    }

    private void LevelUp()
    {
        currentLevel++;
        OnPlayerLevelUp?.Invoke();
        UpdateExpNeeded();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLevel(currentLevel);
        }
    }

    private void UpdateExpNeeded()
    {
        if (playerStats == null || playerStats.expToNextLevel == null) return;

        int index = currentLevel - 1;
        if (index < playerStats.expToNextLevel.Length)
            expNeededForNextLevel = playerStats.expToNextLevel[index];
        else
            expNeededForNextLevel = playerStats.expToNextLevel[playerStats.expToNextLevel.Length - 1];
    }

    // --- [Passive Skill System] ---

    public int GetPassiveLevel(string passiveName)
    {
        if (passiveLevels.ContainsKey(passiveName))
            return passiveLevels[passiveName];
        return 0;
    }

    public void ApplyPassive(PassiveUpgradeSO data)
    {
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
                bonusCooldownReduction = Mathf.Min(bonusCooldownReduction, 0.8f);
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

        if (passiveLevels.ContainsKey(data.upgradeName))
        {
            passiveLevels[data.upgradeName]++;
        }
        else
        {
            passiveLevels.Add(data.upgradeName, 1);
            learnedPassives.Add(data);
        }

        OnStatsChanged?.Invoke();
    }

    public void ApplyDamageMultiplier(float multiplierBonus)
    {
        currentDamageMultiplier += multiplierBonus;
    }

    public void ApplyMaxHealth(float healthBonus)
    {
        runtimeMaxHP += healthBonus;
        currentHP += healthBonus;

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