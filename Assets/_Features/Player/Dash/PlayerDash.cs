using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public DashConfigSO dashConfig;

    public GameObject pathPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 0);

    public float effectExtraLifetime = 0.3f;

    private int currentDashStacks;
    private float stackRechargeTimer;
    private GameObject currentPathEffect;

    private bool isDashing = false;
    private bool isInvincible = false;
    private float dashMoveTimer;
    private float invincibilityTimer;
    private Vector2 dashDirection;

    private Rigidbody2D rb;
    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        currentDashStacks = Mathf.Max(0, Mathf.Min(dashConfig.startingDashStacks, dashConfig.maxDashStacks));
    }

    void Update()
    {
        HandleInput();
        HandleStackRecharge();
        HandleDashTimers();
    }

    void FixedUpdate()
    {
        HandleDashMovement();
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

        if (pathPrefab != null)
        {
            SpawnPathEffect();
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

    private void SpawnPathEffect()
    {
        GameObject effectObj = Instantiate(pathPrefab, transform);

        effectObj.transform.localPosition = effectOffset;
        effectObj.transform.localRotation = Quaternion.identity;

        PathEffect effectScript = effectObj.GetComponent<PathEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(dashDirection);
        }
        else
        {
            effectObj.transform.right = dashDirection;
        }

        currentPathEffect = effectObj;
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

                if (currentPathEffect != null)
                {
                    currentPathEffect.transform.SetParent(null);

                    PathEffect effectScript = currentPathEffect.GetComponent<PathEffect>();
                    if (effectScript != null)
                    {
                        effectScript.StartFadeOut(effectExtraLifetime);
                    }
                    else
                    {
                        Destroy(currentPathEffect, effectExtraLifetime);
                    }

                    currentPathEffect = null;
                }
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

    private void HandleDashMovement()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashConfig.dashSpeed;
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