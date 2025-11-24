using UnityEngine;

// [MODIFIED] CreateAssetMenu path updated to reflect new name
[CreateAssetMenu(fileName = "NewMoonData", menuName = "Stats/Skill Data/Moon Data")]
public class MoonDataSO : SkillDataSO // [MODIFIED] Class name changed from RotatingSkillDataSO
{
    [Header("Rotating Skill Specific")] // [NOTE] Header kept for clarity
    public GameObject projectilePrefab;
    public float baseDuration = 5f;
    public float baseRotationSpeed = 100f;
    public float baseRotationRadius = 1.5f;

    [Header("Rotating Skill Level Up Stats")] // [NOTE] Header kept for clarity
    public float level2_DamageIncrease = 10f;
    public int level3_ProjectileCount = 2;
    public float level4_RotationSpeedIncrease = 20f;

    // [MODIFIED] Changed to a "Mastery" level, keeping Radius and adding Projectile Count
    [Header("Level 5 (Mastery)")] // [NEW] Added Header for clarity
    [Tooltip("Level 5: Additional radius increase")] // [NEW] Added Tooltip
    public float level5_RadiusIncrease = 0.5f; // This field was already here
    [Tooltip("Level 5: Set the total number of projectiles")] // [NEW] Added new field
    public int level5_ProjectileCount = 3;
}