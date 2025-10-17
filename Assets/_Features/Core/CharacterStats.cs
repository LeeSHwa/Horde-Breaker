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
        currentHP = maxHP;

        // When the game starts, store the initial moveSpeed value from the Inspector into BaseMoveSpeed just once to serve as the initial baseline.
        BaseMoveSpeed = moveSpeed;

        // --- Start of added code ---
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        // --- End of added code ---
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


        //
        // Note: We don't directly set `moveSpeed = BaseMoveSpeed;` here because a speed effect might be active.
        // The SpeedEffectController is responsible for applying the correct speed,
        // so we just update the base value and let the controller handle the rest.
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
        // If already dead, do nothing.
        if (isDead) return;

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
            // Set state to dead to prevent this from being called again.
            isDead = true;

            // Clamp the health to 0 so it doesn't go negative.
            currentHP = 0;

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