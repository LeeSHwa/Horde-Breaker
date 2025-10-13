using UnityEngine;

public class Gun : MonoBehaviour
{
    // The bullet prefab to be fired
    public GameObject bulletPrefab;

    // The point where the bullet will be spawned (the muzzle)
    public Transform firePoint;

    // The rate of fire (delay between shots in seconds)
    public float fireRate = 0.5f;

    // A variable to store the time of the next allowed shot
    private float nextFireTime = 0f;


    void Update()
    {
        // If the current game time has passed the next allowed fire time (for automatic firing)
        if (Time.time >= nextFireTime)
        {
            // Update the next fire time (current time + fire rate)
            nextFireTime = Time.time + fireRate;

            // Call the shoot function
            Shoot();
        }
    }

    // Function that fires a bullet
    void Shoot()
    {
        // Creates (instantiates) a bullet at the firePoint's position and rotation.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}