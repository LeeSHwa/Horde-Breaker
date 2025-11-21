using UnityEngine;
using System.Collections.Generic; // Required for List

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class ExpOrb : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 40f;        // Speed when moving to player
    public float directPickupRadius = 0.5f; // Distance to be "collected"

    // --- [NEW] Static list to track all active orbs ---
    // This is used for the map-wide magnet item.
    public static readonly List<ExpOrb> ActiveOrbs = new List<ExpOrb>();
    // ---

    private int expValue;
    private StatsController playerStats; // Reference to give EXP
    private Transform targetPlayer;      // The player to move towards
    private bool isMovingToPlayer = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        isMovingToPlayer = false;
        targetPlayer = null;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerStats = playerObj.GetComponent<StatsController>();
        }
        else
        {
            // No player found, deactivate immediately
            // (PoolManager will just circulate it)
            gameObject.SetActive(false);
            return; // Stop further execution
        }

        // [NEW] Add this orb to the active list
        if (!ActiveOrbs.Contains(this))
        {
            ActiveOrbs.Add(this);
        }
    }

    // [NEW] Called when SetActive(false) is used
    void OnDisable()
    {
        // Remove this orb from the active list
        ActiveOrbs.Remove(this);
    }

    // Called by StatsController when spawned
    public void Initialize(int value)
    {
        this.expValue = value;
        UpdateColor(value);
    }

    private void UpdateColor(int value)
    {
        // Example: Change color based on value tiers
        if (value >= 15)
        {
            spriteRenderer.color = new Color(1f, 0.95f, 0.1f);
        }
        else if (value >= 5)
        {
            spriteRenderer.color = new Color(0.3f, 1f, 1f);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
    // Called by PlayerPickup.cs or Magnet Item
    public void ActivateMagnet(Transform playerTarget)
    {
        if (isMovingToPlayer) return;

        this.targetPlayer = playerTarget;
        this.isMovingToPlayer = true;
    }

    void Update()
    {
        if (isMovingToPlayer && targetPlayer != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPlayer.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, targetPlayer.position) <= directPickupRadius)
            {
                CollectExp();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectExp();
        }
    }

    private void CollectExp()
    {
        if (playerStats != null)
        {
            playerStats.AddExp(expValue);
        }

        gameObject.SetActive(false);

    }
}