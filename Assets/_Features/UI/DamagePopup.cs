using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    // 움직임 관련 변수
    private Vector3 moveVector;
    private const float DISAPPEAR_TIMER_MAX = 1f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // [수정됨] 데미지와 크리티컬 여부를 받아 스타일 설정
    public void Setup(float damageAmount, bool isCriticalHit)
    {
        textMesh.text = damageAmount.ToString("F0");

        if (!isCriticalHit)
        {
            // [일반] 노란색, 기본 크기
            textMesh.fontSize = 5;
            textColor = Color.yellow;
            textMesh.fontStyle = FontStyles.Normal;
        }
        else
        {
            // [크리티컬] 빨간색, 큰 크기, 굵게
            textMesh.fontSize = 8;
            textColor = Color.red;
            textMesh.fontStyle = FontStyles.Bold; // 굵게 처리 추가
        }

        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        // 위로 튀어 오르는 움직임 (X축 랜덤성을 주면 더 자연스러움)
        moveVector = new Vector3(Random.Range(-0.5f, 0.5f), 1f) * 10f;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        // 위로 이동하며 감속
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        // 커졌다 작아지는 연출
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        // 투명해지며 사라짐
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                // 풀 매니저 반환을 위해 비활성화
                gameObject.SetActive(false);
            }
        }
    }
}