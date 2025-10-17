using UnityEngine;

// This is the base blueprint for all weapons.
public abstract class Weapon : MonoBehaviour
{
    [Header("Common Stats")]
    [Tooltip("Damage dealt per hit.")]
    public float damage = 10f;
    [Tooltip("Cooldown time between attacks in seconds.")]
    public float attackCooldown = 0.5f;
    [Tooltip("Force applied to enemies on hit.")]
    public float knockback = 5f;

    [Header("References")]
        [Tooltip("The point from which the weapon attacks originate.")]
    public Transform aim;

    public float offset = 1.0f;

    protected float lastAttackTime; // The time of the last attack.

    // A virtual method that can be overridden by child classes.
    // This method checks the cooldown and initiates the attack.
    public virtual void TryAttack(Vector2 aimDirection)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {

            aim.position = (Vector2)transform.position + aimDirection * offset;

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aim.rotation = Quaternion.Euler(0, 0, angle);

            PerformAttack(aimDirection);
            lastAttackTime = Time.time;
        }
    }
    

    // An abstract method that MUST be implemented by child classes.
    // This defines the actual attack behavior (e.g., swinging, shooting).
    protected abstract void PerformAttack(Vector2 aimDirection);

}

