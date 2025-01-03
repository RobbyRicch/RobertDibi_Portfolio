using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

public class SLIME_CHEAT : MonoBehaviour
{
    private void Start()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        if(str == "1")
            PlayerPrefs.SetString("deletekeyall","0");
    }
    public void BTN_ResetData()
    {
        PlayerPrefs.SetString("PlayerEnergyTimer", "");
        PlayerPrefs.SetString("LastTimer", "");
        PlayerPrefs.SetString("LastLvl", "");
        PlayerPrefs.SetString("LastExp", "");
        PlayerPrefs.SetString("LastDiamonds", "");
        PlayerPrefs.SetString("LastEnergy", "");
        PlayerPrefs.SetString("LastIcon", "");
        PlayerPrefs.SetString("sysString", "");

        Debug.Log("reset keys");
    }
    public void BTN_DeleteData()
    {

        PlayerPrefs.SetString("deletekeyall","1");
        PlayerPrefs.DeleteKey("PlayerEnergyTimer");
        PlayerPrefs.DeleteKey("LastTimer");
        PlayerPrefs.DeleteKey("LastLvl");
        PlayerPrefs.DeleteKey("LastExp");
        PlayerPrefs.DeleteKey("LastDiamonds");
        PlayerPrefs.DeleteKey("LastEnergy");
        PlayerPrefs.DeleteKey("LastIcon");
        PlayerPrefs.DeleteKey("sysString");
        PlayerPrefs.Save();
        Debug.Log("deleted keys next run" + PlayerPrefs.GetString("deletekeyall"));
    }
}
