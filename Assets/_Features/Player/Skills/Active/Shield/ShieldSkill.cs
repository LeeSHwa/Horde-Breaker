using UnityEngine;
using System.Collections;

public class ShieldSkill : Skills
{
    private ShieldDataSO shieldData;

    // Visual components
    private GameObject visualInstance;
    private SpriteRenderer visualRenderer;

    // Runtime Stats
    private int baseMaxStacks; // From SO + Level Up
    private float currentInvulnTime;
    private bool isGhostModeUnlocked = false;
    private float ghostModeSpeedBonus;

    // State Variables
    private int currentStacks = 0;
    private bool isInvulnerable = false; // True if currently in the post-hit invulnerability phase

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        if (skillData is ShieldDataSO)
        {
            shieldData = (ShieldDataSO)skillData;
        }
        else
        {
            Debug.LogError("ShieldSkill has wrong DataSO! Expected ShieldDataSO.");
            return;
        }

        baseMaxStacks = shieldData.baseMaxStacks;
        currentInvulnTime = shieldData.baseInvulnTime;

        isGhostModeUnlocked = false;
        ghostModeSpeedBonus = 0f;

        // Visual Setup
        if (visualInstance == null && shieldData.shieldVisualPrefab != null)
        {
            visualInstance = Instantiate(shieldData.shieldVisualPrefab, transform);
            visualInstance.transform.localPosition = Vector3.zero;
            visualRenderer = visualInstance.GetComponent<SpriteRenderer>();
            visualInstance.SetActive(false);
        }

        // Subscribe to damage event
        ownerStats.OnDamageProcess -= HandleDamage;
        ownerStats.OnDamageProcess += HandleDamage;

        // Start with 1 stack
        currentStacks = 1;
        UpdateVisuals();
    }

    void OnDisable()
    {
        if (ownerStats != null)
        {
            ownerStats.OnDamageProcess -= HandleDamage;
        }
    }

    // "Attack" in this context means "Recharge Shield"
    public override void TryAttack()
    {
        // Calculate dynamic max stacks: Base(Level) + Passive(Projectile Count)
        int finalMaxStacks = baseMaxStacks + ownerStats.bonusProjectileCount;

        if (currentStacks < finalMaxStacks)
        {
            if (Time.time >= lastAttackTime + currentAttackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
    }

    protected override void PerformAttack()
    {
        int finalMaxStacks = baseMaxStacks + ownerStats.bonusProjectileCount;

        if (currentStacks < finalMaxStacks)
        {
            currentStacks++;
            UpdateVisuals();

            if (shieldData.shieldRechargeSound != null)
            {
                SoundManager.Instance.PlaySFX(shieldData.shieldRechargeSound);
            }
        }
    }

    // Returns true if damage should be blocked.
    private bool HandleDamage(float damageAmount)
    {
        if (isInvulnerable) return true;

        if (currentStacks > 0)
        {
            currentStacks--;
            StartCoroutine(InvulnerabilityRoutine());

            UpdateVisuals();

            if (shieldData.shieldBreakSound != null)
            {
                SoundManager.Instance.PlaySFX(shieldData.shieldBreakSound);
            }

            // Reset recharge timer
            lastAttackTime = Time.time;
            return true;
        }
        return false;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        if (isGhostModeUnlocked) StartGhostMode();

        yield return new WaitForSeconds(currentInvulnTime);

        if (isGhostModeUnlocked) EndGhostMode();

        isInvulnerable = false;
    }

    private void StartGhostMode()
    {
        ownerStats.SetSpeedBuff(ghostModeSpeedBonus);

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }
    }

    private void EndGhostMode()
    {
        ownerStats.SetSpeedBuff(0f);

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
    }

    private void UpdateVisuals()
    {
        if (visualInstance == null || visualRenderer == null) return;

        if (currentStacks <= 0)
        {
            visualInstance.SetActive(false);
        }
        else
        {
            visualInstance.SetActive(true);
            int finalMaxStacks = baseMaxStacks + ownerStats.bonusProjectileCount;

            float alphaRatio = (float)currentStacks / finalMaxStacks;
            float finalAlpha = Mathf.Lerp(0.3f, 0.8f, alphaRatio);

            Color c = visualRenderer.color;
            c.a = finalAlpha;
            visualRenderer.color = c;
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: ApplyStats(shieldData.level2_CooldownReduction, shieldData.level2_InvulnTimeBonus, shieldData.level2_StackIncrease, shieldData.level2_UnlockGhostMode, shieldData.level2_MoveSpeedBonus); break;
            case 3: ApplyStats(shieldData.level3_CooldownReduction, shieldData.level3_InvulnTimeBonus, shieldData.level3_StackIncrease, shieldData.level3_UnlockGhostMode, shieldData.level3_MoveSpeedBonus); break;
            case 4: ApplyStats(shieldData.level4_CooldownReduction, shieldData.level4_InvulnTimeBonus, shieldData.level4_StackIncrease, shieldData.level4_UnlockGhostMode, shieldData.level4_MoveSpeedBonus); break;
            case 5: ApplyStats(shieldData.level5_CooldownReduction, shieldData.level5_InvulnTimeBonus, shieldData.level5_StackIncrease, shieldData.level5_UnlockGhostMode, shieldData.level5_MoveSpeedBonus); break;
            case 6: ApplyStats(shieldData.level6_CooldownReduction, shieldData.level6_InvulnTimeBonus, shieldData.level6_StackIncrease, shieldData.level6_UnlockGhostMode, shieldData.level6_MoveSpeedBonus); break;
            case 7: ApplyStats(shieldData.level7_CooldownReduction, shieldData.level7_InvulnTimeBonus, shieldData.level7_StackIncrease, shieldData.level7_UnlockGhostMode, shieldData.level7_MoveSpeedBonus); break;
            case 8: ApplyStats(shieldData.level8_CooldownReduction, shieldData.level8_InvulnTimeBonus, shieldData.level8_StackIncrease, shieldData.level8_UnlockGhostMode, shieldData.level8_MoveSpeedBonus); break;
            case 9: ApplyStats(shieldData.level9_CooldownReduction, shieldData.level9_InvulnTimeBonus, shieldData.level9_StackIncrease, shieldData.level9_UnlockGhostMode, shieldData.level9_MoveSpeedBonus); break;
        }

        // Safety check
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    private void ApplyStats(float cooldown, float invuln, int stack, bool ghost, float speed)
    {
        // All stats are applied as flat values
        currentAttackCooldown -= cooldown;
        currentInvulnTime += invuln;
        baseMaxStacks += stack;
        if (ghost) isGhostModeUnlocked = true;
        ghostModeSpeedBonus += speed;
    }
}