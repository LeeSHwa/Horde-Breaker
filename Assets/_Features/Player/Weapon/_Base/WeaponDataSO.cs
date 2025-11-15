using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Stats/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Common Stats")]
    public float baseDamage = 10f;
    public float baseAttackCooldown = 0.5f;
    public float knockback = 5f;

    [Header("UI & Leveling")]
    public string weaponName = "Weapon";
    public Sprite icon;
    public int maxLevel = 5;

    [TextArea(3, 5)]
    public string weaponDescription = "Weapon description.";

    // A list to hold descriptions for levels 2, 3, 4, 5+
    public List<string> levelDescriptions;
}