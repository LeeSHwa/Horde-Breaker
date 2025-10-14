using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
[RequireComponent(typeof(CircleCollider2D))] // Necessary Component
public class EnemyMovement : MonoBehaviour
{
    [Header("Weight")] // Name to Display in Inspector Tab
    public float mass = 0.5f; // Weight of the enemy

    private CharacterStats stats; // Load CharacterStats script

    private Transform player; // Load the player's Transform component
    private Rigidbody2D rb; // Physics Engine(Rigidbody2D)

    // --- KNOCKBACK FIX ---
    // A flag to control whether the AI can move on its own.
    // --- 넉백 수정 ---
    // AI가 스스로 움직일 수 있는지 제어하는 플래그.
    private bool canMove = true;

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
        stats = GetComponent<CharacterStats>(); // Get the CharacterStats component

    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (player == null)
            {
                rb.linearVelocity = Vector3.zero;
                return; // If no player survives on the map, it will stop.
            }

            Vector2 direction = (player.position - transform.position).normalized; // Get the direction from enemy to player
                                                                                   //float faceAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle to See the player
                                                                                   //rb.rotation = faceAngle; // Rotate the enemy to face the player
            rb.linearVelocity = direction * stats.moveSpeed; // Move the enemy towards the player
        }
    }
    // --- KNOCKBACK FIX ---
    // A public function that can be called by other scripts (like MeleeWeapon).
    // --- 넉백 수정 ---
    // 다른 스크립트(MeleeWeapon 등)에서 호출할 수 있는 public 함수.
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // Stop the coroutine if it's already running to reset the knockback.
        // 넉백을 초기화하기 위해 이미 실행 중인 코루틴이 있다면 멈춤.
        StopAllCoroutines();
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force, float duration)
    {
        // 1. Disable AI movement.
        // 1. AI의 움직임을 비활성화.
        canMove = false;

        // 2. Apply the knockback force.
        // 2. 넉백 힘을 적용.
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // 3. Wait for the knockback duration.
        // 3. 넉백 지속 시간만큼 대기.
        yield return new WaitForSeconds(duration);

        // 4. Re-enable AI movement.
        // 4. AI의 움직임을 다시 활성화.
        canMove = true;
    }
}
