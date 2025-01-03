using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using System;

using UnityEngine.UI;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI txtEnergyNumber;
    private int numEnergy;

    [SerializeField]
    private UIEnergyTimer uiEnergyTimer;

    [SerializeField]
    private GameObject TimerGO;

    private void OnEnable()
    {
        Actions.EnergyNumberUpdate += UpdateEnergyAfterTimer;
        if (PlayerPrefs.HasKey("LastEnergy"))
        {
            if (PlayerPrefs.GetString("LastEnergy") != "")
                numEnergy = int.Parse(PlayerPrefs.GetString("LastEnergy"));
            else
                numEnergy = 12;

        }
        else
            numEnergy = 12;
    }
    private void OnDisable()
    {
        Actions.EnergyNumberUpdate -= UpdateEnergyAfterTimer;
        
        
    }

    private void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        //Debug.Log(str + "___");
        if (str == "1")
        {

            //PlayerPrefs.DeleteKey("LastEnergy");
        }
        else
        {
            numEnergy = int.Parse(txtEnergyNumber.text);
            PlayerPrefs.SetString("LastEnergy", "" + numEnergy);
        }
    }
    private void Start()
    {
        //Debug.Log("Start:" + numEnergy);
        SetUI_Energy(numEnergy);
    }

    public void SetUI_Energy(int _newValEnergy)//set
    {
        //Debug.Log("_newValEnergy" + _newValEnergy);
        numEnergy = _newValEnergy;
        if (numEnergy >= 100)
        {
            numEnergy = 100;
            TimerGO.SetActive(false);
            uiEnergyTimer.ReSetCounterTimer();
        }
        else
        {
            TimerGO.SetActive(true);
        }
        txtEnergyNumber.text = "" + numEnergy;
       
        PlayerPrefs.SetString("LastEnergy", "" + numEnergy);
    }
    public int GetUI_Energy()//get
    {
        return numEnergy;
    }

    private void UpdateEnergyAfterTimer(int _addEnergy)
    {
        //Debug.Log("UpdateEnergyAfterTimer");
       
        SetUI_Energy(numEnergy + _addEnergy);
    }

}
