using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDamaged;
    public event EventHandler OnDeath;

    private int _maxHealth;
    private int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } }

    public HealthSystem(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)_currentHealth / _maxHealth;
    }

    public void Damage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            if (OnDeath != null) OnDeath(this, EventArgs.Empty);
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
        if (OnDamaged != null) OnDamaged(this, EventArgs.Empty);
    }

    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth >= _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public void RefillHealth()
    {
        _currentHealth = _maxHealth;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
}
