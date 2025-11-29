using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyDashSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float triggerRange = 5f;
    public float chargeTime = 0.5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float restTime = 1.0f;
    public float skillCooldown = 5.0f;

    [Header("Visual Settings")]
    [Tooltip("If true, the sprite turns color during charge. Disable if using animations.")]
    public bool useColorChange = true; // [NEW] 색상 변경 사용 여부 토글
    public Color chargeColor = Color.red;

    [Header("Animation Settings")]
    public string animChargeTrigger = "doCharge";
    public string animDashTrigger = "doDash";
    public string animIdleTrigger = "doIdle";
    public string animMoveBool = "isMoving";

    // Components
    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Transform playerTarget;
    private Color originalColor;

    private bool isSkillAvailable = true;
    private bool isDashing = false;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 색상 변경을 쓸 때만 원래 색상을 저장
        if (sr != null) originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (!isSkillAvailable || isDashing || playerTarget == null) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);

        if (distance <= triggerRange)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        isSkillAvailable = false;

        // 1. [제어권 가져오기]
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // 2. [차징]
        if (anim != null)
        {
            anim.SetBool(animMoveBool, false);
            anim.SetTrigger(animChargeTrigger);
        }

        // [MODIFIED] 토글이 켜져 있을 때만 색상 변경
        if (useColorChange && sr != null)
        {
            sr.color = chargeColor;
        }

        yield return new WaitForSeconds(chargeTime);

        // 3. [돌진]
        Vector2 direction;
        if (playerTarget != null)
            direction = (playerTarget.position - transform.position).normalized;
        else
            direction = transform.right;

        if (anim != null) anim.SetTrigger(animDashTrigger);

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.linearVelocity = direction * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        // 4. [휴식]
        rb.linearVelocity = Vector2.zero;

        // [MODIFIED] 토글이 켜져 있을 때만 색상 복구
        if (useColorChange && sr != null)
        {
            sr.color = originalColor;
        }

        if (anim != null) anim.SetTrigger(animIdleTrigger);

        yield return new WaitForSeconds(restTime);

        // 5. [복귀]
        if (movementScript != null) movementScript.enabled = true;
        isDashing = false;

        // 6. [쿨타임]
        yield return new WaitForSeconds(skillCooldown);
        isSkillAvailable = true;
    }
}