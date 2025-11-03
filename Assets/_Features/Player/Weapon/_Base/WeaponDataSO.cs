using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Stats/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Common Stats")]
    public float baseDamage = 10f;
    public float baseAttackCooldown = 0.5f;
    public float knockback = 5f;

    [Header("Description")]
    public string weaponName = "Weapon";
    [TextArea]
    public string weaponDescription = "Weapon description.";
}