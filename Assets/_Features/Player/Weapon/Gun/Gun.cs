using UnityEngine;

public class Gun : Weapon
{
    // The bullet prefab to be fired
    public GameObject bulletPrefab;

    // The point where the bullet will be spawned (the muzzle)
    public Transform firePoint;

    // The rate of fire (delay between shots in seconds)
    public float fireRate = 0.5f;

    // A variable to store the time of the next allowed shot
    private float nextFireTime = 0f;

    public override void TryAttack()
    {
        // Check if enough time has passed since the last shot
        if (Time.time >= nextFireTime)
        {
            Shoot(); // Call the Shoot function to fire a bullet
            nextFireTime = Time.time + fireRate; // Update the next allowed fire time
        }
    }

    // Function that fires a bullet
    void Shoot()
    {
        // Creates (instantiates) a bullet at the firePoint's position and rotation.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}