// FileName: CharacterStats.cs
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // A header for organization in the Inspector.
    [Header("Core Stats")]
    // The maximum health of the character.
    public float maxHealth = 100f;

    // The character's current movement speed. This can be changed in real-time by buffs/debuffs.
    public float moveSpeed = 5f;

    // This is the character's 'pure base' movement speed, unaffected by buffs/debuffs. (Read-only)
    public float BaseMoveSpeed { get; private set; }

    // Hides this variable in the Inspector.
    [HideInInspector]
    // The character's current health.
    public float currentHealth;

    // This function is called once when the script instance is being loaded. It's used for initialization.
    void Awake()
    {
        // Initialize current health to the maximum health at the start.
        currentHealth = maxHealth;

        // When the game starts, store the initial moveSpeed value from the Inspector into BaseMoveSpeed just once to serve as the initial baseline.
        BaseMoveSpeed = moveSpeed;
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
        // If the damage value is less than 0, do nothing.
        if (damage < 0) return;

        // Subtract the damage from the current health.
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If health drops to 0 or below
        if (currentHealth <= 0)
        {
            // Clamp the health to 0 so it doesn't go negative.
            currentHealth = 0;

            // Call the death handling function.
            Die();
        }
    }

    // Handles the death of the character.
    private void Die()
    {
        Debug.Log(transform.name + " has died.");

        // For now, just destroy the GameObject.
        Destroy(gameObject);
    }
}