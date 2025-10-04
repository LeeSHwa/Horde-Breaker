using UnityEngine;

// abstract: This means this script is a 'blueprint' that won't be attached to an object directly. 
// Instead, other weapon scripts will inherit from it.
public abstract class Weapon : MonoBehaviour
{
    [Header("Common Stats")]
    public float damage = 10f;
    public float attackCooldown = 0.5f;
    public float knockback = 5f;

    protected float lastAttackTime;

    // The common 'attack attempt' logic that all child weapons will use.
    public virtual void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    // The 'actual attack' logic. Every child weapon MUST implement this in its own way.
    protected abstract void PerformAttack();
}
