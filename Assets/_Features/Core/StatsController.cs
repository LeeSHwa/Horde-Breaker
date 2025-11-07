using System.Collections;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [Header("Data Source")]
    public CharacterStatsSO baseStats;

    [Header("Runtime Stats")]
    [HideInInspector]
    public float currentHP;
    public float currentMoveSpeed;
    public float currentDamageMultiplier;

    private bool isDead = false;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        // Get components
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Initialize stats (especially for non-pooled objects like Player)
        InitializeStats();
    }

    void Start()
    {
        // [FIX] Move the UI logic here.
        // Start() runs AFTER all Awake() methods (including UIManager.Awake()) are complete.
        if (gameObject.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
            }
            else
            {
                Debug.LogError("UIManager.Instance is STILL null, even in Start()! Check the UIManager object.");
            }
        }
    }

    void OnEnable()
    {
        // ... (Re-enable components: isDead, col, rb) ...
        InitializeStats(); // OK to re-initialize stats

        // [FIX] We also need to update UI when re-enabled (e.g. Player respawns)
        // But we must check if UIManager.Instance is ready.
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
        }
    }

    public void InitializeStats()
    {
        if (baseStats == null)
        {
            // (This might be the *other* cause, see Error 2 fix)
            Debug.LogError(gameObject.name + " is missing BaseStats SO!");
            return; // Stop here if baseStats is null
        }

        currentHP = baseStats.baseMaxHealth;
        currentMoveSpeed = baseStats.baseMoveSpeed;
        currentDamageMultiplier = baseStats.baseDamageMultiplier;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage < 0) return;

        currentHP -= damage;

        GameObject popup = PoolManager.Instance.GetFromPool("DamageText");
        if (popup != null)
        {
            popup.transform.position = transform.position + new Vector3(0, 0.5f, 0);

            DamagePopup dp = popup.GetComponent<DamagePopup>();
            if (dp != null)
            {
                dp.Setup(damage, false); // false = isCritical (dummy for now)
            }
        }

        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (gameObject.CompareTag("Player"))
        {
            UIManager.Instance.UpdateHP((int)currentHP, (int)baseStats.baseMaxHealth);
        }

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;
            Die();
        }
    }


    private void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        if (col != null) col.enabled = false;

        StartCoroutine(DieAndDisable(baseStats.deathAnimationLength));
    }

    private IEnumerator DieAndDisable(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}