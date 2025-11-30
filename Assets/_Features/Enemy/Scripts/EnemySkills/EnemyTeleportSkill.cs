using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(Collider2D))]
public class EnemyTeleportSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float triggerRange = 10f;       // Distance to player to start using skill
    public float teleportRadius = 3f;      // Random radius around player to reappear
    public float skillCooldown = 8.0f;

    [Header("Timing Settings")]
    public float chargeTime = 0.5f;        // Time before vanishing
    public float vanishDuration = 1.0f;    // Time spent invisible/invincible
    public float appearDelay = 0.5f;       // Time to wait after reappearing

    [Header("Visual Settings")]
    public bool useColorChange = true;
    public Color chargeColor = Color.magenta;
    public Color appearColor = Color.yellow;

    [Header("Animation Settings")]
    public string animChargeTrigger = "doTeleportCharge";
    public string animVanishTrigger = "doVanish";
    public string animAppearTrigger = "doAppear";
    public string animIdleTrigger = "doIdle";
    public string animMoveBool = "isMoving";

    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;
    private Transform playerTarget;
    private Color originalColor;

    private float lastUsedTime = -999f;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr != null) originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    // [Manager Call]
    public bool IsReady(float distanceToPlayer)
    {
        bool isCooldownReady = Time.time >= lastUsedTime + skillCooldown;
        bool isRangeReady = distanceToPlayer <= triggerRange;
        bool isNotDisabled = (movementScript != null && movementScript.enabled);

        return isCooldownReady && isRangeReady && isNotDisabled;
    }

    // [Manager Call]
    public void Execute(System.Action onComplete)
    {
        lastUsedTime = Time.time;
        StartCoroutine(TeleportRoutine(onComplete));
    }

    private IEnumerator TeleportRoutine(System.Action onComplete)
    {
        // --- PHASE 1: CHARGE (Preparation) ---
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool(animMoveBool, false);
            anim.SetTrigger(animChargeTrigger);
        }

        if (useColorChange && sr != null) sr.color = chargeColor;

        yield return new WaitForSeconds(chargeTime);

        // --- PHASE 2: VANISH (Invincible & Invisible) ---
        if (col != null) col.enabled = false;

        if (anim != null && !string.IsNullOrEmpty(animVanishTrigger))
            anim.SetTrigger(animVanishTrigger);

        if (sr != null) sr.enabled = false;

        yield return new WaitForSeconds(vanishDuration);

        // Calculate new position near player
        Vector2 targetPos = CalculateTeleportPosition();
        transform.position = targetPos;

        // --- PHASE 3: APPEAR (Re-enable visuals) ---
        if (sr != null) sr.enabled = true;

        if (anim != null) anim.SetTrigger(animAppearTrigger);
        if (useColorChange && sr != null) sr.color = appearColor;

        yield return new WaitForSeconds(appearDelay);

        // --- PHASE 4: RECOVERY (Reset State) ---
        if (col != null) col.enabled = true;

        if (useColorChange && sr != null) sr.color = originalColor;
        if (anim != null) anim.SetTrigger(animIdleTrigger);

        if (movementScript != null) movementScript.enabled = true;

        // Notify Manager
        onComplete?.Invoke();
    }

    private Vector2 CalculateTeleportPosition()
    {
        if (playerTarget == null) return transform.position;

        // Get random position around player
        Vector2 randomOffset = Random.insideUnitCircle.normalized * teleportRadius;
        Vector2 finalPos = (Vector2)playerTarget.position + randomOffset;

        // Constraint to Camera Bounds if available
        Vector2 min = CameraBoundsController.MinBounds;
        Vector2 max = CameraBoundsController.MaxBounds;

        if (min != Vector2.zero && max != Vector2.zero)
        {
            float margin = 1.0f;
            finalPos.x = Mathf.Clamp(finalPos.x, min.x + margin, max.x - margin);
            finalPos.y = Mathf.Clamp(finalPos.y, min.y + margin, max.y - margin);
        }

        return finalPos;
    }
}