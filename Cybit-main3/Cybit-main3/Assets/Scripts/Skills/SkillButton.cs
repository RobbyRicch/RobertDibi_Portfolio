using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SkillType _pathType;
    [SerializeField] private PassiveSkillTree_Manager skillTreeManager;
    [SerializeField] private int _skillIndex;
    [SerializeField] private int _tooltipID;

    private void Start()
    {
        if (!skillTreeManager)
            skillTreeManager = FindObjectOfType<PassiveSkillTree_Manager>();
    }

    public void OnSkillButtonClick()
    {
        bool isUnlocked = skillTreeManager.UnlockSkill(_pathType, _skillIndex);

        if (isUnlocked)
            GetComponent<UnityEngine.UI.Image>().color = Color.green;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipID == -1) return;
        ToolltipManager.Instance.InstantiateToolTip(_tooltipID, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolltipManager.Instance.DestroyTooltip();
    }
}