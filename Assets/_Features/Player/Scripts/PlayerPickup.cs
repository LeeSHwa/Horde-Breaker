using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPickup : MonoBehaviour
{
    private CircleCollider2D pickupCollider;
    private Transform playerTransform;

    // The base radius from the SO, stored at runtime
    private float currentBaseRadius;

    // Bonus percentage from passives (e.g., 0.2f = +20%)
    private float bonusRadiusPercent = 0f;

    void Awake()
    {
        playerTransform = transform.parent;
        pickupCollider = GetComponent<CircleCollider2D>();
        pickupCollider.isTrigger = true;

        // Ensure Rigidbody2D is Kinematic
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>(); // Add if missing
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    // Called by StatsController during initialization
    public void InitializeRadius(float baseRadiusFromSO)
    {
        currentBaseRadius = baseRadiusFromSO;
        UpdateColliderRadius();
    }

    // Example: playerPickup.SetBonusRadiusPercent(0.2f); // for +20%
    //public void SetBonusRadiusPercent(float newBonusPercent)
    //{
    //    bonusRadiusPercent = newBonusPercent;
    //    UpdateColliderRadius();
    //}

    // Helper function to apply the final calculated radius
    private void UpdateColliderRadius()
    {
        pickupCollider.radius = currentBaseRadius * (1f + bonusRadiusPercent);
    }

    // This trigger logic remains the same
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExpOrb"))
        {
            ExpOrb orb = other.GetComponent<ExpOrb>();
            if (orb != null)
            {
                orb.ActivateMagnet(playerTransform);
            }
        }
    }
}