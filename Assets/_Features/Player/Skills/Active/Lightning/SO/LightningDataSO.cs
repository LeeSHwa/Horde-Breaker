using UnityEngine;

[CreateAssetMenu(fileName = "NewLightningData", menuName = "Stats/Skill Data/Lightning Data")]
public class LightningDataSO : SkillDataSO
{
    [Header("Lightning Specific Base Stats")]
    public GameObject chainVisualPrefab;
    public GameObject thunderStrikePrefab;

    public float radius_R1 = 4f;
    public float radius_R2 = 7f;
    public float radius_R3 = 10f;

    public float ratio_A = 0.8f;
    public float ratio_B = 0.5f;
    public float ratio_C = 0.2f;

    public int maxBranches = 3;

    [Header("Audio")]
    public AudioClip thunderStrikeSound;
    public AudioClip chainSound;

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_CooldownReduction;
    public int level2_BranchIncrease;
    public bool level2_UnlockTier2;
    public bool level2_UnlockTier3;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_CooldownReduction;
    public int level3_BranchIncrease;
    public bool level3_UnlockTier2;
    public bool level3_UnlockTier3;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_CooldownReduction;
    public int level4_BranchIncrease;
    public bool level4_UnlockTier2;
    public bool level4_UnlockTier3;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_CooldownReduction;
    public int level5_BranchIncrease;
    public bool level5_UnlockTier2;
    public bool level5_UnlockTier3;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_CooldownReduction;
    public int level6_BranchIncrease;
    public bool level6_UnlockTier2;
    public bool level6_UnlockTier3;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_CooldownReduction;
    public int level7_BranchIncrease;
    public bool level7_UnlockTier2;
    public bool level7_UnlockTier3;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_CooldownReduction;
    public int level8_BranchIncrease;
    public bool level8_UnlockTier2;
    public bool level8_UnlockTier3;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_CooldownReduction;
    public int level9_BranchIncrease;
    public bool level9_UnlockTier2;
    public bool level9_UnlockTier3;
}