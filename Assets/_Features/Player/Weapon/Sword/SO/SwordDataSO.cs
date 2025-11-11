using UnityEngine;

// Defines the ScriptableObject data asset for the Sword weapon.
[CreateAssetMenu(fileName = "NewSwordData", menuName = "Stats/Weapon Data/Sword Data")]
public class SwordDataSO : WeaponDataSO
{
    [Header("Sword Specific")]
    // The prefab to spawn for the attack hitbox (e.g., the slash object).
    public GameObject hitboxPrefab;

    // The base size (area) of the sword attack at Level 1.
    public float baseArea = 1f;

    // A global multiplier to adjust the final size (area) of the attack.
    // This is used for balancing the overall scale.
    public float sizeMultiplier = 1.0f;

    [Header("Sword Level Up Stats")]
    // The multiplier for the area increase at Level 3 (e.g., 1.2f = +20% area).
    public float level3_AreaIncrease = 1.2f;

    // The amount of 'pierce' (ability to hit multiple enemies) gained at Level 5.
    public int level5_PierceIncrease = 1;
}