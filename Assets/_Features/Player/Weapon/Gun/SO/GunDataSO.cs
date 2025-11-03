using UnityEngine;

// This [CreateAssetMenu] is the important part.
[CreateAssetMenu(fileName = "NewGunData", menuName = "Stats/Weapon Data/Gun Data")]
public class GunDataSO : WeaponDataSO
{
    [Header("Gun Specific")]
    public GameObject bulletPrefab;

    [Header("Gun Level Up Stats")]
    [Header("Level 2")]
    public float level2_DamageBonus = 5f;
    [Header("Level 3")]
    public int level3_ProjectileCount = 2;
    [Header("Level 4")]
    public float level4_CooldownReduction = 0.1f;
    [Header("Level 5 (Max)")]
    public bool level5_UnlocksPenetration = true;
}