using UnityEngine;
using System.Collections;

public class ShieldSkill : Skills
{
    private ShieldDataSO shieldData;

    // Visual components
    private GameObject visualInstance;
    private SpriteRenderer visualRenderer;

    // Runtime Stats
    private int currentMaxStacks;
    private float currentInvulnTime;
    private bool isGhostModeUnlocked = false;
    private float ghostModeSpeedBonus;

    // State Variables
    private int currentStacks = 0;
    private bool isInvulnerable = false; // True if currently in the post-hit invulnerability phase

    protected override void Awake()
    {
        base.Awake(); // Sets ownerStats

        if (skillData is ShieldDataSO)
        {
            shieldData = (ShieldDataSO)skillData;
        }
        else
        {
            Debug.LogError("ShieldSkill has wrong DataSO! Expected ShieldDataSO.");
        }

        InitializeStats();
    }

    // Subscribe to the damage event when enabled
    void OnEnable()
    {
        if (ownerStats != null)
        {
            ownerStats.OnDamageProcess += HandleDamage;
        }

        // Start with 1 stack as requested
        currentStacks = 1;
        UpdateVisuals();
    }

    // Unsubscribe when disabled (Very Important to prevent memory leaks or errors)
    void OnDisable()
    {
        if (ownerStats != null)
        {
            ownerStats.OnDamageProcess -= HandleDamage;
        }
    }

    protected override void InitializeStats()
    {
        base.InitializeStats(); // Initializes cooldown and base damage (damage unused here)

        currentMaxStacks = shieldData.baseMaxStacks;
        currentInvulnTime = shieldData.baseInvulnTime;

        isGhostModeUnlocked = false;
        ghostModeSpeedBonus = shieldData.level5_MoveSpeedBonus;

        // Instantiate the visual effect as a child of this object (which follows the player)
        if (visualInstance == null && shieldData.shieldVisualPrefab != null)
        {
            visualInstance = Instantiate(shieldData.shieldVisualPrefab, transform);
            visualInstance.transform.localPosition = Vector3.zero;
            visualRenderer = visualInstance.GetComponent<SpriteRenderer>();

            // Initially hidden if stack is 0 (though we force 1 in OnEnable)
            visualInstance.SetActive(false);
        }
    }

    // === 1. Recharging Logic (Called by PassiveSkillManager) ===
    public override void TryAttack()
    {
        // Only try to recharge if we are missing stacks
        if (currentStacks < currentMaxStacks)
        {
            // Check cooldown using base logic
            if (Time.time >= lastAttackTime + currentAttackCooldown)
            {
                PerformAttack(); // Recharge one stack
                lastAttackTime = Time.time;
            }
        }
        // If stacks are full, the cooldown timer essentially "pauses" or waits.
    }

    // "Attack" in this context means "Recharge Shield"
    protected override void PerformAttack()
    {
        if (currentStacks < currentMaxStacks)
        {
            currentStacks++;
            UpdateVisuals();
            Debug.Log($"Shield Recharged! Current: {currentStacks}/{currentMaxStacks}");

            // [NEW] Play Recharge Sound
            if (shieldData.shieldRechargeSound != null)
            {
                SoundManager.Instance.PlaySFX(shieldData.shieldRechargeSound);
            }
        }
    }

    // === 2. Damage Blocking Logic (Called by StatsController) ===
    // Returns true if damage should be blocked.
    private bool HandleDamage(float damageAmount)
    {
        // 1. If already invulnerable, block damage without consuming stacks.
        if (isInvulnerable)
        {
            return true;
        }

        // 2. If we have shield stacks, consume one and trigger invulnerability.
        if (currentStacks > 0)
        {
            currentStacks--;
            StartCoroutine(InvulnerabilityRoutine());

            UpdateVisuals();
            Debug.Log($"Shield Blocked! Remaining Stacks: {currentStacks}");

            // [NEW] Play Break Sound
            if (shieldData.shieldBreakSound != null)
            {
                SoundManager.Instance.PlaySFX(shieldData.shieldBreakSound);
            }

            // Reset recharge timer so the next shield takes full duration to charge
            lastAttackTime = Time.time;

            return true; // Damage Blocked!
        }

        // 3. No shield, not invulnerable -> Take Damage.
        return false;
    }

    // === 3. Invulnerability & Ghost Mode Routine ===
    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        // [Level 5] Ghost Mode: Pass through enemies and Move Faster
        if (isGhostModeUnlocked)
        {
            StartGhostMode();
        }

        // Wait for the invulnerability duration
        yield return new WaitForSeconds(currentInvulnTime);

        // End Ghost Mode
        if (isGhostModeUnlocked)
        {
            EndGhostMode();
        }

        isInvulnerable = false;
    }

    private void StartGhostMode()
    {
        // 1. Apply Speed Buff via StatsController
        ownerStats.SetSpeedBuff(ghostModeSpeedBonus);

        // 2. Ignore collision between Player and Enemy layers
        // Assuming standard Unity layers: Player and Enemy.
        // You should make sure these Layers exist in your project settings.
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }
    }

    private void EndGhostMode()
    {
        // 1. Remove Speed Buff
        ownerStats.SetSpeedBuff(0f);

        // 2. Re-enable collision
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
    }


    // === 4. Visuals Management ===
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

            // Adjust alpha based on stack count (1 stack = faint, Max stacks = solid)
            // Avoid 0 alpha, clamp between 0.3 and 1.0
            float alphaRatio = (float)currentStacks / currentMaxStacks;
            float finalAlpha = Mathf.Lerp(0.3f, 0.8f, alphaRatio); // 0.3 to 0.8 transparency

            Color c = visualRenderer.color;
            c.a = finalAlpha;
            visualRenderer.color = c;

            // Optional: Slightly scale up with more stacks
            // visualInstance.transform.localScale = Vector3.one * (1f + (currentStacks * 0.1f));
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: // Increase Max Stacks
                currentMaxStacks += shieldData.level2_StackIncrease;
                // Note: We increase MAX, but we don't instantly refill them. 
                // Player must wait for recharge.
                UpdateVisuals(); // Update alpha ratio
                break;
            case 3: // Increase Invulnerability Duration
                currentInvulnTime += shieldData.level3_InvulnTimeIncrease;
                break;
            case 4: // Reduce Recharge Time (Cooldown)
                currentAttackCooldown -= shieldData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5: // Unlock Ghost Mode
                isGhostModeUnlocked = shieldData.level5_UnlockGhostMode;
                break;
        }
    }
}