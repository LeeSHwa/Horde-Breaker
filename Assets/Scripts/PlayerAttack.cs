using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Tooltip("플레이어가 사용할 무기")]
    public Transform weaponTransform;

    [Tooltip("발사할 총알 프리팹")]
    public GameObject bulletPrefab;

    [Tooltip("초당 발사 횟수")]
    public float fireRate = 5f;

    private Transform firePoint;
    private float nextFireTime = 0f;

    void Start()
    {
        // 무기 오브젝트에서 발사 위치(FirePoint)를 찾아 저장합니다.
        // FirePoint 오브젝트의 이름이 "FirePoint"여야 합니다.
        firePoint = weaponTransform.Find("FirePoint");
    }

    void Update()
    {
        // 자동 발사 로직
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}