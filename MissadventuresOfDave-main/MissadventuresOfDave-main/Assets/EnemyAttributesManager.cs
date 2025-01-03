using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributesManager : MonoBehaviour
{
    [Header("Health")]
    public int _enemyCurrentHP;
    public int _enemyMaxHP;
    public bool _enemyCanTakeDmg;

    [Header("Stamina")]
    public int _enemyCurrentStamina;
    public int _enemyMaxStamina;

    [Header("Damage Flash")]
    [SerializeField] private SpriteRenderer _currentSpriteRenderer;
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healColor;
    [SerializeField] private float _flashTime;


    private void Start()
    {
        _enemyCurrentHP = _enemyMaxHP;
        _enemyCurrentStamina = _enemyMaxStamina;
    }

    public void TakeDamage(int Damage)
    {
        _enemyCurrentHP -= Damage;
        StartCoroutine(DamageFlash(_currentSpriteRenderer, _damageColor, _flashTime));
        if (_enemyCurrentHP <= 0)
        {
            Destroy(gameObject);
        }

    }
    public void HealHP(int HP)
    {
        _enemyCurrentHP += HP;

        StartCoroutine(HealFlash(_currentSpriteRenderer, _healColor, _flashTime));
        if (_enemyCurrentHP > _enemyMaxHP)
        {
            _enemyCurrentHP = _enemyMaxHP;
        }
    }

    public void DrainStamina(int stamina)
    {
        _enemyCurrentStamina -= stamina;
    }

    public void GainStamina(int stamina)
    {
        _enemyCurrentStamina += stamina;

        if (_enemyCurrentStamina > _enemyMaxStamina)
        {
            _enemyCurrentStamina = _enemyMaxStamina;
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
