using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    private Vector3 moveVector;
    private const float DISAPPEAR_TIMER_MAX = 1f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount, bool isCriticalHit)
    {
        textMesh.text = damageAmount.ToString("F0");

        if (!isCriticalHit)
        {
            textMesh.fontSize = 5;
            textColor = Color.yellow;
            textMesh.fontStyle = FontStyles.Normal;
        }
        else
        {
            textMesh.fontSize = 8;
            textColor = Color.red;
            textMesh.fontStyle = FontStyles.Bold; 
        }

        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        moveVector = new Vector3(Random.Range(-0.5f, 0.5f), 1f) * 10f;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

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

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}