using UnityEngine;

public class PathEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color startColor;
    private float fadeTimer; // (1.0 -> 0.0)
    private float fadeSpeed;
    private bool isFading = false; // '사라지기' 시작 스위치

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    // 1. PlayerDash가 '방향'을 설정하기 위해 호출
    public void Setup(Vector2 direction)
    {
        transform.right = direction;
    }

    // 2. PlayerDash가 '대쉬 끝났으니 사라져!'라고 호출
    public void StartFadeOut(float duration)
    {
        isFading = true;    // '사라지기' 스위치 ON
        fadeTimer = 1.0f;   // (1.0 = 불투명)

        // 0이 아닌 duration으로 나누기 (0으로 나누기 방지)
        if (duration > 0)
        {
            fadeSpeed = 1.0f / duration; // duration 시간 동안 1.0 -> 0.0이 되도록 속도 계산
        }
        else
        {
            fadeSpeed = 100f; // 0초면 그냥 엄청 빨리 사라지게
        }

        Destroy(gameObject, duration); // duration 시간 뒤에 확실히 파괴 예약
    }

    void Update()
    {
        // 3. '사라지기' 스위치가 켜졌고, 아직 투명해지지 않았다면
        if (isFading && fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime * fadeSpeed;

            Color c = startColor;
            c.a = fadeTimer; // 알파(투명도) 값을 줄임
            sr.color = c;
        }
    }
}