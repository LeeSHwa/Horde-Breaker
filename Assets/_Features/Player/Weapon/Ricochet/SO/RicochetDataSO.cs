using UnityEngine;

[CreateAssetMenu(fileName = "RicochetData", menuName = "Stats/Weapon Data/RicochetData")]
public class RicochetDataSO : WeaponDataSO
{
    [Header("Ricochet Base Stats (Lv.1)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public int baseBounces = 1;
    public float bounceRange = 8f;
    public float lifetime = 3f;

    // Note: Base Damage(6) and Base Cooldown(0.6) are set in the parent WeaponDataSO fields.

    [Header("Level Up Stats")]

    [Header("Level 2")]
    public float level2_DamageBonus = 2f;

    [Header("Level 3")]
    [Tooltip("Percentage reduction (e.g., 10 = 10%)")]
    public float level3_CooldownReduction = 10f;
    public int level3_BounceIncrease = 1;

    [Header("Level 4")]
    public float level4_DamageBonus = 4f;

    [Header("Level 5")]
    public int level5_ProjectileCountIncrease = 1;

    [Header("Level 6")]
    [Tooltip("Percentage reduction (e.g., 10 = 10%)")]
    public float level6_CooldownReduction = 10f;
    public int level6_BounceIncrease = 1;

    [Header("Level 7")]
    public float level7_DamageBonus = 10f;

    [Header("Level 8")]
    public int level8_ProjectileCountIncrease = 1;

    [Header("Level 9")]
    public int level9_ProjectileCountIncrease = 1;
}