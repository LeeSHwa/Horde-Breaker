using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    public float visualBaselineRadius = 1.5f;

    private SwordDataSO swordData;
    private enum State { Idle, Swinging }
    private State currentState = State.Idle;
    private Vector2 currentAimDirection;
    private float currentSwingDuration;
    private float currentAngle;
    private float currentAreaRadius;
    private float currentKnockback;
    private List<Collider2D> enemiesHitThisSwing;
    private int attackCount = 0;

    private bool isProjectileUnlocked = false;
    private int attacksPerProjectile;
    private Transform trailAnchor;

    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        base.Initialize(aimObj, owner, animator);

        if (weaponData is SwordDataSO) { swordData = (SwordDataSO)weaponData; }
        else { Debug.LogError(gameObject.name + " has wrong SO!"); return; }

        // Initialize Stats
        currentAreaRadius = swordData.baseAreaRadius;
        UpdateVisualScale();

        currentSwingDuration = swordData.baseSwingDuration;
        currentAngle = swordData.baseAngle;
        currentKnockback = swordData.knockback;

        isProjectileUnlocked = false;
        attacksPerProjectile = swordData.baseAttacksPerProjectile;

        enemiesHitThisSwing = new List<Collider2D>();

        if (swordTrail != null)
        {
            trailAnchor = swordTrail.transform;
            trailAnchor.localPosition = new Vector3(visualBaselineRadius / 2f, 0, 0);
            swordTrail.widthMultiplier = currentAreaRadius;
            swordTrail.emitting = false;
        }

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            SwordHitbox hitbox = hitboxCollider.GetComponent<SwordHitbox>();
            if (hitbox != null) { hitbox.swordController = this; }
        }

        if (guidelineContainer != null) { guidelineContainer.SetActive(false); }
    }

    private void UpdateVisualScale()
    {
        if (visualBaselineRadius <= 0) visualBaselineRadius = 1f;
        float scaleFactor = currentAreaRadius / visualBaselineRadius;
        pivot.localScale = Vector3.one * scaleFactor;
        if (swordTrail != null) swordTrail.widthMultiplier = currentAreaRadius;
    }

    public override void TryAttack(Vector2 aimDirection)
    {
        this.currentAimDirection = aimDirection;
        if (Time.time >= lastAttackTime + currentAttackCooldown && currentState == State.Idle)
        {
            lastAttackTime = Time.time;
            StartCoroutine(SwingCoroutine(aimDirection));

            if (weaponData.attackSound != null)
            {
                SoundManager.Instance.PlaySFX(weaponData.attackSound, 0.1f);
            }
        }
    }

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

                float canvasScale = guidelineContainer.transform.localScale.x;
                float diameter = (currentAreaRadius * guidelineRadiusMultiplier * 2f);
                if (Mathf.Abs(canvasScale) > 0.0001f) { diameter /= canvasScale; }
                else { diameter /= 0.01f; }
                guidelineImage.rectTransform.sizeDelta = new Vector2(diameter, diameter);
                guidelineImage.rectTransform.localRotation = Quaternion.Euler(0, 0, calibratedAngle / 2f);
            }
            else
            {
                guidelineContainer.SetActive(false);
            }
        }
    }

    private IEnumerator SwingCoroutine(Vector2 fixedDirection)
    {
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

        float centerAngle = Mathf.Atan2(fixedDirection.y, fixedDirection.x) * Mathf.Rad2Deg;
        float startAngle; float endAngle;
        if (fixedDirection.x < 0)
        {
            startAngle = centerAngle - swordData.swingStartOffset;
            endAngle = centerAngle + (currentAngle - swordData.swingStartOffset);
        }
        else
        {
            startAngle = centerAngle + swordData.swingStartOffset;
            endAngle = centerAngle - (currentAngle - swordData.swingStartOffset);
        }

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

        pivot.position = aim.position;
        pivot.rotation = Quaternion.Euler(0, 0, endAngle);
        hitboxCollider.enabled = false;
        playerAnimator.UnlockFacing();
        currentState = State.Idle;
        if (swordTrail != null)
        {
            swordTrail.emitting = false;
        }

        CheckProjectile(fixedDirection);
    }

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

    private void CheckProjectile(Vector2 direction)
    {
        if (currentLevel < 9) return;
        if (!isProjectileUnlocked) return;

        if (swordData.projectilePrefab == null) return;

        if (attackCount >= attacksPerProjectile)
        {
            attackCount = 0;
            GameObject projectileObj = PoolManager.Instance.GetFromPool(swordData.projectilePrefab.name);
            if (projectileObj == null) return;

            projectileObj.transform.position = aim.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            float arcLength = currentAreaRadius * (currentAngle * Mathf.Deg2Rad);
            float projectileScaleY = arcLength * swordData.projectileArcScaleMultiplier;
            projectileObj.transform.localScale = new Vector3(pivot.localScale.x, projectileScaleY, 1f);

            Bullet projectileScript = projectileObj.GetComponent<Bullet>();
            if (projectileScript != null)
            {
                float projDmg = (currentDamage * ownerStats.currentDamageMultiplier) * swordData.projectileDamagePercent;
                float projKb = currentKnockback * swordData.projectileKnockbackPercent;
                projectileScript.Initialize(projDmg, projKb, true, ownerStats.transform, weaponData.hitSound);
            }
        }
    }

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

        if (weaponData.hitSound != null)
        {
            SoundManager.Instance.PlaySFX(weaponData.hitSound, 0.1f);
        }
    }

    protected override void PerformAttack(Vector2 aimDirection) { }

    // Apply stats for all levels (2~9)
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                ApplyStats(swordData.level2_DamageBonus, swordData.level2_AreaIncrease,
                           swordData.level2_AngleIncrease, swordData.level2_CooldownReduction);
                currentSwingDuration -= swordData.level2_SpeedIncrease;
                break;
            case 3:
                ApplyStats(swordData.level3_DamageBonus, swordData.level3_AreaIncrease,
                           swordData.level3_AngleIncrease, swordData.level3_CooldownReduction);
                break;
            case 4:
                ApplyStats(swordData.level4_DamageBonus, swordData.level4_AreaIncrease,
                           swordData.level4_AngleIncrease, swordData.level4_CooldownReduction);
                break;
            case 5:
                ApplyStats(swordData.level5_DamageBonus, swordData.level5_AreaIncrease,
                           swordData.level5_AngleIncrease, swordData.level5_CooldownReduction);
                break;
            case 6:
                ApplyStats(swordData.level6_DamageBonus, swordData.level6_AreaIncrease,
                           swordData.level6_AngleIncrease, swordData.level6_CooldownReduction);
                break;
            case 7:
                ApplyStats(swordData.level7_DamageBonus, swordData.level7_AreaIncrease,
                           swordData.level7_AngleIncrease, swordData.level7_CooldownReduction);
                break;
            case 8:
                ApplyStats(swordData.level8_DamageBonus, swordData.level8_AreaIncrease,
                           swordData.level8_AngleIncrease, swordData.level8_CooldownReduction);
                break;
            case 9: // Level 9 (Max)
                ApplyStats(swordData.level9_DamageBonus, swordData.level9_AreaIncrease,
                           swordData.level9_AngleIncrease, swordData.level9_CooldownReduction);

                // Unlock Projectile at Level 9
                isProjectileUnlocked = true;
                attackCount = 0;
                attacksPerProjectile -= swordData.level9_AttacksPerProjectile_Reduce;
                if (attacksPerProjectile < 1) attacksPerProjectile = 1;
                break;
        }

        // Safety Checks
        if (currentSwingDuration < 0.1f) currentSwingDuration = 0.1f;
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
        UpdateVisualScale();
    }

    private void ApplyStats(float dmg, float area, float angle, float cooldownPercent)
    {
        currentDamage += dmg;
        currentAreaRadius += area;
        currentAngle += angle;

        // If current cooldown is 1.0s and input is 10, reduction is 0.1s
        float reductionAmount = currentAttackCooldown * (cooldownPercent / 100f);
        currentAttackCooldown -= reductionAmount;
    }
}