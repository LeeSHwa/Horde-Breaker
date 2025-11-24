using UnityEngine;

public class BackgroundShaker : MonoBehaviour
{
    
    public float shakeAmount = 5f;

    public float shakeSpeed = 20f;

    public bool smoothMotion = false;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.localPosition;

        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (smoothMotion)
        {
            float x = Mathf.PerlinNoise(Time.time * shakeSpeed + timeOffset, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * shakeSpeed + timeOffset) * 2f - 1f;

            transform.localPosition = startPosition + new Vector3(x, y, 0) * shakeAmount;
        }
        else
        {
            Vector3 randomPoint = startPosition + (Vector3)(Random.insideUnitCircle * shakeAmount);

            transform.localPosition = Vector3.Lerp(transform.localPosition, randomPoint, Time.deltaTime * shakeSpeed);
        }
    }
}