// No changes needed. This class is already correct.
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsSO : CharacterStatsSO
{
    [Header("Leveling (Player-Only)")]
    public int[] expToNextLevel;

    [Header("Pickup Stats (Player-Only)")]
    // The base magnet radius for ExpOrbs
    public float basePickupRadius = 4f;
}