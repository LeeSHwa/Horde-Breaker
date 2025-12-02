using UnityEngine;
using System.Collections; 

public class PlayerDash : MonoBehaviour
{
    public DashConfigSO dashConfig;
    public Material ghostMaterial;
    public float trailEffectLifetime = 0.5f;
    public float trailSpawnInterval = 0.03f;

    private int currentDashStacks;
    private float stackRechargeTimer;
    private bool isDashing = false;
    private bool isInvincible = false;
    private float dashMoveTimer;
    private float invincibilityTimer;
    private Vector2 dashDirection;

    private Rigidbody2D rb;
    private PlayerController playerController;
    private SpriteRenderer playerSpriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        currentDashStacks = Mathf.Max(0, Mathf.Min(dashConfig.startingDashStacks, dashConfig.maxDashStacks));

        if (UIManager.Instance != null)
        {
            UIManager.Instance.InitDashSlots(dashConfig.maxDashStacks);
        }
    }

    void Update()
    {
        HandleInput();
        HandleStackRecharge();
        HandleDashTimers();

        UpdateDashUI();
    }

    public void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashConfig.dashSpeed;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentDashStacks > 0)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        isInvincible = true;
        dashMoveTimer = dashConfig.dashMoveDuration;
        invincibilityTimer = dashConfig.invincibilityDuration;
        currentDashStacks--;

        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = playerController.LastMoveInput;
        }

        if (ghostMaterial != null)
        {
            StartCoroutine(SpawnTrailCoroutine());
        }
    }

    private void UpdateDashUI()
    {
        if (UIManager.Instance == null) return;

        float totalCharge = 0f;

        if (currentDashStacks >= dashConfig.maxDashStacks)
        {
            totalCharge = (float)dashConfig.maxDashStacks;
        }
        else
        {
            float progress = stackRechargeTimer / dashConfig.stackRechargeCooldown;
            totalCharge = currentDashStacks + progress;
        }
        UIManager.Instance.UpdateDashUI(totalCharge);

    }


    private IEnumerator SpawnTrailCoroutine()
    {
        // Get player sprite info *once* at the start
        Sprite sprite = playerSpriteRenderer.sprite;
        bool flipX = playerSpriteRenderer.flipX;
        int sortingOrder = playerSpriteRenderer.sortingOrder;
        string dashPoolTag = "DashEffect"; // The tag for the pool

        // Loop *while* the dash is active
        while (isDashing)
        {
            GameObject effectObj = PoolManager.Instance.GetFromPool(dashPoolTag);

            if (effectObj == null)
            {
                Debug.LogWarning("DashEffect pool is full or does not exist!!");
                break; // Stop the coroutine if pool is empty
            }

            // Set position to the player's *current* position
            effectObj.transform.position = transform.position;
            effectObj.transform.rotation = Quaternion.identity;

            DashEffect effectScript = effectObj.GetComponent<DashEffect>();
            if (effectScript != null)
            {
                effectScript.Initialize(
                    sprite,
                    flipX,
                    sortingOrder, // Will be set to (sortingOrder - 1) in DashEffect.cs
                    ghostMaterial,
                    trailEffectLifetime
                );
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(trailSpawnInterval);
        }
    }

    private void HandleStackRecharge()
    {
        if (currentDashStacks < dashConfig.maxDashStacks)
        {
            stackRechargeTimer += Time.deltaTime;
            if (stackRechargeTimer >= dashConfig.stackRechargeCooldown)
            {
                currentDashStacks++;
                stackRechargeTimer = 0f;
            }
        }
    }

    private void HandleDashTimers()
    {
        if (isDashing)
        {
            dashMoveTimer -= Time.deltaTime;
            if (dashMoveTimer <= 0)
            {
                isDashing = false;

                rb.linearVelocity = Vector2.zero;
            }
        }

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }
    public bool IsDashing() { return isDashing; }
    public bool IsInvincible() { return isInvincible; }
    public int GetCurrentStacks() { return currentDashStacks; }
    public float GetCooldownProgress()
    {
        if (currentDashStacks == dashConfig.maxDashStacks) return 1f;
        return stackRechargeTimer / dashConfig.stackRechargeCooldown;
    }
}