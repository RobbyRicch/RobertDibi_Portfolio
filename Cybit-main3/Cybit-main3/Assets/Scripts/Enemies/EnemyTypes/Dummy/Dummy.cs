using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private Color _damageColor;

    [SerializeField] private bool _isFacingRight = true;
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }

    [SerializeField] private bool _isFlashing = false;
    public bool IsFlashing { get => _isFlashing; set => _isFlashing = value; }

    [SerializeField] private List<SpriteRenderer> _spriteRenderers;
    public List<SpriteRenderer> SpriteRenderers => _spriteRenderers;

    [SerializeField] private Transform _playerTr;
    public Transform PlayerTr { get => _playerTr; set => _playerTr = value; }

    private void Update()
    {
        AimAtTarget();
    }

    private void FlipEnemy()
    {
        // Toggle the facing flags
        _isFacingRight = !_isFacingRight;

        // Flip the character's scale
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }
    private void AimAtTarget()
    {
        if (!_playerTr)
        {
            Debug.Log("Missing Player transform");
            return;
        }

        Vector3 directionToPlayer = _playerTr.transform.position - transform.position;
        if (directionToPlayer.x < 0 && _isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", false);
            //Debug.Log("flipped left");

        }
        else if (directionToPlayer.x > 0 && !_isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", true);
            //Debug.Log("flipped right");
        }
    }

    private IEnumerator DamageFlash()
    {
        _isFlashing = true;

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
            elapsedTime += Time.deltaTime * 5f;
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                _spriteRenderers[i].color = Color.Lerp(_damageColor, originalColors[i], elapsedTime);
            }
            yield return null;
        }

        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = originalColors[i];
        }

        _isFlashing = false;
    }
    public void TakeDamage()
    {
        if (!_isFlashing)
        {
            StartCoroutine(DamageFlash());
            _enemyAnimator.SetTrigger("TakeDamage");
        }

        EventManager.InvokeHitDummy();
    }
}
