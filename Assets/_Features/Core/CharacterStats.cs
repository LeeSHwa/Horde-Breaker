// FileName: CharacterStats.cs
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // A header for organization in the Inspector.
    [Header("Core Stats")]
    // The maximum health of the character.
    public float maxHealth = 100f;

    // The character's base attack damage. (THIS IS THE NEW LINE)
    public float attackDamage = 10f; // add damage stat 

    // The character's current movement speed. This can be changed in real-time by buffs/debuffs.
    public float moveSpeed = 5f;

    // This is the character's 'pure base' movement speed, unaffected by buffs/debuffs. (Read-only)
    public float BaseMoveSpeed { get; private set; }

    // Hides this variable in the Inspector.
    [HideInInspector]
    // The character's current health.
    public float currentHealth;

    // --- Start of added code ---
    private bool isDead = false; // Added variable to track death state
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    // --- End of added code ---

    // This function is called once when the script instance is being loaded. It's used for initialization.
    void Awake()
    {
        // Initialize current health to the maximum health at the start.
        currentHealth = maxHealth;

        // When the game starts, store the initial moveSpeed value from the Inspector into BaseMoveSpeed just once to serve as the initial baseline.
        BaseMoveSpeed = moveSpeed;

        // --- Start of added code ---
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        // --- End of added code ---
    }

    // --- Function newly added ---
    // Updates the character's permanent base movement speed due to leveling up, equipping items, etc.
    public void UpdateBaseMoveSpeed(float newBaseSpeed)
    {
        // Permanently changes the base speed to the new value.
        BaseMoveSpeed = newBaseSpeed;

        // --- CORE MODIFICATION ---
        // The responsibility of checking for active effects and adjusting moveSpeed
        // has been moved to the SpeedEffectController.
        // This script no longer needs to check for TemporarySpeedEffect.
        // The controller will automatically recalculate the speed.

        //
        // Note: We don't directly set `moveSpeed = BaseMoveSpeed;` here because a speed effect might be active.
        // The SpeedEffectController is responsible for applying the correct speed,
        // so we just update the base value and let the controller handle the rest.
    }


    // Reduces the character's health by the given damage amount.
    public void TakeDamage(float damage)
    {
        // If already dead, do nothing.
        if (isDead) return;

        // If the damage value is less than 0, do nothing.
        if (damage < 0) return;

        // Subtract the damage from the current health.
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If health drops to 0 or below
        if (currentHealth <= 0)
        {
            // Set state to dead to prevent this from being called again.
            isDead = true;

            // Clamp the health to 0 so it doesn't go negative.
            currentHealth = 0;

            // Call the death handling function.
            Die();
        }
    }

    // Handles the death of the character.
    private void Die()
    {
        // --- Start of added code ---
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        if (col != null)
        {
            col.enabled = false;
        }

        float deathAnimationLength = 0.5f; // ? Adjust to match the animation's length!
        Destroy(gameObject, deathAnimationLength);
        // --- End of added code ---
    }
}