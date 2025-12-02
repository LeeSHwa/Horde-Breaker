using UnityEngine;

/// <summary>
/// This component sits on the 'Hitbox' child object of the Sword.
/// Its only job is to detect collisions and report them to the main Sword.cs script.
/// Requires a Collider2D (IsTrigger=true) and a Rigidbody2D (Kinematic).
/// </summary>
public class SwordHitbox : MonoBehaviour
{
    // A public reference to the main controller script (Sword.cs)
    // This MUST be assigned by Sword.cs during its Initialize phase.
    public Sword swordController;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Do nothing if the controller isn't assigned
        if (swordController == null)
        {
            Debug.LogWarning("SwordHitbox has no swordController reference!");
            return;
        }

        // Ignore triggers that aren't tagged as "Enemy"
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        // Pass the collision event up to the main Sword.cs script
        swordController.HandleHit(other);
    }
}