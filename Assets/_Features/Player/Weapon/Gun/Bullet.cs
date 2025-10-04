using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Speed of the bullet and the component that will control its physics
    public float speed = 20f;
    public Rigidbody2D rb;

    // Define the bullet's damage and knockback force
    public float damage;
    public float knockbackForce;

    void Start()
    {
        // Check if the component is assigned (to prevent errors)
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
        Destroy(gameObject, 3f);
    }

    // Function called when the bullet's collider hits another collider (one of them must be a trigger)
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            //Enemy enemy = hitInfo.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //  enemy.TakeDamage(damage);
            //}

            // Get the Rigidbody2D component from the Enemy object that was hit.
            Rigidbody2D enemyRb = hitInfo.GetComponent<Rigidbody2D>();

            // If the component was successfully retrieved?
            if (enemyRb != null)
            {
                // Apply knockback in the bullet's direction, using an instant force impulse.
                enemyRb.AddForce(transform.right * knockbackForce, ForceMode2D.Impulse);
            }
        }
        // Destroy the bullet immediately, whether it hit an enemy or any other trigger object.
        Destroy(gameObject);
    }
}