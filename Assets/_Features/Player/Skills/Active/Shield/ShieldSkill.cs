using UnityEngine;
using System.Collections;

public class ShieldSkill : Skills
{
    private ShieldDataSO shieldData;

    private GameObject visualInstance;
    private SpriteRenderer visualRenderer;

    private int currentMaxStacks;
    private float currentInvulnTime;
    private bool isGhostModeUnlocked = false;
    private float ghostModeSpeedBonus;

    private int currentStacks = 0;
    private bool isInvulnerable = false;

    // Initialize Override
    public override void Initialize(StatsController owner)
    {
        // 1. Base Init
        base.Initialize(owner);

        // 2. Data Casting
        if (skillData is ShieldDataSO)
        {
            shieldData = (ShieldDataSO)skillData;
        }
        else
        {
            Debug.LogError("ShieldSkill has wrong DataSO!");
            return;
        }

        // 3. Setup Stats (Including Passive mapping)
        // [Mapping Logic] Projectile Count -> Extra Shield Stacks
        int bonusStacks = ownerStats.bonusProjectileCount;
        currentMaxStacks = shieldData.baseMaxStacks + bonusStacks;

        currentInvulnTime = shieldData.baseInvulnTime;
        isGhostModeUnlocked = false;
        ghostModeSpeedBonus = shieldData.level5_MoveSpeedBonus;

        // 4. Visual Init
        if (visualInstance == null && shieldData.shieldVisualPrefab != null)
        {
            visualInstance = Instantiate(shieldData.shieldVisualPrefab, transform);
            visualInstance.transform.localPosition = Vector3.zero;
            visualRenderer = visualInstance.GetComponent<SpriteRenderer>();
            visualInstance.SetActive(false);
        }

        // 5. Events
        // Important: Re-subscribe if Initialize is called again
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

    // InitializeStats is handled inside Initialize now.
    // protected override void InitializeStats() { ... }

    public override void TryAttack()
    {
        // Check if stats changed dynamically (e.g., passive gained mid-game)
        if (ownerStats != null)
        {
            // Update Max Stacks dynamically based on passive
            int bonusStacks = ownerStats.bonusProjectileCount;
            int baseMax = shieldData.baseMaxStacks;

            // Check level up bonus
            if (currentLevel >= 2) baseMax += shieldData.level2_StackIncrease;

            currentMaxStacks = baseMax + bonusStacks;
        }

        if (currentStacks < currentMaxStacks)
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
        if (currentStacks < currentMaxStacks)
        {
            currentStacks++;
            UpdateVisuals();

            if (shieldData.shieldRechargeSound != null)
            {
                SoundManager.Instance.PlaySFX(shieldData.shieldRechargeSound);
            }
        }
    }

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
            float alphaRatio = (float)currentStacks / currentMaxStacks;
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
            case 2:
                // Just update internal base stat logic, visuals update in TryAttack loop or next hit
                break;
            case 3:
                currentInvulnTime += shieldData.level3_InvulnTimeIncrease;
                break;
            case 4:
                currentAttackCooldown -= shieldData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                isGhostModeUnlocked = shieldData.level5_UnlockGhostMode;
                break;
        }
    }
}