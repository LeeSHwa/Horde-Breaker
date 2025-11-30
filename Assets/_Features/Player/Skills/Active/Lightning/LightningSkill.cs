using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LightningSkill : Skills
{
    private LightningDataSO lightningData;

    // Runtime State
    private bool isTier2Unlocked = false;
    private bool isTier3Unlocked = false;
    private int currentMaxBranches;

    // Helper Lists to reduce garbage collection
    private List<Transform> groupA = new List<Transform>();
    private List<Transform> groupB = new List<Transform>();
    private List<Transform> groupC = new List<Transform>();

    // Tracks how many branches each source has emitted
    private Dictionary<Transform, int> branchCounts = new Dictionary<Transform, int>();

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        if (skillData is LightningDataSO)
        {
            lightningData = (LightningDataSO)skillData;
        }
        else
        {
            Debug.LogError("LightningSkill: Wrong DataSO assigned!");
            return;
        }

        // Reset Tiers based on Level 1 state
        isTier2Unlocked = false;
        isTier3Unlocked = false;
        currentMaxBranches = lightningData.maxBranches;
    }

    protected override void PerformAttack()
    {
        // Calculate total strikes based on passive stats
        int totalStrikes = currentProjectileCount + ownerStats.bonusProjectileCount;

        // Repeat the lightning strike logic for each count
        // Duplicate hits are allowed as per design choice
        for (int i = 0; i < totalStrikes; i++)
        {
            PerformSingleStrike();
        }
    }

    private void PerformSingleStrike()
    {
        // 1. Find a random Pivot (Lightning Rod) inside the screen bounds
        Transform pivot = GetRandomEnemyOnScreen();
        if (pivot == null) return;

        // 2. Apply Damage to Pivot (100%)
        float baseDmg = currentDamage * ownerStats.currentDamageMultiplier;
        ApplyDamage(pivot, baseDmg);

        // Play Thunder Strike Sound
        if (lightningData.thunderStrikeSound != null)
        {
            SoundManager.Instance.PlaySFX(lightningData.thunderStrikeSound, 0.2f);
        }

        // Spawn Sky->Pivot Thunder Strike Visual
        Vector2 skyPos = (Vector2)pivot.position + new Vector2(0, 10f);
        SpawnStrikeVisual(skyPos, pivot.position);

        // 3. Find all potential targets in Max Range (Optimization: OverlapCircle once)
        float maxScanRange = isTier3Unlocked ? lightningData.radius_R3 :
                             (isTier2Unlocked ? lightningData.radius_R2 : lightningData.radius_R1);

        Collider2D[] hits = Physics2D.OverlapCircleAll(pivot.position, maxScanRange);

        // 4. Clear lists for new calculation
        groupA.Clear(); groupB.Clear(); groupC.Clear();
        branchCounts.Clear();

        // 5. Sort enemies into Tiers (A, B, C) based on distance from Pivot
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy") || hit.transform == pivot) continue;
            if (!hit.gameObject.activeInHierarchy) continue;

            float dist = Vector2.Distance(pivot.position, hit.transform.position);

            if (dist <= lightningData.radius_R1)
            {
                groupA.Add(hit.transform);
            }
            else if (isTier2Unlocked && dist <= lightningData.radius_R2)
            {
                groupB.Add(hit.transform);
            }
            else if (isTier3Unlocked && dist <= lightningData.radius_R3)
            {
                groupC.Add(hit.transform);
            }
        }

        // --- Step 1: Pivot -> Group A ---
        if (groupA.Count == 0) return;

        float dmgA = baseDmg * lightningData.ratio_A;
        foreach (Transform targetA in groupA)
        {
            ApplyDamage(targetA, dmgA);
            SpawnChainVisual(pivot.position, targetA.position);

            // Initialize branch count for this source
            if (!branchCounts.ContainsKey(targetA)) branchCounts[targetA] = 0;
        }

        // --- Step 2: Group A -> Group B ---
        if (isTier2Unlocked && groupB.Count > 0)
        {
            List<Transform> hitTargetsB = ConnectTiers(groupA, groupB, baseDmg * lightningData.ratio_B);

            // --- Step 3: Group B -> Group C ---
            if (isTier3Unlocked && groupC.Count > 0 && hitTargetsB.Count > 0)
            {
                foreach (var sourceB in hitTargetsB) branchCounts[sourceB] = 0;
                ConnectTiers(hitTargetsB, groupC, baseDmg * lightningData.ratio_C);
            }
        }
    }

    private List<Transform> ConnectTiers(List<Transform> sources, List<Transform> targets, float damage)
    {
        List<Transform> successfulHits = new List<Transform>();

        if (lightningData.chainSound != null)
        {
            SoundManager.Instance.PlaySFX(lightningData.chainSound, 0.1f);
        }

        foreach (Transform target in targets)
        {
            Transform bestSource = null;
            float closestDist = Mathf.Infinity;

            foreach (Transform source in sources)
            {
                // Skip if source is full
                if (branchCounts.ContainsKey(source) && branchCounts[source] >= currentMaxBranches)
                    continue;

                float d = Vector2.Distance(source.position, target.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    bestSource = source;
                }
            }

            if (bestSource != null)
            {
                ApplyDamage(target, damage);
                SpawnChainVisual(bestSource.position, target.position);
                branchCounts[bestSource]++;
                successfulHits.Add(target);
            }
        }
        return successfulHits;
    }

    private void ApplyDamage(Transform target, float amount)
    {
        StatsController stats = target.GetComponent<StatsController>();
        if (stats != null)
        {
            stats.TakeDamage(amount);
        }
    }

    private void SpawnChainVisual(Vector2 start, Vector2 end)
    {
        if (lightningData.chainVisualPrefab == null) return;

        GameObject visObj = PoolManager.Instance.GetFromPool(lightningData.chainVisualPrefab.name);
        if (visObj != null)
        {
            LightningChainVisual script = visObj.GetComponent<LightningChainVisual>();
            if (script != null)
            {
                script.Initialize(start, end);
            }
        }
    }

    private void SpawnStrikeVisual(Vector2 start, Vector2 end)
    {
        GameObject prefabToUse = lightningData.thunderStrikePrefab != null ?
                                 lightningData.thunderStrikePrefab : lightningData.chainVisualPrefab;

        if (prefabToUse == null) return;

        GameObject visObj = PoolManager.Instance.GetFromPool(prefabToUse.name);
        if (visObj != null)
        {
            LightningChainVisual script = visObj.GetComponent<LightningChainVisual>();
            if (script != null) script.Initialize(start, end);
        }
    }

    private Transform GetRandomEnemyOnScreen()
    {
        Vector2 min = CameraBoundsController.MinBounds;
        Vector2 max = CameraBoundsController.MaxBounds;

        Collider2D[] cols = Physics2D.OverlapAreaAll(min, max);

        List<Transform> enemies = new List<Transform>();
        foreach (var c in cols)
        {
            if (c.CompareTag("Enemy") && c.gameObject.activeInHierarchy)
            {
                enemies.Add(c.transform);
            }
        }

        if (enemies.Count > 0)
        {
            return enemies[Random.Range(0, enemies.Count)];
        }
        return null;
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: ApplyStats(lightningData.level2_DamageBonus, lightningData.level2_CooldownReduction, lightningData.level2_BranchIncrease, lightningData.level2_UnlockTier2, lightningData.level2_UnlockTier3); break;
            case 3: ApplyStats(lightningData.level3_DamageBonus, lightningData.level3_CooldownReduction, lightningData.level3_BranchIncrease, lightningData.level3_UnlockTier2, lightningData.level3_UnlockTier3); break;
            case 4: ApplyStats(lightningData.level4_DamageBonus, lightningData.level4_CooldownReduction, lightningData.level4_BranchIncrease, lightningData.level4_UnlockTier2, lightningData.level4_UnlockTier3); break;
            case 5: ApplyStats(lightningData.level5_DamageBonus, lightningData.level5_CooldownReduction, lightningData.level5_BranchIncrease, lightningData.level5_UnlockTier2, lightningData.level5_UnlockTier3); break;
            case 6: ApplyStats(lightningData.level6_DamageBonus, lightningData.level6_CooldownReduction, lightningData.level6_BranchIncrease, lightningData.level6_UnlockTier2, lightningData.level6_UnlockTier3); break;
            case 7: ApplyStats(lightningData.level7_DamageBonus, lightningData.level7_CooldownReduction, lightningData.level7_BranchIncrease, lightningData.level7_UnlockTier2, lightningData.level7_UnlockTier3); break;
            case 8: ApplyStats(lightningData.level8_DamageBonus, lightningData.level8_CooldownReduction, lightningData.level8_BranchIncrease, lightningData.level8_UnlockTier2, lightningData.level8_UnlockTier3); break;
            case 9: ApplyStats(lightningData.level9_DamageBonus, lightningData.level9_CooldownReduction, lightningData.level9_BranchIncrease, lightningData.level9_UnlockTier2, lightningData.level9_UnlockTier3); break;
        }

        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    private void ApplyStats(float dmg, float cooldown, int branch, bool t2, bool t3)
    {
        currentDamage += dmg;
        currentMaxBranches += branch;

        // Apply cooldown as flat reduction
        currentAttackCooldown -= cooldown;

        if (t2) isTier2Unlocked = true;
        if (t3) isTier3Unlocked = true;
    }
}