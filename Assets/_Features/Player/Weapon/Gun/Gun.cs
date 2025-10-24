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

    private CharacterStats ownerStats;

    // Start is called before the first frame update
    void Start()
    {
        ownerStats = GetComponentInParent<CharacterStats>();
    }

    // This function fulfills the 'contract' from the parent Weapon class.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        GameObject bulletObject = Instantiate(bulletPrefab, aim.position, aim.rotation);

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(damage);
        }


    }
    

}