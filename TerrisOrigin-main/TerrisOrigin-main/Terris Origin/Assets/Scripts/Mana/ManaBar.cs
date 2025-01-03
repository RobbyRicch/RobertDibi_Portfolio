using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Image BGManaBar;
    [SerializeField] private Image MaxManaBar;
    [SerializeField] private Image CurrentManaBar1;
    [SerializeField] private Image DamageManaBar1;
    [SerializeField] private Image CurrentManaBar2;
    [SerializeField] private Image DamageManaBar2;
    private ManaSystem _manaSystem;

    [SerializeField] private float _shrinkTimerDelay = 0.1f;
    [SerializeField] private float _shrinkSpeed = 1;
    private float _damagedManaShrinkTimer;

    [SerializeField] private float _hideSpeed = 1;
    private float _hideTimer;


    private void Update()
    {
        ShrinkBar();
    }

    public void Setup(ManaSystem system)
    {
        _manaSystem = system;
        _manaSystem.OnManaChanged += _manaSystem_OnManaChanged;
        RefreshManaBar();
    }

    private void ShrinkBar()
    {
        _damagedManaShrinkTimer -= Time.deltaTime;
        if (_damagedManaShrinkTimer < 0)
        {
            if (CurrentManaBar1.fillAmount < DamageManaBar1.fillAmount)
            {
                DamageManaBar1.fillAmount -= _shrinkSpeed * Time.deltaTime;
                if (DamageManaBar2 != null) DamageManaBar2.fillAmount -= _shrinkSpeed * Time.deltaTime;
            }
            else if (CurrentManaBar1.fillAmount < DamageManaBar1.fillAmount)
            {
                DamageManaBar1.fillAmount = CurrentManaBar1.fillAmount;
                if (DamageManaBar2 != null) DamageManaBar2.fillAmount = CurrentManaBar2.fillAmount;
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
    }

    private void _manaSystem_OnManaChanged(object sender, System.EventArgs e)
    {
        RefreshManaBar();
    }

    private void RefreshManaBar()
    {
        _damagedManaShrinkTimer = _shrinkTimerDelay;
        CurrentManaBar1.fillAmount = _manaSystem.GetManaPercent();
        if (CurrentManaBar2 != null) CurrentManaBar2.fillAmount = _manaSystem.GetManaPercent();
        if (CurrentManaBar1.fillAmount > DamageManaBar1.fillAmount)
        {
            DamageManaBar1.fillAmount = _manaSystem.GetManaPercent();
            if (DamageManaBar2 != null) DamageManaBar2.fillAmount = _manaSystem.GetManaPercent();
        }

        _hideTimer = 0;
    }
}
