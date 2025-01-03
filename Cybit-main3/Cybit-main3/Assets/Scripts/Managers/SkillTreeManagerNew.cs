using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeManagerNew : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private SkillButtonNew[] _skills;
    [SerializeField] private TextMeshProUGUI _currencyTMP;
    [SerializeField] private TextMeshProUGUI _additionalHealthTMP;
    [SerializeField] private TextMeshProUGUI _meleeDamageBonusTMP;
    [SerializeField] private TextMeshProUGUI _focusCostReductionTMP;

    [Header("Data")]
    //[SerializeField] private Color[] _treeColors = new Color[] { Color.white, Color.white, Color.white };
    private Player_Controller _playerController;
    private float tempTimeScale = 0.0f;

    private void OnEnable()
    {
        EventManager.InvokePause();
        tempTimeScale = Time.timeScale;
        if (_playerController) UpdateUI();
    }
    private void Start()
    {
        InitializeStats();
        EventManager.OnVendingMachineInteract += OnVendingMachineInteract;
    }
    private void OnDisable()
    {
        ToolltipManager.Instance.DestroyTooltip();
        if (_playerController) UpdateUI();
    }
    private void OnDestroy()
    {
        EventManager.OnVendingMachineInteract -= OnVendingMachineInteract;
    }

    private void InitializeStats()
    {
        _focusCostReductionTMP.text = "-";
        _additionalHealthTMP.text = "-";
        _meleeDamageBonusTMP.text = "-";
    }
    private string FormatPercentage(int value)
    {
        string sign = (value >= 1) ? "+" : "-";
        int percentage = Mathf.Abs(Mathf.RoundToInt((value - 1) * 100));
        return sign + percentage.ToString() + "%";
    }
    private string FormatPercentage(float value)
    {
        string sign = (value >= 1) ? "+" : "-";
        int percentage = Mathf.Abs(Mathf.RoundToInt((value - 1) * 100));
        return sign + percentage.ToString() + "%";
    }

    private void UpdateUI()
    {
        _currencyTMP.text = _playerController.Data.Currency.ToString();

        _additionalHealthTMP.text = FormatPercentage(_playerController.Data.MaxHealthFactor);
        _meleeDamageBonusTMP.text = FormatPercentage(_playerController.Data.MeleeDamageFactor);
        _focusCostReductionTMP.text = FormatPercentage(_playerController.Data.FocusCostFactor);
    }

    public void OpenMenu(Player_Controller player)
    {
        _playerController = player;
        UpdateUI();
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventManager.InvokePause();
        tempTimeScale = Time.timeScale;
        Time.timeScale = 0;
        EventManager.InvokeCloseObjective(true);
    }
    public void CloseMenu()
    {
        EventManager.InvokePause();
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _playerController.IsInputDisabled = false;
        Time.timeScale = tempTimeScale;
        EventManager.InvokeReOpenObjective(true);
    }

    public void UnlockSkill(int skillIndex)
    {
        _skills[skillIndex].UnlockSkill(_playerController.Data, this);
        UpdateUI();
    }

    public void DisableUnPurchasedSkills()
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            if (!_skills[i].IsPurchased)
                _skills[i].DisableButton();
        }
    }
    public void ResetAllSkills()
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            _skills[i].ResetButton();
        }
    }
    private void OnVendingMachineInteract(Player_Controller player)
    {
        _currencyTMP.text = player.Data.Currency.ToString();
        if (!gameObject.activeInHierarchy)
            OpenMenu(player);
        else
            CloseMenu();
    }
}