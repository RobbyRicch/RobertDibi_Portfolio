using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Experience : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtExpNumber;
    
    [SerializeField]
    private int deExp;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("LastExp"))
        {
            if (PlayerPrefs.GetString("LastExp") != "")
                deExp = int.Parse(PlayerPrefs.GetString("LastExp"));
            else
                deExp = 0;

        }
        else
            deExp = 0;
       
        txtExpNumber.text = "" + deExp;

    }

    private void OnEnable()
    {
        
        Actions.PlayerTaskDone += SetExpAfterTask;
    }
    private void OnDisable()//
    {
        Actions.PlayerTaskDone -= SetExpAfterTask;
    }
    private void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        if (str == "1")
        {

            //PlayerPrefs.DeleteKey("LastExp");
        }
        else
        {
            deExp = int.Parse(txtExpNumber.text);
            PlayerPrefs.SetString("LastExp", "" + deExp);
        }

    }
    private void SetExpAfterTask(bool _isDone)
    {
        if (_isDone)
        {
            //set animation EXP
            SetExp(10);
        }
    }
    public void SetExp(int _deExp)
    {
        if (deExp+_deExp >100)
        {
            Actions.LevelNumberUpdate(+1);

            deExp = 0;
        }
        else
            deExp = deExp+_deExp;
        txtExpNumber.text = "" + deExp;
        
    }
}
