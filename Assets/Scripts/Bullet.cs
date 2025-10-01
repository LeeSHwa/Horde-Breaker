using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    // ▼▼▼ 바로 이 줄이 필요합니다! ▼▼▼
    // 이 변수가 있어야 Inspector 창에 'Rb'라는 슬롯이 생깁니다.
    public Rigidbody2D rb;

    void Start()
    {
        // rb가 null이 아닌지 확인하는 안전장치를 추가하는 것이 좋습니다.
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
        else
        {
            Debug.LogError("Bullet's Rigidbody2D (rb) is not assigned!");
        }

        Destroy(gameObject, 3f);
    }
}