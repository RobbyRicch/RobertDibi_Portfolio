using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Level : MonoBehaviour
{ //on StripExp gameobject- under UiGame
    [SerializeField]
    private TextMeshProUGUI txtLevelNumber;
    [SerializeField]
    private int deLvl;

    [SerializeField]
    private GameObject LevelupUI;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("LastLvl"))
        {
            if (PlayerPrefs.GetString("LastLvl") != "")
                deLvl = int.Parse(PlayerPrefs.GetString("LastLvl"));
            else
                deLvl = 1;

        }
        else
            deLvl = 1;
        
        txtLevelNumber.text = "" + deLvl;

    }

    private void OnEnable()
    {

        Actions.LevelNumberUpdate += SetLvl;
    }
    private void OnDisable()//
    {
        Actions.LevelNumberUpdate -= SetLvl;
    }
    private void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        //Debug.Log(str + "__");
        if (str == "1")
        {
           // Debug.Log("HasKey: "+PlayerPrefs.HasKey("LastLvl"));
            //PlayerPrefs.DeleteKey("LastLvl");
        }
        else
        {

            deLvl = int.Parse(txtLevelNumber.text);
            PlayerPrefs.SetString("LastLvl", "" + deLvl);

        }

    }

    private void SetLvl(int _deLvl)
    {
        deLvl ++;
        txtLevelNumber.text = ""+deLvl;
        //LevelupUI.SetActive(true);//on ui_manager
    }
}
