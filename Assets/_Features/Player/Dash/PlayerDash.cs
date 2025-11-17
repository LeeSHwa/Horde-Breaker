using UnityEngine;
using System.Collections; 

public class PlayerDash : MonoBehaviour
{
    public DashConfigSO dashConfig;
    public GameObject pathPrefab; 
    public Material ghostMaterial;
    public float trailEffectLifetime = 0.5f;
    public float trailSpawnInterval = 0.05f;
    public int blinkTrailCount = 10;

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
    }

    void Update()
    {
        HandleInput();
        HandleStackRecharge();
        HandleDashTimers();
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

        Vector2 startPos = rb.position;
        float dashDistance = dashConfig.dashSpeed * dashConfig.dashMoveDuration;
        Vector2 endPos = startPos + dashDirection * dashDistance;
        rb.MovePosition(endPos);

        if (pathPrefab != null && ghostMaterial != null)
        {
            StartCoroutine(SpawnBlinkTrailCoroutine(startPos));
        }
    }
    private IEnumerator SpawnBlinkTrailCoroutine(Vector2 startPos)
    {
        Sprite sprite = playerSpriteRenderer.sprite;
        bool flipX = playerSpriteRenderer.flipX;
        int sortingOrder = playerSpriteRenderer.sortingOrder;
        string dashPoolTag = "DashEffect";

        for (int i = 0; i < blinkTrailCount; i++)
        {
            Vector2 currentDynamicEndPos = transform.position;
            float t = (blinkTrailCount <= 1) ? 0.5f : (float)i / (blinkTrailCount - 1);
            Vector2 trailPos = Vector2.Lerp(startPos, currentDynamicEndPos, t);

            GameObject effectObj = PoolManager.Instance.GetFromPool(dashPoolTag);

            if (effectObj == null)
            {
                Debug.LogWarning("DashEffect pool is full or does not exist!!");
                break;
            }

            effectObj.transform.position = trailPos;
            effectObj.transform.rotation = Quaternion.identity;

            DashEffect effectScript = effectObj.GetComponent<DashEffect>();

            if (effectScript != null)
            {
                effectScript.Initialize(
          sprite,
          flipX,
          sortingOrder,
          ghostMaterial,
          trailEffectLifetime
        );
            }

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