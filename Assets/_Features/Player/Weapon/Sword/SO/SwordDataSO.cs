using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordData", menuName = "Stats/Weapon Data/Sword Data")]
public class SwordDataSO : WeaponDataSO
{
    [Header("Sword Specific")]
    [Tooltip("The time it takes to complete one swing animation.")]
    public float baseSwingDuration = 0.5f;

    [Tooltip("The base angle of the swing arc.")]
    public float baseAngle = 90f;

    [Tooltip("The base radius/length of the swing.")]
    public float baseAreaRadius = 1.5f;

    [Tooltip("Angle offset to make the swing look like 'over-the-top'. e.g., 45 degrees.")]
    public float swingStartOffset = 45f;

    [Header("Sword Level Up Stats")]
    [Header("Level 2 (Dmg + Speed)")]
    public float level2_DamageBonus = 10f;
    [Tooltip("Reduces swing duration, making it faster.")]
    public float level2_SpeedIncrease = 0.1f; // Swing duration 'reduction'

    [Header("Level 3 (Length)")]
    [Tooltip("Increases the radius/length of the swing.")]
    public float level3_AreaIncrease = 0.5f;  // Radius 'increase'

    [Header("Level 4 (Angle)")]
    [Tooltip("Increases the angle of the swing arc.")]
    public float level4_AngleIncrease = 30f;  // Angle 'increase'

    [Header("Level 5 (Projectile)")]
    [Tooltip("Number of swings required to launch one projectile.")]
    public int level5_AttacksPerProjectile = 5;

    [Tooltip("The crescent-shaped projectile prefab to spawn.")]
    public GameObject projectilePrefab;

    [Tooltip("Damage as a percentage of the base swing (e.g., 0.5 = 50%).")]
    public float projectileDamagePercent = 0.5f;

    [Tooltip("Knockback as a percentage of the base swing (e.g., 0.5 = 50%).")]
    public float projectileKnockbackPercent = 0.5f;

    [Tooltip("How long the projectile lasts in seconds.")]
    public float projectileLifetime = 3f;
}