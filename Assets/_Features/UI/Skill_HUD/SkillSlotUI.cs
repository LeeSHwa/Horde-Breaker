using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("Components")]
    public Image iconImage;
    public TextMeshProUGUI levelText;
    public GameObject contentParent; 

    public void SetSlot(Sprite icon, int level)
    {
        gameObject.SetActive(true);

        if (contentParent != null) contentParent.SetActive(true);

        if (iconImage != null) iconImage.sprite = icon;
        if (levelText != null) levelText.text = $"Lv.{level}";
    }

    public void ClearSlot()
    {
        gameObject.SetActive(false);
    }
}