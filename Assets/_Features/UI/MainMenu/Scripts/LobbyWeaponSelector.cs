using UnityEngine;

public class LobbyWeaponSelector : MonoBehaviour
{
    public GameObject weaponPrefab;

    public void SelectWeapon()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.selectedWeaponPrefab = weaponPrefab;
        }
        else
        {
            Debug.LogError("GameManager is not in scene!");
        }
    }
}