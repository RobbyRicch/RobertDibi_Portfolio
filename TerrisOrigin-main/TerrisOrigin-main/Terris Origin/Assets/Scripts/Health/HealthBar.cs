using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image BGHealthBar;
    [SerializeField] private Image MaxHealthBar;
    [SerializeField] private Image CurrentHealthBar1;
    [SerializeField] private Image DamageHealthBar1;
    [SerializeField] private Image CurrentHealthBar2;
    [SerializeField] private Image DamageHealthBar2;
    [SerializeField] private bool DisplayInCircle;
    private HealthSystem _healthSystem;

    [SerializeField] private float _shrinkTimerDelay = 0.1f;
    [SerializeField] private float _shrinkSpeed = 1;
    private float _damagedHealthShrinkTimer;

    [SerializeField] private float _hideSpeed = 1;
    private float _hideTimer;
    [SerializeField] bool _useTransparency = true;


    private void Awake()
    {
        if (DisplayInCircle)
        {
            CurrentHealthBar1.rectTransform.localScale = Vector3.zero;
            DamageHealthBar1.rectTransform.localScale = Vector3.zero;
        }

        SetTransparency(0);
    }

    private void Update()
    {
        ShrinkBar();
    }


    private void ShrinkBar()
    {
        _damagedHealthShrinkTimer -= Time.deltaTime;
        if (_damagedHealthShrinkTimer < 0)
        {
            if (!DisplayInCircle)
            {
                if (CurrentHealthBar1.fillAmount < DamageHealthBar1.fillAmount)
                {
                    DamageHealthBar1.fillAmount -= _shrinkSpeed * Time.deltaTime;
                    if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount -= _shrinkSpeed * Time.deltaTime;
                }
                else if (CurrentHealthBar1.fillAmount < DamageHealthBar1.fillAmount)
                {
                    DamageHealthBar1.fillAmount = CurrentHealthBar1.fillAmount;
                    if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount = CurrentHealthBar2.fillAmount;
                }

                if (_hideTimer >= _hideSpeed)
                {
                    SetTransparency(0);
                    _hideTimer = 0;
                }
                else
                {
                    _hideTimer += Time.deltaTime;
                }
            }
            else
            {
                if (CurrentHealthBar1.rectTransform.localScale.x < DamageHealthBar1.rectTransform.localScale.x)
                {
                    CurrentHealthBar1.rectTransform.localScale += Vector3.one * (1 - _healthSystem.GetHealthPercent()) * _shrinkSpeed * Time.deltaTime;
                }
                else
                {
                    if (_hideTimer >= _hideSpeed)
                    {
                        SetTransparency(0);
                        _hideTimer = 0;
                    }
                    else
                    {
                        _hideTimer += Time.deltaTime;
                    }
                }
            }
        }
    }

    public void Setup(HealthSystem healthSystem)
    {
        this._healthSystem = healthSystem;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        if (!DisplayInCircle)
        {
            _damagedHealthShrinkTimer = _shrinkTimerDelay;
            CurrentHealthBar1.fillAmount = _healthSystem.GetHealthPercent();
            if (CurrentHealthBar2 != null) CurrentHealthBar2.fillAmount = _healthSystem.GetHealthPercent();
            if (CurrentHealthBar1.fillAmount > DamageHealthBar1.fillAmount)
            {
                DamageHealthBar1.fillAmount = _healthSystem.GetHealthPercent();
                if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount = _healthSystem.GetHealthPercent();
            }
        }
        else
        {
            _damagedHealthShrinkTimer = _shrinkTimerDelay;
            DamageHealthBar1.rectTransform.localScale = Vector3.one * (1 - _healthSystem.GetHealthPercent());
            if (DamageHealthBar1.rectTransform.localScale.x < CurrentHealthBar1.rectTransform.localScale.x)
            {
                CurrentHealthBar1.rectTransform.localScale = Vector3.one * (1 - _healthSystem.GetHealthPercent());
            }
        }

        _hideTimer = 0;
        SetTransparency(0.6f);
    }

    private void SetTransparency(float a)
    {
        if (_useTransparency)
        {
            Color tempCol = MaxHealthBar.color;
            tempCol.a = a;
            MaxHealthBar.color = tempCol;

            tempCol = CurrentHealthBar1.color;
            tempCol.a = a;
            CurrentHealthBar1.color = tempCol;

            tempCol = DamageHealthBar1.color;
            tempCol.a = a;
            DamageHealthBar1.color = tempCol;

            if (BGHealthBar)
            {
                tempCol = BGHealthBar.color;
                tempCol.a = a;
                BGHealthBar.color = tempCol;
            }

            if (CurrentHealthBar2)
            {
                tempCol = CurrentHealthBar2.color;
                tempCol.a = a;
                CurrentHealthBar2.color = tempCol;
            }            
            
            if (DamageHealthBar2)
            {
                tempCol = DamageHealthBar2.color;
                tempCol.a = a;
                DamageHealthBar2.color = tempCol;
            }
        }
    }
}
