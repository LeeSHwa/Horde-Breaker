using UnityEngine;

[CreateAssetMenu(fileName = "NewHellfireData", menuName = "Stats/Skill Data/Hellfire Data")]
public class HellfireDataSO : SkillDataSO
{
    [Header("Missile Specific")]
    public GameObject missilePrefab;
    public float missileSpeed = 15f;
    public float searchRadius = 20f; // Range to find enemies

    [Header("Level Up Stats")]
    [Header("Level 2 (Damage)")]
    public float level2_DamageIncrease = 10f;

    [Header("Level 3 (Double Shot)")]
    public int level3_ProjectileCount = 2; // Total 2 missiles

    [Header("Level 4 (Cooldown)")]
    public float level4_CooldownReduction = 0.5f;

    [Header("Level 5 (Triple Shot)")]
    public int level5_ProjectileCount = 3; // Total 3 missiles
}