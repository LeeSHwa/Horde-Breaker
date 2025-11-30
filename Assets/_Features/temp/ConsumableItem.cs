using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    public enum ItemType { Health, Magnet }

    [Header("Item Settings")]
    public ItemType type;
    public float healAmount = 30f;

    [Header("Magnet Settings")]
    public float magnetPullSpeed = 20f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StatsController playerStats = other.GetComponent<StatsController>();

            if (playerStats != null)
            {
                ApplyEffect(playerStats, other.transform);
                gameObject.SetActive(false);
            }
        }
    }

    private void ApplyEffect(StatsController player, Transform playerTransform)
    {
        switch (type)
        {
            case ItemType.Health:
                player.Heal(healAmount);
                break;

            case ItemType.Magnet:
                foreach (var orb in ExpOrb.ActiveOrbs)
                {
                    orb.ActivateMagnet(playerTransform, magnetPullSpeed);
                }
                break;
        }
    }
}