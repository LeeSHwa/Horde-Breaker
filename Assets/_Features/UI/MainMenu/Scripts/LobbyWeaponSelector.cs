using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LobbyWeaponSelector : MonoBehaviour, IPointerClickHandler
{
    [Header("Data & Prefab")]
    public WeaponDataSO weaponData; 

    public GameObject weaponPrefab;

    [Header("Visual Components")]
    public Image borderImage;

    private WeaponLoadoutUI _manager;

    public void Init(WeaponLoadoutUI manager)
    {
        _manager = manager;
        UpdateVisual(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_manager != null)
        {
            _manager.SelectWeapon(this);
        }

        if (GameManager.Instance != null && weaponPrefab != null)
        {
            GameManager.Instance.SelectWeapon(weaponPrefab);
            Debug.Log($"[Lobby] Weapon Selected: {weaponPrefab.name}");
        }
        else
        {
            if (weaponPrefab == null) Debug.LogWarning("WeaponPrefab is missing in Selector!");
        }
    }

    public void UpdateVisual(bool isSelected)
    {
        if (isSelected)
        {

            if (borderImage != null) borderImage.color = Color.white;

            transform.localScale = Vector3.one * 1.1f;
        }
        else
        {
            if (borderImage != null) borderImage.color = Color.gray;

            transform.localScale = Vector3.one;
        }
    }
}