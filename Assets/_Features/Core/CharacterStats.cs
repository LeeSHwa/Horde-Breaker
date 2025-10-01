using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Core Stats")]
    public float maxHealth = 100f; // The maximum health of the character.
    public float moveSpeed = 5f;   // The movement speed of the character.
    // public float attackPower = 10f;

    [HideInInspector]
    public float currentHealth; // The current health.

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// It's used for initialization.
    /// </summary>
    void Awake()
    {
        // Initialize current health to the maximum health at the start.
        currentHealth = maxHealth;

        // Add this line to update the UI when the game starts.
        //UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    /// <summary>
    /// Reduces the character's health by the given damage amount.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamage(float damage)
    {
        if (damage < 0) return;

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // Add this line to update the UI when damage is taken.
        UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// Handles the death of the character.
    /// </summary>
    private void Die()
    {
        Debug.Log(transform.name + " has died.");

        // For now, just destroy the GameObject.
        Destroy(gameObject);
    }
}