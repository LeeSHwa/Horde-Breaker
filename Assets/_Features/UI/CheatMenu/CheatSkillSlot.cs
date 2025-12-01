using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatSkillSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI titleText; 
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI descText;  

    public Button plusButton;
    public Button minusButton;

    private SkillCheatManager manager;
    private GameObject skillPrefab;
    private Skills activeInstance;

    private void Start()
    {
        if (plusButton) plusButton.onClick.AddListener(OnPlusClicked);
        if (minusButton) minusButton.onClick.AddListener(OnMinusClicked);
    }

    public void Setup(SkillCheatManager manager, GameObject prefab, Skills instance)
    {
        this.manager = manager;
        this.skillPrefab = prefab;
        this.activeInstance = instance;

        gameObject.SetActive(true);
        RefreshUI();
    }

    private void RefreshUI()
    {
        Skills dataRef = (activeInstance != null) ? activeInstance : skillPrefab.GetComponent<Skills>();

        if (iconImage != null) iconImage.sprite = dataRef.GetIcon();

        if (activeInstance == null)
        {
            if (titleText) titleText.text = dataRef.GetName();
            if (levelText) levelText.text = "Lv.0";
            if (descText) descText.text = "Click (+) to Equip";

            if (iconImage) iconImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            if (titleText) titleText.text = activeInstance.GetName();

            if (levelText) levelText.text = $"Lv.{activeInstance.CurrentLevel}";

            if (iconImage) iconImage.color = Color.white;

            if (descText)
            {
                descText.text = activeInstance.GetCurrentLevelDescription();
            }
        }
    }

    private void OnPlusClicked()
    {
        if (activeInstance == null)
        {
            manager.ForceEquipSkill(skillPrefab);
        }
        else
        {
            activeInstance.Cheat_SetLevel(activeInstance.CurrentLevel + 1);
            RefreshUI();
        }
    }

    private void OnMinusClicked()
    {
        if (activeInstance == null) return;

        if (activeInstance.CurrentLevel <= 1)
        {
            manager.ForceRemoveSkill(activeInstance);

            activeInstance = null;

            RefreshUI();
        }
        else
        {
            activeInstance.Cheat_SetLevel(activeInstance.CurrentLevel - 1);
            RefreshUI();
        }
    }

}