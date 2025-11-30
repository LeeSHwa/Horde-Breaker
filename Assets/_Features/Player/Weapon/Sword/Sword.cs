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

    [Header("Visual Sync")]
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
        else { Debug.LogError("Sword: Wrong DataSO!"); return; }

        currentAreaRadius = swordData.baseAreaRadius;
        currentSwingDuration = swordData.baseSwingDuration;
        currentAngle = swordData.baseAngle;
        currentKnockback = swordData.knockback;

        isProjectileUnlocked = false;
        attacksPerProjectile = swordData.baseAttacksPerProjectile;

        enemiesHitThisSwing = new List<Collider2D>();

        // Visual Setup
        UpdateVisualScale();

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
        if (playerAnimator != null) playerAnimator.LockFacing(fixedDirection);
        hitboxCollider.enabled = true;
        if (guidelineContainer != null) guidelineContainer.SetActive(false);
        attackCount++;

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

        pivot.position = aim.position;
        pivot.rotation = Quaternion.Euler(0, 0, startAngle);

        if (swordTrail != null)
        {
            swordTrail.Clear();
            swordTrail.emitting = true;
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
        if (playerAnimator != null) playerAnimator.UnlockFacing();
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
        if (!isProjectileUnlocked) return;
        if (swordData.projectilePrefab == null) return;

        if (attackCount >= attacksPerProjectile)
        {
            attackCount = 0;

            // Spawn multiple projectiles based on total count
            int count = GetFinalProjectileCount();

            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float startAngle = baseAngle;
            float spreadAngle = 15f;

            if (count > 1)
            {
                float totalSpread = (count - 1) * spreadAngle;
                startAngle = baseAngle - (totalSpread / 2f);
            }

            for (int i = 0; i < count; i++)
            {
                GameObject projectileObj = PoolManager.Instance.GetFromPool(swordData.projectilePrefab.name);
                if (projectileObj == null) continue;

                projectileObj.transform.position = aim.position;

                float currentAngle = startAngle + (i * spreadAngle);
                projectileObj.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

                float arcLength = currentAreaRadius * (this.currentAngle * Mathf.Deg2Rad);
                float projectileScaleY = arcLength * swordData.projectileArcScaleMultiplier;
                Vector3 finalScale = new Vector3(pivot.localScale.x, projectileScaleY, 1f);

                Bullet projectileScript = projectileObj.GetComponent<Bullet>();
                if (projectileScript != null)
                {
                    float baseDamage = GetFinalDamage(out bool isCrit);
                    float projDmg = baseDamage * swordData.projectileDamagePercent;
                    float projKb = currentKnockback * swordData.projectileKnockbackPercent;
                    float baseLifetime = 3.0f;
                    float finalLifetime = GetFinalDuration(baseLifetime);

                    // Added dummy parameter '0' for pierceCount, since isInfinitePen is true
                    projectileScript.Initialize(
                        projDmg,
                        projKb,
                        true,
                        0,
                        ownerStats.transform,
                        finalScale,
                        finalLifetime,
                        weaponData.hitSound,
                        isCrit
                    );
                }
            }
        }
    }

    private void _Attack(StatsController enemyStats, Collider2D enemyCollider, float damage, float knockback)
    {
        // Multi-hit Logic based on Projectile Count
        int hitCount = GetFinalProjectileCount();

        for (int i = 0; i < hitCount; i++)
        {
            float finalDamage = GetFinalDamage(out bool isCrit);
            enemyStats.TakeDamage(finalDamage, isCrit);
        }

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

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: ApplyStats(swordData.level2_DamageBonus, swordData.level2_AreaBonus, swordData.level2_AngleBonus, swordData.level2_CooldownReduction); break;
            case 3: ApplyStats(swordData.level3_DamageBonus, swordData.level3_AreaBonus, swordData.level3_AngleBonus, swordData.level3_CooldownReduction); break;
            case 4: ApplyStats(swordData.level4_DamageBonus, swordData.level4_AreaBonus, swordData.level4_AngleBonus, swordData.level4_CooldownReduction); break;
            case 5: ApplyStats(swordData.level5_DamageBonus, swordData.level5_AreaBonus, swordData.level5_AngleBonus, swordData.level5_CooldownReduction); break;
            case 6: ApplyStats(swordData.level6_DamageBonus, swordData.level6_AreaBonus, swordData.level6_AngleBonus, swordData.level6_CooldownReduction); break;
            case 7: ApplyStats(swordData.level7_DamageBonus, swordData.level7_AreaBonus, swordData.level7_AngleBonus, swordData.level7_CooldownReduction); break;
            case 8: ApplyStats(swordData.level8_DamageBonus, swordData.level8_AreaBonus, swordData.level8_AngleBonus, swordData.level8_CooldownReduction); break;
            case 9:
                ApplyStats(swordData.level9_DamageBonus, swordData.level9_AreaBonus, swordData.level9_AngleBonus, swordData.level9_CooldownReduction);
                if (swordData.level9_UnlockProjectile)
                {
                    isProjectileUnlocked = true;
                    attackCount = 0;
                    attacksPerProjectile -= swordData.level9_AttacksPerProjectile_Reduce;
                    if (attacksPerProjectile < 1) attacksPerProjectile = 1;
                }
                break;
        }

        if (currentSwingDuration < 0.1f) currentSwingDuration = 0.1f;
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
        UpdateVisualScale();
    }

    private void ApplyStats(float dmg, float area, float angle, float cooldownPercent)
    {
        currentDamage += dmg;
        currentAreaRadius += area;
        currentAngle += angle;
        float reductionAmount = currentAttackCooldown * (cooldownPercent / 100f);
        currentAttackCooldown -= reductionAmount;
    }
}