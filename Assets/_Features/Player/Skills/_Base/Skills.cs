using UnityEngine;

public abstract class Skills : MonoBehaviour
{
    [Header("Data Source")]

    public SkillDataSO skillData;

    [Header("Runtime Stats")]
    protected int currentLevel = 1;
    protected float currentDamage;
    protected float currentAttackCooldown;
    protected int currentProjectileCount = 1;
    protected float lastAttackTime;

    protected StatsController ownerStats;

    protected virtual void Awake()
    {
        ownerStats = GetComponentInParent<StatsController>();
        if (ownerStats == null)
        {
            Debug.LogError("Skill cannot find StatsController in parent!");
        }

        // [MODIFIED] Do NOT call InitializeStats() here.
        // The child script must call it after setting up its specific data.
        // InitializeStats(); // [REMOVED]
    }

    public virtual void TryAttack()
    {
        if (Time.time >= lastAttackTime + currentAttackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
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
        lastAttackTime = 0;
    }
}