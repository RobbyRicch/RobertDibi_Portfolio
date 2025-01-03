using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrier_System : MonoBehaviour
{
    #region Stats
    [Header("Barrier Base Stats")]
    [SerializeField] private int _currentLife;
    [SerializeField] private int _maxLife;
    [SerializeField] public float _timeToRaise;
    [SerializeField] public float _timeToLower;
    [SerializeField] public float _regenDelay; // New field for regen delay
    #endregion

    #region States
    [Header("Barrier States")]
    [SerializeField] public bool _isActive;
    [SerializeField] public bool _shouldActivate;
    [SerializeField] public bool _canBeDamaged;
    [SerializeField] public bool _canRegen;
    [SerializeField] public bool _hasInactiveState;
    [SerializeField] public bool _isInactive;
    [SerializeField] private bool _isFlashing;
    private bool _isRaisingBarrier = false;
    private Coroutine _regenCoroutine; // Coroutine reference for regen

    [SerializeField] private List<Sprite> _stageOneState;
    [SerializeField] private List<Sprite> _stageTwoState;
    [SerializeField] private List<Sprite> _stageThreeState;
    [SerializeField] private List<Sprite> _stageFourState;
    #endregion

    #region References
    [Header("Barrier References")]
    [SerializeField] private BoxCollider2D _barrierCollider;
    [SerializeField] private Animator _barrierAnimator;
    [SerializeField] private List<SpriteRenderer> _spriteRenderers;
    [SerializeField] private GameObject _inactiveStateGO;
    [SerializeField] private AudioSource _barrierAudioSource;
    [SerializeField] private AudioClip _activateBarrierAC, _lowerBarrierAC, _barrierDamageAC, _barrierRegenAC;
    [SerializeField] private Color _damageColor;
    #endregion

    private void Start()
    {
        if (_shouldActivate)
        {
            StartCoroutine(RaiseBarrier(_timeToRaise));
        }
    }

    private void Update()
    {

        float percentage = (_currentLife / (float)_maxLife) * 100;

        // Determine which sprite list to use\
        if (percentage > 0)
        {
            List<Sprite> currentSprites = GetCurrentSpriteList(percentage);

            // Update the sprites on the SpriteRenderers
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                if (i < currentSprites.Count)
                {
                    _spriteRenderers[i].sprite = currentSprites[i];
                }
            }
        }

        if (percentage <= 0 || _currentLife <= 0)
        {
            StartCoroutine(LowerBarrier(_timeToLower));
            foreach (SpriteRenderer sprites in _spriteRenderers)
            {
                sprites.sprite = null;
            }


        }

    }
    private List<Sprite> GetCurrentSpriteList(float percentage)
    {
        if (percentage > 75)
        {
            return _stageOneState;
        }
        else if (percentage > 50)
        {
            return _stageTwoState;
        }
        else if (percentage > 25)
        {
            return _stageThreeState;
        }
        else
        {
            return _stageFourState;
        }

    }

    public IEnumerator RaiseBarrier(float time)
    {
        _isRaisingBarrier = true;
        _shouldActivate = false;
        _currentLife = _maxLife;
        _barrierAnimator.SetTrigger("RaiseBarrier");

        yield return new WaitForSeconds(time);

        _isActive = true;
        _isInactive = false;
        _barrierCollider.enabled = true;

        if (_barrierAudioSource != null)
        {
            _barrierAudioSource.clip = _activateBarrierAC;
            _barrierAudioSource.Play();
        }

        _barrierAnimator.ResetTrigger("RaiseBarrier");

        _isRaisingBarrier = false; // Reset the flag when the coroutine finishes
    }

    public void TakeDamage(int dmg)
    {
        _currentLife -= dmg;
        if (!_isFlashing) // Check if a flash is not already in progress
        {
            StartCoroutine(DamageFlash());
        }
        if (_barrierAudioSource != null)
        {
            _barrierAudioSource.clip = _barrierDamageAC;
            _barrierAudioSource.Play();
        }

        if (_currentLife <= 0)
        {
            StartCoroutine(LowerBarrier(_timeToLower));
        }
    }

    public IEnumerator LowerBarrier(float time)
    {
        _barrierAnimator.SetTrigger("LowerBarrier");
        yield return new WaitForSeconds(time);
        _isActive = false;
        _isInactive = true;
        _barrierCollider.enabled = false;
        if (_barrierAudioSource != null)
        {
            _barrierAudioSource.clip = _lowerBarrierAC;
            _barrierAudioSource.Play();
        }
        _barrierAnimator.ResetTrigger("LowerBarrier");

        if (_canRegen && !_isRaisingBarrier && !_isInactive)
        {
            _regenCoroutine = StartCoroutine(RegenerateBarrier(_regenDelay));
        }
    }

    protected IEnumerator DamageFlash()
    {
        _isFlashing = true; // Set the flag to indicate that damage flash is in progress

        // Store original colors
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in _spriteRenderers)
        {
            originalColors.Add(renderer.color);
            renderer.color = _damageColor;
        }

        yield return new WaitForSeconds(0.1f);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * 5f; // Adjust the speed of color lerp here
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                _spriteRenderers[i].color = Color.Lerp(_damageColor, originalColors[i], elapsedTime);
            }
            yield return null;
        }

        // Ensure we set the original color explicitly
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = originalColors[i];
        }

        _isFlashing = false; // Reset the flag after damage flash is complete    
    }


    public IEnumerator RegenerateBarrier(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset barrier state
        _currentLife = _maxLife;
        _isActive = true;
        _isInactive = false;
        _barrierCollider.enabled = true;
        _barrierAnimator.SetTrigger("RaiseBarrier");

        // Play regen sound if available
        if (_barrierAudioSource != null && _barrierRegenAC != null)
        {
            _barrierAudioSource.clip = _barrierRegenAC;
            _barrierAudioSource.Play();
        }
        yield return new WaitForSeconds(0.1f);
        _barrierAnimator.ResetTrigger("RaiseBarrier");

    }
}

