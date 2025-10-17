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

    // 이 총을 소유한 캐릭터의 스탯을 저장할 변수
    private CharacterStats ownerStats;

    // Start is called before the first frame update
    void Start()
    {
        // 이 총의 부모 계층(아마도 Player)에 있는 CharacterStats 컴포넌트를 찾아서 저장합니다.
        ownerStats = GetComponentInParent<CharacterStats>();
    }

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
        // 1. 총알 프리팹을 생성하고, 생성된 게임 오브젝트를 변수에 저장합니다.
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. 생성된 총알 오브젝트에서 Bullet 스크립트 컴포넌트를 가져옵니다.
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();

        // 3. Bullet 스크립트를 성공적으로 찾았고, 주인의 스탯(ownerStats)도 있다면
        if (bulletScript != null && ownerStats != null)
        {
            // 4. 주인의 공격력(ownerStats.attackDamage)으로 총알의 데미지를 초기화합니다.
            bulletScript.Initialize(ownerStats.attackDamage);
        }
    }
    */

    // This function fulfills the 'contract' from the parent Weapon class. // 이 함수는 부모 Weapon 클래스와의 '계약'을 이행함.
    protected override void PerformAttack()
    {
        // Creates (instantiates) a bullet at the firePoint's position and rotation. // firePoint의 위치와 회전값으로 총알을 생성.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
    

}