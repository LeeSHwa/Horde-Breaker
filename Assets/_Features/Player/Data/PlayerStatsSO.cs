// No changes needed. This class is already correct.
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsSO : CharacterStatsSO
{
    [Header("Leveling (Player-Only)")]
    public int[] expToNextLevel = new int[] {
    5, 10, 15, 20, 25, 30, 40, 55, 70, 90,
    110, 135, 160, 190, 220, 255, 290, 330, 370, 415,
    460, 510, 560, 610, 660, 710, 760, 810, 860, 910,
    990, 1070, 1150, 1230, 1310, 1390, 1470, 1550, 1630, 1710
};

    [Header("Pickup Stats (Player-Only)")]
    // The base magnet radius for ExpOrbs
    public float basePickupRadius = 4f;

    [Header("Critical Stats")]
    [Tooltip("Base chance to land a critical hit (0.0 to 1.0). e.g., 0.05 = 5%")]
    public float baseCritChance = 0f; // Default 0%

    [Tooltip("Damage multiplier on critical hit. e.g., 1.5 = 150% damage")]
    public float baseCritMultiplier = 1.5f; // Default 150% damage

}