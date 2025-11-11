using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(StatsController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform aimObject;       // Reference to the 'aim' GameObject in scene
    public Transform weaponHolder;    // Parent transform for instantiating weapons
    public GameObject startingWeaponPrefab; // Prefab to equip at start

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera mainCamera;
    private StatsController stats;

    // [ADDED] Reference to the PlayerAnimator
    private PlayerAnimator playerAnimator;

    // The currently equipped weapon instance
    private Weapon currentWeapon;

    // Public property for other scripts to access aim direction if needed
    public Vector2 AimDirection { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<StatsController>();
        // [ADDED] Get the PlayerAnimator component
        playerAnimator = GetComponent<PlayerAnimator>();
        mainCamera = Camera.main;

        AimDirection = Vector2.right;

        // Equip starting weapon if assigned
        if (startingWeaponPrefab != null)
        {
            EquipWeapon(startingWeaponPrefab);
        }
    }

    void Update()
    {
        // 1. Handle Movement Input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // 2. Calculate Aim Direction
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        ).normalized;
        AimDirection = direction;

        // 3. Update & Attack with Current Weapon
        if (currentWeapon != null)
        {
            // Always update aim visual
            currentWeapon.UpdateAim(AimDirection);

            // Check for attack input (e.g., left mouse button)
            // For auto-attack, remove this if-check and just call TryAttack

            currentWeapon.TryAttack(AimDirection);

        }
    }

    private void FixedUpdate()
    {
        // Physics-based movement
        // Assuming 'linearVelocity' is a valid property in your Unity version (Unity 6+)
        // older versions use 'rb.velocity'
        rb.linearVelocity = moveInput * stats.currentMoveSpeed;
    }

    // Method to equip a new weapon from a prefab
    public void EquipWeapon(GameObject newWeaponPrefab)
    {
        // 1. Destroy old weapon if it exists
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // 2. Instantiate new weapon as a child of weaponHolder
        GameObject weaponObj = Instantiate(newWeaponPrefab, weaponHolder.position, Quaternion.identity, weaponHolder);

        // 3. Get Weapon component and initialize it
        currentWeapon = weaponObj.GetComponent<Weapon>();
        if (currentWeapon != null)
        {
            // [MODIFIED] Inject dependencies: aim object, stats controller, AND player animator
            currentWeapon.Initialize(this.aimObject, this.stats, this.playerAnimator);
        }
        else
        {
            Debug.LogError("Equipped prefab does not have a Weapon component!");
        }
    }
}