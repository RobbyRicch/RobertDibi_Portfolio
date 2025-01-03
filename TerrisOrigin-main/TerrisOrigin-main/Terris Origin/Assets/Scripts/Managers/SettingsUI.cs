using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public TMP_Text temp;
    public string Jump;
    public GameObject inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setKeyBind()
    {
        temp = GetComponentInChildren<TMP_Text>();
        print(temp.text);
    }
}
