using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Diamonds : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtDiamondsNumber; 
    [SerializeField]
    private int deDiamonds;
    [SerializeField]
    private Timed_ui timed_UI;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("LastDiamonds"))
        {
            if (PlayerPrefs.GetString("LastDiamonds") != "")
                deDiamonds = int.Parse(PlayerPrefs.GetString("LastDiamonds"));
            else
                deDiamonds = 50;

        }
        else
            deDiamonds = 50;

        SetDiamonds(deDiamonds);
        
    }
    private void OnEnable()
    {
        
    }

    private void OnDisable()//
    {
    }
    private void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        if (str == "1")
        {
           // PlayerPrefs.DeleteKey("LastDiamonds");
        }
        else
        {

            PlayerPrefs.SetString("LastDiamonds", "" + deDiamonds);
        }

    }
    public void SetDiamonds(int _deDiamonds)
    {
        txtDiamondsNumber.text =""+ _deDiamonds;
    }
    public void SetDiamondsMinus(int _deDiamondsMinus)
    {
        if (deDiamonds - _deDiamondsMinus < 0)
        {
            timed_UI.ShowMe_UI();
            return;
        }
        deDiamonds = deDiamonds - _deDiamondsMinus;
        txtDiamondsNumber.text = "" + deDiamonds;
        Actions.EnergyNumberUpdate(+100);
    }
}
