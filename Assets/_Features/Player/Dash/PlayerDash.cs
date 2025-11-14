using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Stats (SO)")]
    public DashConfigSO dashConfig;

    [Header("대쉬 이펙트")]
    public GameObject pathPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 0);

    // ▼▼▼ [추가] 님이 요청하신 "추가 생존 시간" 변수입니다! ▼▼▼
    [Header("이펙트가 대쉬 후 추가로 남는 시간(초)")]
    public float effectExtraLifetime = 0.3f;
    // ▲▲▲

    private int currentDashStacks;
    private float stackRechargeTimer;
    private GameObject currentPathEffect; // 이펙트 저장용


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
        // 1. 플레이어의 '자식'으로 생성 (따라다니게)
        GameObject effectObj = Instantiate(pathPrefab, transform);

        effectObj.transform.localPosition = effectOffset;
        effectObj.transform.localRotation = Quaternion.identity;

        // 2. 방향 설정 (꺽쇠<> 사용!)
        PathEffect effectScript = effectObj.GetComponent<PathEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(dashDirection);
        }
        else
        {
            effectObj.transform.right = dashDirection;
        }

        // 3. 이펙트 저장
        currentPathEffect = effectObj;
    }

    private void HandleDashTimers()
    {
        // 1. 대쉬 이동 타이머
        if (isDashing)
        {
            dashMoveTimer -= Time.deltaTime;
            if (dashMoveTimer <= 0) // ▼▼▼ 대쉬가 "끝나는" 순간! ▼▼▼
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;

                // [수정] 이펙트 처리 로직 변경
                if (currentPathEffect != null)
                {
                    // 1. "이제 따라다니지 마!" (부모-자식 관계 해제)
                    currentPathEffect.transform.SetParent(null);

                    // 2. "이제 스스로 사라져!" (PathEffect 스크립트에게 명령)
                    PathEffect effectScript = currentPathEffect.GetComponent<PathEffect>();
                    if (effectScript != null)
                    {
                        // 3. 님이 설정한 "추가 생존 시간"을 넘겨줍니다.
                        effectScript.StartFadeOut(effectExtraLifetime);
                    }
                    else
                    {
                        // (스크립트가 없으면 그냥 딜레이 파괴)
                        Destroy(currentPathEffect, effectExtraLifetime);
                    }

                    // 4. 참조 비우기
                    currentPathEffect = null;
                }
            }
        }

        // 2. 무적 타이머
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

    // --- (이하 GetCurrentStacks, IsInvincible 등 나머지 함수는 동일) ---
    public bool IsDashing() { return isDashing; }
    public bool IsInvincible() { return isInvincible; }
    public int GetCurrentStacks() { return currentDashStacks; }
    public float GetCooldownProgress()
    {
        if (currentDashStacks == dashConfig.maxDashStacks) return 1f;
        return stackRechargeTimer / dashConfig.stackRechargeCooldown;
    }
}