using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldData", menuName = "Stats/Skill Data/Shield Data")]
public class ShieldDataSO : SkillDataSO
{
    [Header("Shield Specific Base Stats")]
    // The visual prefab (translucent circle) to spawn around the player.
    public GameObject shieldVisualPrefab;

    // Initial number of shield layers (stacks).
    public int baseMaxStacks = 1;

    // Time (in seconds) the player remains invulnerable after a shield breaks.
    public float baseInvulnTime = 1.0f;

    [Header("Audio")]
    public AudioClip shieldRechargeSound;
    public AudioClip shieldBreakSound;

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_CooldownReduction;
    public float level2_InvulnTimeBonus;
    public int level2_StackIncrease;
    public bool level2_UnlockGhostMode;
    public float level2_MoveSpeedBonus;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_CooldownReduction;
    public float level3_InvulnTimeBonus;
    public int level3_StackIncrease;
    public bool level3_UnlockGhostMode;
    public float level3_MoveSpeedBonus;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_CooldownReduction;
    public float level4_InvulnTimeBonus;
    public int level4_StackIncrease;
    public bool level4_UnlockGhostMode;
    public float level4_MoveSpeedBonus;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_CooldownReduction;
    public float level5_InvulnTimeBonus;
    public int level5_StackIncrease;
    public bool level5_UnlockGhostMode;
    public float level5_MoveSpeedBonus;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_CooldownReduction;
    public float level6_InvulnTimeBonus;
    public int level6_StackIncrease;
    public bool level6_UnlockGhostMode;
    public float level6_MoveSpeedBonus;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_CooldownReduction;
    public float level7_InvulnTimeBonus;
    public int level7_StackIncrease;
    public bool level7_UnlockGhostMode;
    public float level7_MoveSpeedBonus;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_CooldownReduction;
    public float level8_InvulnTimeBonus;
    public int level8_StackIncrease;
    public bool level8_UnlockGhostMode;
    public float level8_MoveSpeedBonus;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_CooldownReduction;
    public float level9_InvulnTimeBonus;
    public int level9_StackIncrease;
    public bool level9_UnlockGhostMode;
    public float level9_MoveSpeedBonus;
}
