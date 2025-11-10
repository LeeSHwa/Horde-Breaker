using UnityEngine;
using System.Collections;

// This class manages the logic for the Sword weapon, inheriting from Weapon2.
public class Sword : Weapon
{
    // Caches the Sword-specific ScriptableObject data.
    private SwordDataSO swordData;

    // The current attack area, modified by level-ups.
    protected float currentArea;

    // The current knockback strength.
    protected float currentKnockback;

    // The duration of the slash attack in seconds, configurable in the Inspector.
    [SerializeField]
    private float slashDuration = 0.3f;

    // (1) Override Initialize to handle Sword-specific data
    public override void Initialize(Transform aimObj, StatsController owner)
    {
        // MUST call base.Initialize first to set up common stats
        base.Initialize(aimObj, owner);

        // Cast generic data to specific data
        swordData = (SwordDataSO)weaponData;

        // Initialize Sword-specific stats
        currentArea = swordData.baseArea;
        currentKnockback = swordData.knockback;
    }

    // Called by the Weapon2 base class to perform an attack.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // Get a recycled hitbox object from the PoolManager.
        GameObject slashHitbox = PoolManager.Instance.GetFromPool(swordData.hitboxPrefab.name);

        // Set the starting position of the slash (relative to the 'aim' transform).
        Vector3 spawnPos = aim.position;
        // Minor adjustment to position the slash correctly.
        float y_adjustment = 0.2f;
        spawnPos.y -= y_adjustment;

        slashHitbox.transform.position = spawnPos;
        slashHitbox.transform.rotation = aim.rotation;

        // Get the hitbox script component to initialize it.
        SwordHitbox hitboxScript = slashHitbox.GetComponent<SwordHitbox>();
        if (hitboxScript != null)
        {
            // Pass the current damage, knockback, and area to the hitbox.
            hitboxScript.Initialize(
                currentDamage,
                currentKnockback,
                currentArea,
                aim.rotation,
                ownerStats.transform // [New] Pass the owner's transform as the attack source
            );

            // Start the coroutine that creates the swinging motion.
            StartCoroutine(SwingSwordHitbox(slashHitbox, slashHitbox.transform.rotation, currentArea));
        }
    }

    // Coroutine to animate the sword slash (rotation and scale) over time.
    private IEnumerator SwingSwordHitbox(GameObject hitbox, Quaternion startRotation, float targetArea)
    {
        float timer = 0f;
        float swingDuration = slashDuration;

        // Defines the arc of the swing (120 degrees total).
        float swingAngle = 120f;
        // Starts the swing 60 degrees "behind" the aim direction.
        float startAngleOffset = -60f;

        // The hitbox starts at 20% of its full size.
        float initialScaleFactor = 0.2f;

        // Calculate the final target size, including the global multiplier from the SO.
        float finalTargetArea = targetArea * swordData.sizeMultiplier;

        // Set the initial scale of the hitbox.
        hitbox.transform.localScale = Vector3.one * (finalTargetArea * initialScaleFactor);

        yield return null; // Wait for the next frame.

        // Loop over the duration of the slash.
        while (timer < swingDuration)
        {
            // 'progress' goes from 0 to 1 over the swingDuration.
            float progress = timer / swingDuration;

            // Animate the rotation:
            // Lerp creates a smooth rotation from the start angle to the end angle.
            float currentAngle = Mathf.Lerp(startAngleOffset, startAngleOffset + swingAngle, progress);
            hitbox.transform.rotation = startRotation * Quaternion.Euler(0, 0, currentAngle);

            // Animate the scale:
            // Mathf.Sin(progress * Mathf.PI) creates a "grow and shrink" effect (0 -> 1 -> 0).
            float scaleProgress = Mathf.Sin(progress * Mathf.PI);
            // Apply the scaling effect, starting from initialScaleFactor up to finalTargetArea.
            hitbox.transform.localScale = Vector3.one * (finalTargetArea * (initialScaleFactor + (1 - initialScaleFactor) * scaleProgress));

            // Advance the timer.
            timer += Time.deltaTime;
            yield return null; // Wait for the next frame.
        }
    }

    // Applies stat changes when the weapon levels up.
    protected override void ApplyLevelUpStats()
    {
        // Check the new level and apply upgrades accordingly.
        switch (currentLevel)
        {
            case 3:
                // Increase the attack area by the specified multiplier.
                currentArea *= swordData.level3_AreaIncrease;
                break;
            case 5:
                // Example: Increase pierce (currently commented out).
                // currentPierce += swordData.level5_PierceIncrease;
                break;
        }
    }
}