using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Stats (SO)")]
    public DashConfigSO dashConfig; 


    private int currentDashStacks;
    private float stackRechargeTimer;

  
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

        

        currentDashStacks = dashConfig.maxDashStacks;
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
        if (Input.GetKeyDown(KeyCode.Space) && currentDashStacks > 0 )
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

        // 방향 결정
        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = playerController.LastMoveInput;
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
        // 1. 대쉬 이동 타이머
        if (isDashing)
        {
            dashMoveTimer -= Time.deltaTime;
            if (dashMoveTimer <= 0)
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;
            }
        }

        // 2. 무적 타이머
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false; // 무적 플래그 OFF
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


    // --- 다른 스크립트가 참조할 함수들 ---

    public bool IsDashing()
    {
        return isDashing; // PlayerController가 참조
    }

    public bool IsInvincible()
    {
        return isInvincible; // EnemyContactAttack이 참조
    }

    public int GetCurrentStacks()
    {
        return currentDashStacks;
    }

    public float GetCooldownProgress()
    {
        if (currentDashStacks == dashConfig.maxDashStacks) return 1f;
        return stackRechargeTimer / dashConfig.stackRechargeCooldown;
    }
}