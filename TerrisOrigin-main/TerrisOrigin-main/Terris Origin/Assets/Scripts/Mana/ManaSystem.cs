using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public event EventHandler OnManaChanged;

    [SerializeField] private ManaBar _manaBar;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _currentMana;
    public int CurrentMana { get { return _currentMana; } }

    [SerializeField] private int _regenerate = 2;
    [SerializeField] private int _regenerateInterval = 1;
    [SerializeField] private float _regenDelay = 2;
    private float _regenDelayTimer;
    private float _regenTimer;

    private void Awake()
    {
        _currentMana = _maxMana;
        if (_manaBar != null) _manaBar.Setup(this);
    }

    private void Update()
    {
        ManaRegen();
    }

    private void ManaRegen()
    {
        if (_currentMana < _maxMana && _regenDelayTimer >= _regenDelay)
        {
            if (_regenTimer >= _regenerateInterval)
            {
                _currentMana += _regenerate;
                _regenTimer = 0;
                if (_currentMana >= _maxMana)
                {
                    _currentMana = _maxMana;
                    _regenTimer = _regenerateInterval;
                    _regenDelayTimer = 0;
                }
                if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
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

    public float GetManaPercent()
    {
        return (float)_currentMana / _maxMana;
    }

    public bool Decrease(int amount)
    {
        if (amount <= _currentMana)
        {
            _currentMana -= amount;
            if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
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
        _currentMana += amount;
        if (_currentMana >= _maxMana)
        {
            _currentMana = _maxMana;
        }
        if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
    }

    public void RefillMana()
    {
        _currentMana = _maxMana;
        if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
    }
}
