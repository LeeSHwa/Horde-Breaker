using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WeaponLoadoutUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public GameObject startButton;

    [Header("Selectors")]
    public List<LobbyWeaponSelector> weaponSelectors;

    private LobbyWeaponSelector _currentSelectedSlot;

    void Start()
    {
        foreach (var selector in weaponSelectors)
        {
            selector.Init(this);
        }

        if (weaponSelectors.Count > 0)
        {
            SelectWeapon(weaponSelectors[0]);
        }
    }

    public void SelectWeapon(LobbyWeaponSelector slot)
    {
        if (_currentSelectedSlot == slot) return;

        _currentSelectedSlot = slot;

        foreach (var selector in weaponSelectors)
        {
            selector.UpdateVisual(selector == slot);
        }

        WeaponDataSO data = slot.weaponData;

        if (data != null)
        {
            nameText.text = $"-==={data.weaponName}===-";

            string stats = $"\n[Stats]\nDamage: {data.baseDamage}\nCooldown: {data.baseAttackCooldown}s";

            descText.text = data.weaponDescription + stats;

            if (GameManager.Instance != null && slot.weaponPrefab != null)
            {
                GameManager.Instance.selectedWeaponPrefab = slot.weaponPrefab;
            }
        }

        if (startButton != null) startButton.SetActive(true);
    }
}