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
    public bool useColorChange = true;
    public Color chargeColor = Color.red;

    [Header("Animation Settings")]
    public string animChargeTrigger = "doCharge";
    public string animDashBool = "isDashing";
    public string animIdleTrigger = "doIdle";
    public string animMoveBool = "isMoving";

    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Transform playerTarget;
    private Color originalColor;

    // Timer to track individual skill cooldown
    private float lastUsedTime = -999f;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr != null) originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    // [Manager Call] Check if this skill is ready to be used
    public bool IsReady(float distanceToPlayer)
    {
        bool isCooldownReady = Time.time >= lastUsedTime + skillCooldown;
        bool isRangeReady = distanceToPlayer <= triggerRange;

        return isCooldownReady && isRangeReady;
    }

    // [Manager Call] Execute the skill logic
    public void Execute(System.Action onComplete)
    {
        lastUsedTime = Time.time; // Update cooldown usage time
        StartCoroutine(DashRoutine(onComplete));
    }

    private IEnumerator DashRoutine(System.Action onComplete)
    {
        // 1. Prepare (Stop movement)
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // 2. Charge Animation/Visuals
        if (anim != null)
        {
            anim.SetBool(animMoveBool, false);
            anim.SetTrigger(animChargeTrigger);
        }

        if (useColorChange && sr != null) sr.color = chargeColor;

        yield return new WaitForSeconds(chargeTime);

        // 3. Dash Action
        if (anim != null) anim.SetBool(animDashBool, true);

        Vector2 direction;
        if (playerTarget != null)
            direction = (playerTarget.position - transform.position).normalized;
        else
            direction = transform.right;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.linearVelocity = direction * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        if (anim != null) anim.SetBool(animDashBool, false);

        rb.linearVelocity = Vector2.zero;

        // 4. Reset Visuals
        if (useColorChange && sr != null) sr.color = originalColor;
        if (anim != null) anim.SetTrigger(animIdleTrigger);

        // 5. Rest (Post-skill delay within the skill itself)
        yield return new WaitForSeconds(restTime);

        if (movementScript != null) movementScript.enabled = true;

        // 6. Notify Manager that skill is finished
        onComplete?.Invoke();
    }
}