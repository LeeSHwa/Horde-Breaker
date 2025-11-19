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

    // Helper Lists
    private List<Transform> groupA = new List<Transform>();
    private List<Transform> groupB = new List<Transform>();
    private List<Transform> groupC = new List<Transform>();

    // Tracks branches
    private Dictionary<Transform, int> branchCounts = new Dictionary<Transform, int>();

    protected override void Awake()
    {
        base.Awake();
        if (skillData is LightningDataSO)
        {
            lightningData = (LightningDataSO)skillData;
        }
        else
        {
            Debug.LogError("LightningSkill: Wrong DataSO assigned!");
        }
        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();
        isTier2Unlocked = false;
        isTier3Unlocked = false;
        currentMaxBranches = lightningData.maxBranches;
    }

    protected override void PerformAttack()
    {
        // 1. Find Pivot
        Transform pivot = GetRandomEnemyOnScreen();
        if (pivot == null) return;

        // 2. Apply Damage to Pivot
        float baseDmg = currentDamage * ownerStats.currentDamageMultiplier;
        ApplyDamage(pivot, baseDmg);

        // [MODIFIED] Removed Player->Pivot visual.
        // [NEW] Spawn Sky->Pivot Thunder Strike
        Vector2 skyPos = (Vector2)pivot.position + new Vector2(0, 10f); // 10 units above enemy
        SpawnStrikeVisual(skyPos, pivot.position);

        // 3. Find targets in Max Range
        float maxScanRange = isTier3Unlocked ? lightningData.radius_R3 :
                             (isTier2Unlocked ? lightningData.radius_R2 : lightningData.radius_R1);

        Collider2D[] hits = Physics2D.OverlapCircleAll(pivot.position, maxScanRange);

        // 4. Sort into Tiers based on distance from Pivot
        groupA.Clear(); groupB.Clear(); groupC.Clear();
        branchCounts.Clear();

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy") || hit.transform == pivot) continue;
            if (!hit.gameObject.activeInHierarchy) continue;

            // R1, R2, R3 are circles centered on the Pivot
            float dist = Vector2.Distance(pivot.position, hit.transform.position);

            if (dist <= lightningData.radius_R1) groupA.Add(hit.transform);
            else if (isTier2Unlocked && dist <= lightningData.radius_R2) groupB.Add(hit.transform);
            else if (isTier3Unlocked && dist <= lightningData.radius_R3) groupC.Add(hit.transform);
        }

        // --- Step 1: Pivot -> Group A ---
        if (groupA.Count == 0) return;

        float dmgA = baseDmg * lightningData.ratio_A;
        foreach (Transform targetA in groupA)
        {
            ApplyDamage(targetA, dmgA);
            SpawnChainVisual(pivot.position, targetA.position); // Chain Visual

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

        foreach (Transform target in targets)
        {
            Transform bestSource = null;
            float closestDist = Mathf.Infinity;

            foreach (Transform source in sources)
            {
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
                SpawnChainVisual(bestSource.position, target.position); // Chain Visual
                branchCounts[bestSource]++;
                successfulHits.Add(target);
            }
        }
        return successfulHits;
    }

    private void ApplyDamage(Transform target, float amount)
    {
        StatsController stats = target.GetComponent<StatsController>();
        if (stats != null) stats.TakeDamage(amount);
    }

    private void SpawnChainVisual(Vector2 start, Vector2 end)
    {
        if (lightningData.chainVisualPrefab == null) return;

        GameObject visObj = PoolManager.Instance.GetFromPool(lightningData.chainVisualPrefab.name);
        if (visObj != null)
        {
            LightningChainVisual script = visObj.GetComponent<LightningChainVisual>();
            if (script != null) script.Initialize(start, end);
        }
    }

    private void SpawnStrikeVisual(Vector2 start, Vector2 end)
    {
        // If specific strike prefab is missing, fallback to chain prefab
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
            if (c.CompareTag("Enemy") && c.gameObject.activeInHierarchy) enemies.Add(c.transform);
        }

        if (enemies.Count > 0) return enemies[Random.Range(0, enemies.Count)];
        return null;
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: currentDamage += lightningData.level2_DamageIncrease; break;
            case 3: isTier2Unlocked = lightningData.level3_UnlockTier2; break;
            case 4:
                currentAttackCooldown -= lightningData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                isTier3Unlocked = lightningData.level5_UnlockTier3;
                currentDamage += lightningData.level5_BonusDamage;
                break;
        }
    }
}