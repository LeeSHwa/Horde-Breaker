using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
[RequireComponent(typeof(CircleCollider2D))] // Necessary Component
public class EnemyMovement : MonoBehaviour
{
    [Header("Move Speed")] // Name to Display in Inspector Tab
    public float moveSpeed = 3f; //Moving speed(global variable, modifiable)

    [Header("Weight")] // Name to Display in Inspector Tab
    public float mass = 0.5f; // Weight of the enemy

    private Transform player; // Load the player's Transform component
    private Rigidbody2D rb; // Physics Engine(Rigidbody2D)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Load Physics Engine(Rigidbody2D)
        rb.mass = mass; // Set the weight of the enemy(from Hearder "Weight")
        rb.gravityScale = 0; // Top Down 2D game(no gravity)

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Finding "player" tag
        if (playerObject != null)
        {
            player = playerObject.transform; // Load the player's Transform component
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector3.zero;
            return; // If no player survives on the map, it will stop.
        }

        Vector2 direction = (player.position - transform.position).normalized; // Get the direction from enemy to player
        float faceAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle to See the player
        rb.rotation = faceAngle; // Rotate the enemy to face the player
        rb.linearVelocity = direction * moveSpeed; // Move the enemy towards the player
    }
}
