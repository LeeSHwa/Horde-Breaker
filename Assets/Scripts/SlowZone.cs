using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]

public class SlowZone : MonoBehaviour
{
    [Header("Block Settings")]
    [Tooltip("If checked, this becomes a solid 'wall'. If unchecked, it becomes a pass-through 'zone'.")]
    public bool isSolid = false;

    [Header("Slow Settings")]
    [Range(0, 50)]
    [Tooltip("The amount of Linear Damping applied. Higher values cause objects to slow down faster. (5-15 recommended)")]
    public float stickiness = 10f;

    // Stores the original damping values for each Rigidbody that enters
    private Dictionary<Rigidbody2D, float> originalDampings = new Dictionary<Rigidbody2D, float>();
    private BoxCollider2D boxCollider;

    void Awake()
    {
        // CHANGED: Now specifically gets a BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();
        // Set the trigger state based on the isSolid value
        boxCollider.isTrigger = !isSolid;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!boxCollider.isTrigger) return;
        ApplySlow(other.GetComponent<Rigidbody2D>());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (boxCollider.isTrigger) return;
        ApplySlow(other.collider.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!boxCollider.isTrigger) return;
        ResetSlow(other.GetComponent<Rigidbody2D>());
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (boxCollider.isTrigger) return;
        ResetSlow(other.collider.GetComponent<Rigidbody2D>());
    }

    void ApplySlow(Rigidbody2D rb)
    {
        if (rb == null || originalDampings.ContainsKey(rb)) return;

        originalDampings.Add(rb, rb.linearDamping);
        rb.linearDamping = stickiness;
    }

    void ResetSlow(Rigidbody2D rb)
    {
        if (rb == null) return;

        if (originalDampings.TryGetValue(rb, out float originalDampingValue))
        {
            rb.linearDamping = originalDampingValue;
            originalDampings.Remove(rb);
        }
    }

    private void OnValidate()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = !isSolid;
        }
    }
}