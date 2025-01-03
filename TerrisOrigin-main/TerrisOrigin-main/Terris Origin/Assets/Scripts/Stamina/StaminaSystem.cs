using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public event EventHandler OnStaminaChanged;

    [SerializeField] private StaminaBar _staminaBar;
    [SerializeField] private int _maxStamina;
    [SerializeField] private int _currentStamina;
    public int CurrentMana { get { return _currentStamina; } }

    [SerializeField] private int _regenerate = 2;
    [SerializeField] private float _regenerateInterval = 1;
    [SerializeField] private float _regenDelay = 2;
    private float _regenDelayTimer;
    private float _regenTimer;

    private void Awake()
    {
        _currentStamina = _maxStamina;
        if (_staminaBar != null) _staminaBar.Setup(this);
    }

    private void Update()
    {
        StaminaRegen();
    }

    private void StaminaRegen()
    {
        if (_currentStamina < _maxStamina && _regenDelayTimer >= _regenDelay)
        {
            if (_regenTimer >= _regenerateInterval)
            {
                _currentStamina += _regenerate;
                _regenTimer = 0;
                if (_currentStamina >= _maxStamina)
                {
                    _currentStamina = _maxStamina;
                    _regenTimer = _regenerateInterval;
                    _regenDelayTimer = 0;
                }
                if (OnStaminaChanged != null) OnStaminaChanged(this, EventArgs.Empty);
            }
            else
            {
                _regenTimer += Time.deltaTime;
            }
        }
        else
        {
            _regenDelayTimer += Time.deltaTime;
        }
    }

    public float GetStaminaPercent()
    {
        return (float)_currentStamina / _maxStamina;
    }

    public bool Decrease(int amount)
    {
        if (amount <= _currentStamina)
        {
            _currentStamina -= amount;
            if (OnStaminaChanged != null) OnStaminaChanged(this, EventArgs.Empty);
            _regenDelayTimer = 0;
            _regenTimer = _regenerateInterval;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Increase(int amount)
    {
        _currentStamina += amount;
        if (_currentStamina >= _maxStamina)
        {
            _currentStamina = _maxStamina;
        }
        if (OnStaminaChanged != null) OnStaminaChanged(this, EventArgs.Empty);
    }

    public void RefillMana()
    {
        _currentStamina = _maxStamina;
        if (OnStaminaChanged != null) OnStaminaChanged(this, EventArgs.Empty);
    }
}
