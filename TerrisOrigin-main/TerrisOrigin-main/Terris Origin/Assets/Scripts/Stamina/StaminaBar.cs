using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image BGStaminaBar;
    [SerializeField] private Image MaxStaminaBar;
    [SerializeField] private Image CurrentStaminaBar1;
    [SerializeField] private Image DamageStaminaBar1;
    [SerializeField] private Image CurrentStaminaBar2;
    [SerializeField] private Image DamageStaminaBar2;
    private StaminaSystem _staminaSystem;

    [SerializeField] private float _shrinkTimerDelay = 0.1f;
    [SerializeField] private float _shrinkSpeed = 1;
    private float _damagedStaminaShrinkTimer;

    [SerializeField] private float _hideSpeed = 1;
    private float _hideTimer;


    private void Update()
    {
        //ShrinkBar();
    }

    public void Setup(StaminaSystem system)
    {
        _staminaSystem = system;
        _staminaSystem.OnStaminaChanged += _staminaSystem_OnStaminaChanged;
        RefreshStaminaBar();
    }

    /*private void ShrinkBar()
    {
        _damagedStaminaShrinkTimer -= Time.deltaTime;
        if (_damagedStaminaShrinkTimer < 0)
        {
            if (CurrentStaminaBar1.fillAmount < DamageStaminaBar1.fillAmount)
            {
                DamageStaminaBar1.fillAmount -= _shrinkSpeed * Time.deltaTime;
                if (DamageStaminaBar2 != null) DamageStaminaBar2.fillAmount -= _shrinkSpeed * Time.deltaTime;
            }
            else if (CurrentStaminaBar1.fillAmount < DamageStaminaBar1.fillAmount)
            {
                DamageStaminaBar1.fillAmount = CurrentStaminaBar1.fillAmount;
                if (DamageStaminaBar2 != null) DamageStaminaBar2.fillAmount = CurrentStaminaBar2.fillAmount;
            }

            if (_hideTimer >= _hideSpeed)
            {
                _hideTimer = 0;
            }
            else
            {
                _hideTimer += Time.deltaTime;
            }
        }
    }*/

    private void _staminaSystem_OnStaminaChanged(object sender, System.EventArgs e)
    {
        RefreshStaminaBar();
    }

    private void RefreshStaminaBar()
    {
        //_damagedStaminaShrinkTimer = _shrinkTimerDelay;
        CurrentStaminaBar1.fillAmount = _staminaSystem.GetStaminaPercent();
        //if (CurrentStaminaBar2 != null) CurrentStaminaBar2.fillAmount = _staminaSystem.GetManaPercent();
        /*if (CurrentStaminaBar1.fillAmount > DamageStaminaBar1.fillAmount)
        {
            DamageStaminaBar1.fillAmount = _staminaSystem.GetStaminaPercent();
            if (DamageStaminaBar2 != null) DamageStaminaBar2.fillAmount = _staminaSystem.GetStaminaPercent();
        }*/

        //_hideTimer = 0;
    }
}
