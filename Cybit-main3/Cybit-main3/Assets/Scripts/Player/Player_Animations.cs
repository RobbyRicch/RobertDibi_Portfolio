using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// MVC View
public class Player_Animations : MonoBehaviour
{
    [SerializeField] private Volume _fluffVolume;
    private Bloom _fluffBloomLayer;
    private float _originalFluffBloomIntensity;

    [SerializeField] private SpriteRenderer[] _playerSprites;
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private float _fadeDuration = 0.5f;

    private bool _isFlashing = false;
    public bool IsFlashing { get => _isFlashing; set => _isFlashing = value; }

    [Header("Bools")]
    public bool ShouldApplyAnimations;

    [Header("Refrences")]
    [SerializeField] private Animator _anim = null;
    public Animator @Animator => _anim;

    [SerializeField] private SpriteRenderer _armSprite = null;

    private Vector2 _playerVector;
    private void Start()
    {
        _fluffVolume.profile.TryGet(out _fluffBloomLayer);

        if (_fluffBloomLayer != null)
            _originalFluffBloomIntensity = _fluffBloomLayer.intensity.value;
    }

    private IEnumerator BloomEffectCoroutine(float bloomIntensity, float bloomDuration, AnimationCurve curve)
    {
        float elapsedTime = 0f;

        while (elapsedTime < bloomDuration)
        {
            float curveValue = curve.Evaluate(elapsedTime / bloomDuration);
            _fluffBloomLayer.intensity.value = Mathf.Lerp(_originalFluffBloomIntensity, bloomIntensity, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _fluffBloomLayer.intensity.value = _originalFluffBloomIntensity;
    }
    public void DoBloom(float bloomIntensity, float bloomDuration, AnimationCurve curve)
    {
        StartCoroutine(BloomEffectCoroutine(bloomIntensity, bloomDuration, curve));
    }

    public void DoDamageFlash(Color damageColor)
    {
        StartCoroutine(DamageFlash(damageColor));
    }
    private IEnumerator DamageFlash(Color damageColor)
    {
        if (!_isFlashing && _playerSprites != null && _playerSprites.Length > 0) yield break;

        _isFlashing = true; // Set the flag to indicate that damage flash is in progress

        // Store original colors
        List<Color> originalColors = new List<Color>();

        foreach (SpriteRenderer renderer in _playerSprites)
        {
            originalColors.Add(renderer.color);
            renderer.color = damageColor;
        }

        yield return new WaitForSeconds(_flashDuration);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.unscaledDeltaTime * 5f; // Adjust the speed of color lerp here
            for (int i = 0; i < _playerSprites.Length; i++)
            {
                _playerSprites[i].color = Color.Lerp(damageColor, originalColors[i], elapsedTime);
            }
            yield return null;
        }

        // Ensure we set the original color explicitly
        for (int i = 0; i < _playerSprites.Length; i++)
        {
            _playerSprites[i].color = originalColors[i];
        }

        _isFlashing = false; // Reset the flag after damage flash is complete
    }

    public IEnumerator FadeOutPlayer()
    {
        float maxFadeDuration = 0f;
        foreach (SpriteRenderer sprite in _playerSprites)
        {
            maxFadeDuration = Mathf.Max(maxFadeDuration, _fadeDuration);
        }

        float elapsedTime = 0f;

        while (elapsedTime < maxFadeDuration)
        {
            // Calculate the alpha based on the maximum elapsed time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / maxFadeDuration);

            // Apply this alpha to all sprites
            foreach (SpriteRenderer sprite in _playerSprites)
            {
                Color startColor = sprite.color;
                sprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all sprites are fully transparent at the end
        foreach (SpriteRenderer sprite in _playerSprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
        }
    }
    public IEnumerator FadeInPlayer()
    {
        float maxFadeDuration = 0f;
        foreach (SpriteRenderer sprite in _playerSprites)
        {
            maxFadeDuration = Mathf.Max(maxFadeDuration, _fadeDuration);
        }

        float elapsedTime = 0f;

        while (elapsedTime < maxFadeDuration)
        {
            // Calculate the alpha based on the maximum elapsed time
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / maxFadeDuration);

            // Apply this alpha to all sprites
            foreach (SpriteRenderer sprite in _playerSprites)
            {
                Color startColor = sprite.color;
                sprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all sprites are fully opaque at the end
        foreach (SpriteRenderer sprite in _playerSprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
    }

    public void AnimateMovement(Vector2 moveVector, Vector2 lastMoveDirection, Collider2D playerCollider)
    {
        if (!ShouldApplyAnimations)
            return;

        float moveX = moveVector.x;
        float moveY = moveVector.y;

        if (moveVector.magnitude == 0 && lastMoveDirection.magnitude != 0)
        {
            moveX = lastMoveDirection.x;
            moveY = lastMoveDirection.y;
        }

        _anim.SetFloat("AnimMoveX", moveX);

        if (moveX <= -0.1f)
        {
            _armSprite.sortingOrder = 0;
            Vector2 newOffset = playerCollider.offset;
            newOffset.x = -0.05f;
            playerCollider.offset = newOffset;
        }
        else if (moveX >= 0.1f)
        {
            _armSprite.sortingOrder = 2;
            Vector2 newOffset = playerCollider.offset;
            newOffset.x = 0.05f;
            playerCollider.offset = newOffset;
        }

        _anim.SetFloat("AnimMoveY", moveY);
        _anim.SetFloat("AnimMoveMagnitude", moveVector.magnitude);
        _anim.SetFloat("AnimLastMoveX", lastMoveDirection.x);
        _anim.SetFloat("AnimLastMoveY", lastMoveDirection.y);
    }
    public void DeathAnimation()
    {
        _anim.SetTrigger("Die");
        StartCoroutine(DeathTriggerReset());
        _anim.SetBool("Dead", true);
        _anim.ResetTrigger("Restore");
    }

    private IEnumerator DeathTriggerReset()
    {
        yield return new WaitForSeconds(0.1f);
        _anim.ResetTrigger("Die");
    }

    private IEnumerator RestoreAnimation()
    {
        yield return new WaitForSeconds(3);
        _anim.SetBool("Dead", false);
        _anim.SetTrigger("Restore");

    }

    public void DesyncAnimation()
    {
        _anim.SetBool("Dead", false);
        _anim.SetTrigger("DeSync");
        StartCoroutine(RestoreAnimation());
    }
    public void StopAnimations(bool stop)
    {
        /*        _anim.SetFloat("Blend", 0.0f);
                _anim.SetFloat("AnimMoveX", 0.0f);
                _anim.SetFloat("AnimMoveY", 0.0f);
                _anim.SetFloat("AnimMoveMagnitude", 0.0f);
                _anim.SetFloat("AnimLastMoveX", 0.0f);
                _anim.SetFloat("AnimLastMoveY", 0.0f);*/
        if (stop)
        {
        _anim.SetBool("Idle",true);

        }
        else
        {
            _anim.SetBool("Idle", false);
        }
    }
}
