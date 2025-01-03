using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    [Header("Health")]
     public int _currentHP;
     public int _maxHP;
     public bool _canTakeDmg;

    [Header("Stamina")]
    public int _currentStamina;
    public int _maxStamina;

    [Header("Damage Flash")]
    [SerializeField] private SpriteRenderer _currentSpriteRenderer;
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healColor;
    [SerializeField] private float _flashTime;


    private void Start()
    {
        _currentHP = _maxHP;
        _currentStamina = _maxStamina;
    }

    public void TakeDamage(int Damage)
    {
        _currentHP -= Damage;

        StartCoroutine(DamageFlash(_currentSpriteRenderer, _damageColor, _flashTime));

    }
    public void HealHP(int HP)
    {
        _currentHP += HP;

        StartCoroutine(HealFlash(_currentSpriteRenderer, _healColor, _flashTime));
        if (_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }
    }

    public void DrainStamina(int stamina)
    {
        _currentStamina -= stamina;
    }

    public void GainStamina(int stamina)
    {
        _currentStamina += stamina;

        if (_currentStamina > _maxStamina)
        {
            _currentStamina = _maxStamina;
        }
    }

    public IEnumerator DamageFlash(SpriteRenderer spriteRenderer, Color damageColor, float duration)
    {
       
        Color originalColor = spriteRenderer.color;

        
        float halfDuration = duration / 2;

        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, damageColor, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        spriteRenderer.color = damageColor;

        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            spriteRenderer.color = Color.Lerp(damageColor, originalColor, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        
        spriteRenderer.color = originalColor;
    }

    public IEnumerator HealFlash(SpriteRenderer spriteRenderer, Color healColor, float duration)
    {
        
        Color originalColor = spriteRenderer.color;

        
        float halfDuration = duration / 2;

        
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, healColor, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        
        spriteRenderer.color = healColor;

        
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            spriteRenderer.color = Color.Lerp(healColor, originalColor, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        
        spriteRenderer.color = originalColor;
    }

}
