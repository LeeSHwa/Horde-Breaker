using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatSkillSlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI levelText;

    [Header("Buttons")]
    public Button plusButton;
    public Button minusButton;

    // 데이터 저장용
    private LevelUpManager.UpgradeChoice targetChoice;
    private ActiveSkillManager skillManager;

    public void Init(LevelUpManager.UpgradeChoice choice, ActiveSkillManager manager)
    {
        targetChoice = choice;
        skillManager = manager;

        // UI 그리기
        if (nameText != null) nameText.text = choice.GetName();
        if (descText != null) descText.text = choice.GetDescription();
        if (iconImage != null) iconImage.sprite = choice.GetIcon();

        // 버튼 연결
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(OnClickPlus);

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(OnClickMinus);

        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        Skills currentSkill = GetMyActiveSkill();

        if (currentSkill != null)
        {
            if (levelText != null) levelText.text = currentSkill.CurrentLevel.ToString();
        }
        else
        {
            if (levelText != null) levelText.text = "0";
        }
    }

    private void OnClickPlus()
    {
        Skills currentSkill = GetMyActiveSkill();

        if (currentSkill == null)
        {
            // 스킬이 없으면 획득
            if (targetChoice.itemToUpgrade != null)
            {
                // [수정됨] 인터페이스를 MonoBehaviour로 변환해서 gameObject에 접근
                skillManager.EquipSkill(((MonoBehaviour)targetChoice.itemToUpgrade).gameObject);
                Debug.Log($"[Cheat] {targetChoice.GetName()} 획득!");
            }
        }
        else
        {
            // 스킬이 있으면 레벨업
            currentSkill.LevelUp();
            Debug.Log($"[Cheat] {currentSkill.name} 레벨업");
        }
        UpdateSlotUI();
    }

    private void OnClickMinus()
    {
        Skills currentSkill = GetMyActiveSkill();
        if (currentSkill != null)
        {
            // 레벨 다운 (기능이 있다면)
            // currentSkill.LevelDown();
            Debug.Log("레벨 다운 클릭됨");
            UpdateSlotUI();
        }
    }

    private Skills GetMyActiveSkill()
    {
        if (targetChoice.itemToUpgrade == null) return null;

        var currentList = skillManager.GetEquippedSkills();
        foreach (var skill in currentList)
        {
            // [수정됨] 인터페이스를 MonoBehaviour로 변환해서 name에 접근
            if (skill.name.Contains(((MonoBehaviour)targetChoice.itemToUpgrade).name))
            {
                return skill;
            }
        }
        return null;
    }
}