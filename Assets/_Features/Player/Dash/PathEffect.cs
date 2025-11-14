using UnityEngine;

public class PathEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color startColor;
    private float fadeTimer;
    private float fadeSpeed;
    private bool isFading = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    public void Setup(Vector2 direction)
    {
        transform.right = direction;
    }

    public void StartFadeOut(float duration)
    {
        isFading = true;
        fadeTimer = 1.0f;

        if (duration > 0)
        {
            fadeSpeed = 1.0f / duration;
        }
        else
        {
            fadeSpeed = 100f;
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        if (isFading && fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime * fadeSpeed;

            Color c = startColor;
            c.a = fadeTimer;
            sr.color = c;
        }
    }
}