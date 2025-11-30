using UnityEngine;

public abstract class Skills : MonoBehaviour, AttackInterface
{
    [Header("Data Source")]
    public SkillDataSO skillData;

    [Header("Runtime Stats")]
    protected int currentLevel = 1;
    protected float currentDamage;
    protected float currentAttackCooldown;
    protected int currentProjectileCount = 1;
    protected float lastAttackTime;

    // Reference injected via Initialize()
    protected StatsController ownerStats;

    public int CurrentLevel { get { return currentLevel; } }
    public int MaxLevel { get { return skillData.maxLevel; } }


    public string GetName()
    {
        // Example: Get name from the Scriptable Object
        return skillData.skillName; // Assuming weaponName is in SO
    }
    public string GetNextLevelDescription()
    {
        // Must reference skillData, NOT weaponData
        int index = currentLevel - 1; // Level 2 desc is at index 0
        if (skillData.levelDescriptions != null && index < skillData.levelDescriptions.Count)
        {
            return skillData.levelDescriptions[index];
        }
        return "Max Level Reached";
    }

    public Sprite GetIcon()
    {
        // Example: Get icon from the Scriptable Object
        return skillData.icon;
    }

    // Removed Awake logic to prevent dependency issues.
    // protected virtual void Awake() { ... }

    // Initialize method to inject dependencies (StatsController)
    public virtual void Initialize(StatsController owner)
    {
        this.ownerStats = owner;
        InitializeStats();
    }

    public virtual void TryAttack()
    {
        // Safety check
        if (ownerStats == null) return;

        if (Time.time >= lastAttackTime + currentAttackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;

            // Cast Sound Logic
            if (skillData.castSound != null)
            {
                SoundManager.Instance.PlaySFX(skillData.castSound, 0.1f);
            }
        }
    }

    protected abstract void PerformAttack();

    public virtual void LevelUp()
    {
        if (currentLevel >= 5) return;

        currentLevel++;
        ApplyLevelUpStats();
    }

    protected abstract void ApplyLevelUpStats();

    protected virtual void InitializeStats()
    {
        currentLevel = 1;
        currentDamage = skillData.baseDamage;
        currentAttackCooldown = skillData.baseAttackCooldown;
        currentProjectileCount = 1;
        lastAttackTime = -999f; // Set to allow immediate attack
    }
}