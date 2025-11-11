using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Stats/Enemy Stats Data")]
public class EnemyStatsSO : CharacterStatsSO
{
    [Header("Enemy Specific")]
    public float contactDamage = 10f;

    [Header("EXP Drop (Enemy-Only)")]
    public int expValue = 1;
}