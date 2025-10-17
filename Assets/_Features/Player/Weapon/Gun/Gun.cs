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
            //Shoot(); // Call the Shoot function to fire a bullet
            PerformAttack(); // Call the PerformAttack function to fire a bullet. // PerformAttack 함수를 호출하여 총알을 발사
            nextFireTime = Time.time + fireRate; // Update the next allowed fire time
        }
    }

    /*
    // Function that fires a bullet
    void Shoot()
    {
        // Creates (instantiates) a bullet at the firePoint's position and rotation.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
    */

    // This function fulfills the 'contract' from the parent Weapon class. // 이 함수는 부모 Weapon 클래스와의 '계약'을 이행함.
    protected override void PerformAttack()
    {
        // Creates (instantiates) a bullet at the firePoint's position and rotation. // firePoint의 위치와 회전값으로 총알을 생성.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
    

}