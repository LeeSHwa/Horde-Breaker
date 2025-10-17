using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 총알의 속도와 물리 제어를 위한 컴포넌트
    public float speed = 20f;
    public Rigidbody2D rb;

    // 총알의 넉백 힘
    public float knockbackForce;

    // public float damage; // 이 줄은 더 이상 사용하지 않습니다.
    private float damage; // 외부에서 전달받은 데미지를 저장할 내부 변수

    /// <summary>
    /// 총알이 생성될 때 공격력을 설정하는 함수입니다.
    /// Gun.cs와 같은 다른 스크립트에서 호출됩니다.
    /// </summary>
    /// <param name="damageAmount">총알에 설정할 데미지 값</param>
    public void Initialize(float damageAmount)
    {
        this.damage = damageAmount;
    }

    void Start()
    {
        // Rigidbody2D 컴포넌트가 할당되었는지 확인 (오류 방지)
        if (rb != null)
        {
            // 총알의 오른쪽 방향(발사 방향)으로 속도를 지정
            rb.linearVelocity = transform.right * speed;
        }
        // 3초 뒤에 총알이 자동으로 파괴되도록 설정
        Destroy(gameObject, 3f);
    }

    // 다른 Collider와 충돌했을 때 호출되는 함수 (둘 중 하나는 IsTrigger여야 함)
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 부딪힌 대상의 태그가 "Enemy"인지 확인
        if (hitInfo.CompareTag("Enemy"))
        {
            // 대상으로부터 CharacterStats 컴포넌트를 가져옴
            CharacterStats stats = hitInfo.GetComponent<CharacterStats>();

            // CharacterStats 컴포넌트가 있다면 데미지를 줌
            if (stats != null)
            {
                // Initialize 함수로 설정된 damage 값을 사용합니다.
                stats.TakeDamage(damage);
            }

            // 대상으로부터 Rigidbody2D 컴포넌트를 가져와 넉백 효과를 줌
            Rigidbody2D enemyRb = hitInfo.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // 총알의 진행 방향으로 즉발적인 힘(Impulse)을 가함
                enemyRb.AddForce(transform.right * knockbackForce, ForceMode2D.Impulse);
            }
        }

        // 총알이 적이든 다른 어떤 트리거든 부딪히면 즉시 파괴
        Destroy(gameObject);
    }
}