using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LightningChainVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float duration = 0.35f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Reset coroutine in case of pooling re-use
        StopAllCoroutines();
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}