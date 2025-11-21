/*
 * Sword.cs
 * * This script manages the logic for the 'Sword' weapon. It inherits from Weapon.cs.
 * * [V10 Update] Solves the visual/stat sync problem.
 * - Uses 'visualBaselineRadius' (set in Inspector) as the sprite's "natural" length at scale 1.
 * - Uses 'baseAreaRadius' (from SO) as the "target" starting length.
 * - Initialize() now calculates the *initial scale* (target / baseline)
 * to make the visuals match the stats FROM THE START.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Required for using UI.Image

public class Sword : Weapon
{
    [Header("Sword References")]
    public Transform pivot;
    public Image guidelineImage;
    public GameObject guidelineContainer;
    public Collider2D hitboxCollider;
    public TrailRenderer swordTrail;

    [Header("Guideline Calibration")]
    public float guidelineRadiusMultiplier = 1.0f;
    public float guidelineAngleMultiplier = 1.0f;

    [Header("Visual Sync (V10)")]
    [Tooltip("Set this to the sword's 'natural' visual length when Pivot scale is (1,1,1)")]
    public float visualBaselineRadius = 1.5f; // 예: 1.5 유닛

    // --- Private Variables ---
    private SwordDataSO swordData;
    private enum State { Idle, Swinging }
    private State currentState = State.Idle;
    private Vector2 currentAimDirection;
    private float currentSwingDuration;
    private float currentAngle;
    private float currentAreaRadius; // This is the 'target' radius
    private float currentKnockback;
    private List<Collider2D> enemiesHitThisSwing;
    private int attackCount = 0;
    private Transform trailAnchor;

    // (1) Initialization
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        base.Initialize(aimObj, owner, animator);

        if (weaponData is SwordDataSO) { swordData = (SwordDataSO)weaponData; }
        else { Debug.LogError(gameObject.name + " has wrong SO!"); return; }

        // --- [V10] Automatic Setup from SO & Visual Sync ---

        // 1. Set the 'target' radius to the "desired" start radius from the SO
        currentAreaRadius = swordData.baseAreaRadius; // (e.g., 2.0)

        // 2. Calculate and apply the *initial* scale factor
        // (e.g., target 2.0 / baseline 1.5 = 1.333 scale)
        if (visualBaselineRadius <= 0) visualBaselineRadius = 1f; // Prevent division by zero
        float startScaleFactor = currentAreaRadius / visualBaselineRadius;
        pivot.localScale = Vector3.one * startScaleFactor;

        // 3. Find and Position the Trail Anchor
        if (swordTrail != null)
        {
            trailAnchor = swordTrail.transform;

            // Set Trail Anchor's LOCAL position
            // (It's a child of Pivot, so it scales with Pivot)
            // [V10 Fix] We use the BASELINE radius here, because the parent (Pivot) is already scaled.
            trailAnchor.localPosition = new Vector3(visualBaselineRadius / 2f, 0, 0);

            // 4. Set Trail Width
            // The width should be the final 'target' radius
            swordTrail.widthMultiplier = currentAreaRadius;
            swordTrail.emitting = false;
        }
        else { Debug.LogError("Sword Trail is not assigned!", this); }

        // --- End of V10 Setup ---

        // Init stats
        currentSwingDuration = swordData.baseSwingDuration;
        currentAngle = swordData.baseAngle;
        currentKnockback = swordData.knockback;
        enemiesHitThisSwing = new List<Collider2D>();

        // Init Hitbox
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            SwordHitbox hitbox = hitboxCollider.GetComponent<SwordHitbox>();
            if (hitbox != null) { hitbox.swordController = this; }
            else { Debug.LogError("Hitbox missing SwordHitbox.cs!", hitboxCollider.gameObject); }
        }
        else { Debug.LogError("Hitbox Collider not set!", this); }

        // Init Guideline
        if (guidelineContainer != null) { guidelineContainer.SetActive(false); }
        if (guidelineImage == null) { Debug.LogError("guidelineImage not set!", this); }
    }

    // (2) TryAttack
    public override void TryAttack(Vector2 aimDirection)
    {
        this.currentAimDirection = aimDirection;
        if (Time.time >= lastAttackTime + currentAttackCooldown && currentState == State.Idle)
        {
            lastAttackTime = Time.time;
            StartCoroutine(SwingCoroutine(aimDirection));

            // [NEW] Play Attack Sound
            if (weaponData.attackSound != null)
            {
                SoundManager.Instance.PlaySFX(weaponData.attackSound, 0.1f);
            }
        }
    }

    // (3) Update (Guideline Logic)
    void Update()
    {
        if (guidelineContainer == null || guidelineImage == null) return;
        if (currentState == State.Idle)
        {
            if (Time.time < lastAttackTime + currentAttackCooldown)
            {
                guidelineContainer.SetActive(true);
                guidelineContainer.transform.position = aim.position;
                guidelineContainer.transform.rotation = aim.rotation;

                float calibratedAngle = currentAngle * guidelineAngleMultiplier;
                guidelineImage.fillAmount = calibratedAngle / 360f;

                // [V10] Use currentAreaRadius (the 'target' radius)
                float canvasScale = guidelineContainer.transform.localScale.x;
                float diameter = (currentAreaRadius * guidelineRadiusMultiplier * 2f);
                if (Mathf.Abs(canvasScale) > 0.0001f) { diameter /= canvasScale; }
                else { diameter /= 0.01f; }
                guidelineImage.rectTransform.sizeDelta = new Vector2(diameter, diameter);

                // [V8 FIX] Use a POSITIVE offset
                guidelineImage.rectTransform.localRotation = Quaternion.Euler(0, 0, calibratedAngle / 2f);
            }
            else
            {
                guidelineContainer.SetActive(false);
            }
        }
    }

    // (4) The Core Swing Logic
    private IEnumerator SwingCoroutine(Vector2 fixedDirection)
    {
        // --- 1. PREPARE SWING ---
        currentState = State.Swinging;
        enemiesHitThisSwing.Clear();
        playerAnimator.LockFacing(fixedDirection);
        hitboxCollider.enabled = true;
        if (guidelineContainer != null) guidelineContainer.SetActive(false);
        attackCount++;
        if (swordTrail != null)
        {
            swordTrail.Clear();
            swordTrail.emitting = true;
        }

        // --- 2. CALCULATE ANGLES ---
        float centerAngle = Mathf.Atan2(fixedDirection.y, fixedDirection.x) * Mathf.Rad2Deg;
        float startAngle; float endAngle;
        if (fixedDirection.x < 0)
        { // [V7] Mirrored swing
            startAngle = centerAngle - swordData.swingStartOffset;
            endAngle = centerAngle + (currentAngle - swordData.swingStartOffset);
        }
        else
        {
            startAngle = centerAngle + swordData.swingStartOffset;
            endAngle = centerAngle - (currentAngle - swordData.swingStartOffset);
        }

        // --- 3. EXECUTE SWING ---
        float swingTimer = 0f;
        while (swingTimer < currentSwingDuration)
        {
            swingTimer += Time.deltaTime;
            float t = swingTimer / currentSwingDuration;
            float currentSwingAngle = Mathf.Lerp(startAngle, endAngle, t);
            pivot.position = aim.position;
            pivot.rotation = Quaternion.Euler(0, 0, currentSwingAngle);
            yield return null;
        }

        // --- 4. FINISH SWING ---
        pivot.position = aim.position;
        pivot.rotation = Quaternion.Euler(0, 0, endAngle);
        hitboxCollider.enabled = false;
        playerAnimator.UnlockFacing();
        currentState = State.Idle;
        if (swordTrail != null)
        {
            swordTrail.emitting = false;
        }

        // --- 5. CHECK PROJECTILE ---
        CheckLevel5Projectile(fixedDirection);
    }

    // (5) HandleHit
    public void HandleHit(Collider2D enemyCollider)
    {
        if (enemiesHitThisSwing.Contains(enemyCollider)) { return; }
        enemiesHitThisSwing.Add(enemyCollider);

        StatsController enemyStats = enemyCollider.GetComponent<StatsController>();
        if (enemyStats != null)
        {
            _Attack(enemyStats, enemyCollider, currentDamage, currentKnockback);
        }
    }

    // (6) CheckLevel5Projectile
    private void CheckLevel5Projectile(Vector2 direction)
    {
        if (currentLevel < 5) return;
        if (swordData.projectilePrefab == null) { /* ... */ return; }
        if (attackCount >= swordData.level5_AttacksPerProjectile)
        {
            attackCount = 0;
            GameObject projectileObj = PoolManager.Instance.GetFromPool(swordData.projectilePrefab.name);
            if (projectileObj == null) return;

            projectileObj.transform.position = aim.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            // [V10] Projectile scale is also driven by pivot's scale
            projectileObj.transform.localScale = pivot.localScale;

            Bullet projectileScript = projectileObj.GetComponent<Bullet>();
            if (projectileScript != null)
            {
                float projDmg = (currentDamage * ownerStats.currentDamageMultiplier) * swordData.projectileDamagePercent;
                float projKb = currentKnockback * swordData.projectileKnockbackPercent;

                // [MODIFIED] Pass 'weaponData.hitSound'
                projectileScript.Initialize(projDmg, projKb, true, ownerStats.transform, weaponData.hitSound);
            }
        }
    }

    // (7) ApplyLevelUpStats
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: // Damage + Speed + Cooldown
                currentDamage += swordData.level2_DamageBonus;

                // Swing Duration (Animation Speed)
                currentSwingDuration -= swordData.level2_SpeedIncrease;
                if (currentSwingDuration < 0.1f) currentSwingDuration = 0.1f;

                // [NEW] Cooldown Reduction (Wait Time)
                currentAttackCooldown -= swordData.level2_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;

            case 3: // Length (Radius)
                    // --- [V10] Automatic Scaling ---

                // 1. Update the 'target' radius stat
                currentAreaRadius += swordData.level3_AreaIncrease;

                // 2. Calculate the new total scale factor
                float newScaleFactor = currentAreaRadius / visualBaselineRadius;

                // 3. Apply scale to the visual pivot
                pivot.localScale = Vector3.one * newScaleFactor;

                // 4. Update the trail width to match the new 'target' radius
                if (swordTrail != null)
                {
                    swordTrail.widthMultiplier = currentAreaRadius;
                }
                // (TrailAnchor position updates automatically as a child of pivot)
                // --- End of V10 ---
                break;

            case 4: // Angle
                currentAngle += swordData.level4_AngleIncrease;
                break;

            case 5: // Projectile + Cooldown
                attackCount = 0;

                // [NEW] Cooldown Reduction
                currentAttackCooldown -= swordData.level5_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
        }
    }

    // (8) _Attack
    private void _Attack(StatsController enemyStats, Collider2D enemyCollider, float damage, float knockback)
    {
        float finalDamage = damage * ownerStats.currentDamageMultiplier;
        enemyStats.TakeDamage(finalDamage);
        EnemyMovement enemyMove = enemyCollider.GetComponent<EnemyMovement>();
        if (enemyMove != null)
        {
            Vector2 knockbackDirection = (enemyCollider.transform.position - ownerStats.transform.position).normalized;
            if (knockbackDirection == Vector2.zero) { knockbackDirection = pivot.right; }
            enemyMove.ApplyKnockback(knockbackDirection, knockback, 0.1f);
        }

        // [NEW] Play Hit Sound (Directly)
        if (weaponData.hitSound != null)
        {
            SoundManager.Instance.PlaySFX(weaponData.hitSound, 0.1f);
        }
    }

    // (9) PerformAttack (Empty)
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // Intentionally left empty
    }
}