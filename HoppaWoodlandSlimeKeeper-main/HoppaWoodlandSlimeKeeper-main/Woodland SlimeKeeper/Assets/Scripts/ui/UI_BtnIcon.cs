using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_BtnIcon : MonoBehaviour
{ 
    [SerializeField]
    private int deIconNum;
    [SerializeField]
    private bool isSelected;
    private Button btnMe;

    [SerializeField]
    private Sprite deSptite;
    

    private void Start()
    {

        btnMe = GetComponent<Button>();
        btnMe.onClick.AddListener(delegate { isSelected = true; Actions.IconChange(deIconNum);  }); 
        isSelected = false;
        
    }


    
    public void SetIconNum(int _deNum)
    {
        //Debug.Log("PP" + _deNum);
        deIconNum = _deNum;
    }
    public void SetSpriteIcon(Sprite _deSprite)
    {
        //Debug.Log("PP" + _deNum);
        GetComponent<Image>().sprite = _deSprite;
       
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }
}
