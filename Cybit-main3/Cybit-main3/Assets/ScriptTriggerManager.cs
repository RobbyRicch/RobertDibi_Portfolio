using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTriggerManager : MonoBehaviour
{
    public List<MonoBehaviour> scriptsToManage;
    public GameObject HUD;
    public GameObject cursor;
    public BoxCollider2D PlayerTrigger;
    public bool shouldEnable;
    public bool shouldDisable;
    public bool DisableBoxCollider;


    private void Update()
    {
        if (shouldEnable)
        {
            EnableScripts();
        }

        if (shouldDisable)
        {
            DisableScripts();
        }


    }
    public void EnableScripts()
    {
        foreach (MonoBehaviour script in scriptsToManage)
        {
            script.enabled = true;
        }
            HUD.SetActive(true);
            cursor.SetActive(true);

        if (DisableBoxCollider)
        {
            PlayerTrigger.enabled = true;
        }
    }

    public void DisableScripts()
    {
        foreach (MonoBehaviour script in scriptsToManage)
        {
            script.enabled = false;
        }
            HUD.SetActive(false);
            cursor.SetActive(false);

        if (DisableBoxCollider)
        {
            PlayerTrigger.enabled = false;
        }
    }
}
