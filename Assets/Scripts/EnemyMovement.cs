using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
public class EnemyMovement : MonoBehaviour
{
    [Header("Move Speed")] // Name to Display in Inspector Tab
    public float moveSpeed = 3f; //Moving speed(global variable, modifiable)

    [Header("Distance Limit From Player")] // Name to Display in Inspector Tab
    public float stoppingDistance = 0.5f; // Minimum distance from player(global variable, modifiable)

    private Transform player; // Load the player's Transform component
    private Rigidbody2D rb; // Physics Engine(Rigidbody2D)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Load Physics Engine(Rigidbody2D)

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Calculate the distance between the enemy and the player.
        Vector3 direction = (player.position - transform.position).normalized; // Calculate the direction vector from the enemy to the player & normalize.

        if (distanceToPlayer > stoppingDistance)
        {
            rb.linearVelocity = direction * moveSpeed; // If the mob is far enough away from the player, it will move
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Stop when reaching 'stoppingDistance'
        }
    }
}
