using UnityEngine;

// A new data asset for our Fireball skill
[CreateAssetMenu(fileName = "NewFireballData", menuName = "Stats/Skill Data/Fireball Data")]
public class FireballDataSO : SkillDataSO // Inherits from the base SkillDataSO
{
    [Header("Fireball Specific")]
    public GameObject projectilePrefab; // The fireball prefab
    public float baseProjectileSpeed = 10f;
    public float baseProjectileLifetime = 3f; // Disappears after 3 seconds
    public float damageFalloffPercentage = 50f; // 50 = 50% damage after first hit

    [Header("Fireball Level Up Stats")]
    public float level2_DamageIncrease = 10f;
    public float level3_AreaIncrease = 0.2f; // 0.2 = +20% size
    public float level4_CooldownReduction = 0.2f; // 0.2s faster
    public int level5_ProjectileCount = 3;
    public float level5_SpreadAngle = 15f; // Angle between the 3 projectiles
}