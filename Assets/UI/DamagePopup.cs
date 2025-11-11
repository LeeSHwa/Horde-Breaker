using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    // Movement direction vector
    private Vector3 moveVector;
    private const float DISAPPEAR_TIMER_MAX = 1f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Call this method to setup the popup
    // Call this method to setup the popup
    public void Setup(float damageAmount, bool isCriticalHit)
    {
        string textToDisplay = damageAmount.ToString("F0");

        if (damageAmount < 30)
        {
            textMesh.fontSize = 5;
            textColor = Color.white;
        }
        else if (damageAmount < 70)
        {
            textMesh.fontSize = 6;
            textColor = Color.yellow;
        }
        else if (damageAmount < 100)
        {
            textMesh.fontSize = 7;
            textColor = new Color(1f, 0.5f, 0f);
        }
        else // damageAmount >= 100
        {
            textMesh.fontSize = 8;
            textColor = Color.red;
            textToDisplay = "<b>" + textToDisplay + "</b>";
        }


        textMesh.text = textToDisplay; 
        textMesh.color = textColor;

        disappearTimer = DISAPPEAR_TIMER_MAX;
        moveVector = new Vector3(0, 3f, 0) * 2f;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        // Move upwards
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime; // Decelerate quickly

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            // First half of lifetime: Pop up scale effect
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            // Second half of lifetime: Shrink scale effect
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start fading out
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // When completely invisible, return to pool
            if (textColor.a < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}