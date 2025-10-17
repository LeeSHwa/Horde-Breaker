using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    public float knockbackForce;

    private float damage;

    public void Initialize(float damageAmount)
    {
        this.damage = damageAmount;
    }

    void Start()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            CharacterStats stats = hitInfo.GetComponent<CharacterStats>();

            if (stats != null)
            {
                stats.TakeDamage(damage);
            }

            Rigidbody2D enemyRb = hitInfo.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(transform.right * knockbackForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}