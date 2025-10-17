// FileName: CharacterStats.cs
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // A header for organization in the Inspector.
    [Header("Core Stats")]
    public float maxHP = 100f;
    public float moveSpeed = 5f;
    public float BaseMoveSpeed { get; private set; } // ? 

    // Hides this variable in the Inspector.
    [HideInInspector]
    // The character's current health.
    public float currentHP;

    // This function is called once when the script instance is being loaded. It's used for initialization.
    void Awake()
    {
        // Initialize current health to the maximum health at the start.
        currentHP = maxHP;

        // When the game starts, store the initial moveSpeed value from the Inspector into BaseMoveSpeed just once to serve as the initial baseline.
        BaseMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        //    if (UIManager.Instance != null)
        //    {
        //    }
        if (gameObject.CompareTag("Player"))
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)maxHP);
            StartCoroutine(TakePeriodicDamageRoutine());
        }
    }


    private IEnumerator TakePeriodicDamageRoutine()
    {
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (i >= 5) break;
            TakeDamage(10f);
            i++;
        }
    }

    public void UpdateBaseMoveSpeed(float newBaseSpeed)
    {

        BaseMoveSpeed = newBaseSpeed;

    }


    // Reduces the character's health by the given damage amount.

    public void TakeDamage(float damage)
    {
        // If the damage value is less than 0, do nothing.
        if (damage < 0) return;

        // Subtract the damage from the current health.
        currentHP -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)maxHP);
        }

        // If health drops to 0 or below
        if (currentHP <= 0)
        {
            // Clamp the health to 0 so it doesn't go negative.
            currentHP = 0;

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