using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonNew : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IProfileSaveable
{
    public struct PassiveSkillData
    {
        public int Id;
        public string SkillName;
        public StatType StatToModify;

        public int Cost;
        public float Factor;

        public PassiveSkillData(PassiveSkillNewSO passiveSkillData)
        {
            Id = passiveSkillData.Id;
            SkillName = passiveSkillData.SkillName;
            StatToModify = passiveSkillData.StatToModify;
            Cost = passiveSkillData.Cost;
            Factor = passiveSkillData.Factor;
        }
    }

    [Header("Config")]
    [SerializeField] private PassiveSkillNewSO _passiveSkillData; // is updateing somewhere that it shouldnt
    [SerializeField] private Button _button;
    [SerializeField] private int _skillIndex;
    [SerializeField] private int _tooltipID;
    [SerializeField] private bool _isUnlocked;
    [SerializeField] private bool _isPurchased;
    public bool IsPurchased => _isPurchased;

    private PassiveSkillData _skillData;
    public PassiveSkillData SkillData => _skillData;

    private void Start()
    {
        _skillData = new(_passiveSkillData);
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

    public bool UnlockSkill(Player_Data playerData, SkillTreeManagerNew skillManager)
    {
        int skillCost = _skillData.Cost;
        if (!_isPurchased && playerData.Currency >= skillCost)
        {
            _isPurchased = true;
            _button.interactable = false;
            EventManager.InvokePayCurrency(skillCost);
            EventManager.InvokeSkillAquired(_skillData.StatToModify, _skillData.Factor);
            skillManager.DisableUnPurchasedSkills();
            SaveManager.Instance.SaveGame();
            return true;
        }
        else
        {
            Debug.Log($"Couldn't resolve UnlockSkill, components:\b skillIndex: {_skillIndex}, currency: {playerData.Currency}");
            return false;
        }
    }

    public void DisableButton()
    {
        _button.interactable = false;
    }
    public void ResetButton()
    {
        _skillData = new(_passiveSkillData);
        _button.interactable = true;
    }
    public void SaveData(ref Profile data)
    {
        switch (_skillIndex)
        {
            case 0:
                data.MaxHealthFactor = _skillData.Factor;
                break;
            case 1:
                data.MeleeDamageFactor = _skillData.Factor;
                break;
            case 2:
                data.FocusCostFactor = _skillData.Factor;
                break;
        }
    }
    public void LoadData(Profile data)
    {
        float stateFactor = 1.0f;
        switch (_skillIndex)
        {
            case 0:
                stateFactor = data.MaxHealthFactor;
                break;
            case 1:
                stateFactor = data.MeleeDamageFactor;
                break;
            case 2:
                stateFactor = data.FocusCostFactor;
                break;
        }

        _skillData.Factor = stateFactor;
        if (stateFactor != 1.0f)
            _isPurchased = true;
    }
}
