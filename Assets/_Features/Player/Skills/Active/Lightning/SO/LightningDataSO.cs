using UnityEngine;

[CreateAssetMenu(fileName = "NewLightningData", menuName = "Stats/Skill Data/Lightning Data")]
public class LightningDataSO : SkillDataSO
{
    [Header("Lightning Visuals")]
    [Tooltip("Prefab for the chains between enemies (Thinner)")]
    public GameObject chainVisualPrefab;

    [Tooltip("Prefab for the initial sky-to-ground strike (maybe Thicker)")] 
    public GameObject thunderStrikePrefab;

    [Header("Connection Ranges (Radius from Pivot)")]
    public float radius_R1 = 4f;
    public float radius_R2 = 7f;
    public float radius_R3 = 10f;

    [Header("Damage Ratios (1.0 = 100%)")]
    public float ratio_A = 0.8f;
    public float ratio_B = 0.5f;
    public float ratio_C = 0.2f;

    [Header("Chain Settings")]
    public int maxBranches = 3;

    [Header("Level Up Stats")]
    [Header("Level 2 (Damage)")]
    public float level2_DamageIncrease = 10f;

    [Header("Level 3 (Unlock Tier 2)")]
    public bool level3_UnlockTier2 = true;

    [Header("Level 4 (Cooldown)")]
    public float level4_CooldownReduction = 0.5f;

    [Header("Level 5 (Unlock Tier 3 & Mastery)")]
    public bool level5_UnlockTier3 = true;
    public float level5_BonusDamage = 20f;
}