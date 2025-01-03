using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _costTMP;

    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private int _cost;

    private void OnEnable()
    {
        ShowTooltip(_title, _description, _cost);
    }

    private void ShowTooltip(string titleText, string descriptionText, int cost)
    {
        _titleTMP.text = titleText;
        _descriptionTMP.text = descriptionText;
        _costTMP.text = cost.ToString();
        gameObject.SetActive(true);
    }
    private void HideTooltip()
    {
        gameObject.SetActive(false);
        _titleTMP.text = string.Empty;
        _descriptionTMP.text = string.Empty;
    }
}
