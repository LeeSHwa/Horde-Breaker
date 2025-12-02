using UnityEngine;
using System.Collections;

// This script handles the visual effect for the player's dash trail.
// It's managed by the PoolManager and controlled by PlayerDash.cs.
[RequireComponent(typeof(SpriteRenderer))]
public class DashEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private float fadeDuration; // The total time it takes for this effect to fade out.

    void Awake()
    {
        // Cache the SpriteRenderer component for performance.
        sr = GetComponent<SpriteRenderer>();
    }

    // Called by PlayerDash.cs right after getting this object from the pool.
    public void Initialize(Sprite sprite, bool flipX, int sortingOrder, Material ghostMaterial, float duration)
    {
        // 1. Copy the player's current visual state.
        sr.sprite = sprite;
        sr.flipX = flipX;
        sr.material = ghostMaterial;

        // 2. Set the sorting order to be just behind the player.
        sr.sortingOrder = sortingOrder - 1;

        // 3. Reset the color to opaque white for the fade-out.
        sr.color = Color.white;

        this.fadeDuration = duration;

        // 4. (Important) Stop any previous coroutines if this object is being reused from the pool.
        StopAllCoroutines();

        // 5. Start the new fade-out process.
        StartCoroutine(FadeOutCoroutine());
    }

    // Coroutine to gradually fade the sprite to transparent.
    private IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;
        Color startColor = Color.white;

        // Target color (fully transparent)
        Color endColor = new Color(1, 1, 1, 0);

        while (timer < fadeDuration)
        {
            // Lerp the color from start to end based on the timer.
            sr.color = Color.Lerp(startColor, endColor, timer / fadeDuration);

            // Increment timer and wait for the next frame.
            timer += Time.deltaTime;
            yield return null;
        }

        // 6. Ensure the color is set to fully transparent when done.
        sr.color = endColor;

        // 7. (Important) Return this object to the pool by deactivating it.
        //    (This replaces Destroy(gameObject))
        gameObject.SetActive(false);
    }
}