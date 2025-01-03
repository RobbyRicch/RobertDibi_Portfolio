using UnityEngine;

public class ComboSkillButton : MonoBehaviour
{
    [SerializeField] private SkillType _mainPathType;
    [SerializeField] private SkillType _secondaryPathType;
    [SerializeField] private PassiveSkillTree_Manager skillTreeManager;
    [SerializeField] private int _skillIndex;

    private void Start()
    {
        if (!skillTreeManager)
            skillTreeManager = FindObjectOfType<PassiveSkillTree_Manager>();
    }

    public void OnSkillButtonClick()
    {
        bool isUnlocked = skillTreeManager.UnlockComboSkill(_skillIndex);

        if (isUnlocked)
            GetComponent<UnityEngine.UI.Image>().color = Color.green;
    }

    [ContextMenu("Set By Index")]
    private void SetByIndex()
    {
        switch (_skillIndex)
        {
            case 0:
                _mainPathType = SkillType.Speedster;
                _secondaryPathType = SkillType.Bruiser;
                break;
            case 1:
                _mainPathType = SkillType.Bruiser;
                _secondaryPathType = SkillType.Gunslinger;
                break;
            case 2:
                _mainPathType = SkillType.Bruiser;
                _secondaryPathType = SkillType.Gunslinger;
                break;
            case 3:
                _mainPathType = SkillType.Bruiser;
                _secondaryPathType = SkillType.Speedster;
                break;
            case 4:
                _mainPathType = SkillType.Gunslinger;
                _secondaryPathType = SkillType.Speedster;
                break;
            case 5:
                _mainPathType = SkillType.Gunslinger;
                _secondaryPathType = SkillType.Bruiser;
                break;
        }
    }
}