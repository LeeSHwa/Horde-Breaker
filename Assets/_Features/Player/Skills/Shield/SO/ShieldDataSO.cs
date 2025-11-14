using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldData", menuName = "Stats/Skill Data/Shield Data")]
public class ShieldDataSO : SkillDataSO
{
    [Header("Shield Specific")]
    // The visual prefab (translucent circle) to spawn around the player.
    public GameObject shieldVisualPrefab;

    // Initial number of shield layers (stacks).
    public int baseMaxStacks = 1;

    // Time (in seconds) the player remains invulnerable after a shield breaks.
    public float baseInvulnTime = 1.0f;

    [Header("Level Up Stats")]
    [Header("Level 2 (Max Stacks)")]
    // Amount to increase max stacks by (e.g., 2 means Total = 1 + 2 = 3).
    public int level2_StackIncrease = 2;

    [Header("Level 3 (Invuln Time)")]
    // Amount to increase invulnerability time by.
    public float level3_InvulnTimeIncrease = 2.0f;

    [Header("Level 4 (Recharge Speed)")]
    // Amount to reduce the recharge cooldown by (seconds).
    public float level4_CooldownReduction = 15f;

    [Header("Level 5 (Ghost Mode)")]
    // Unlocks the ability to pass through enemies and gain speed.
    public bool level5_UnlockGhostMode = true;
    // Percentage of speed increase during Ghost Mode (e.g., 0.5 = +50% speed).
    public float level5_MoveSpeedBonus = 0.5f;
}
