using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jake_Melee : MeleeBase
{
    [Header("Components")]
    [SerializeField] private Transform _meleeCollidersParent;
    [SerializeField] private Player_MeleeHit[] _meleeHits;

    [Header("Data")]
    [SerializeField] private float _comboTime = 0.6f;
    [SerializeField] private float _comboTimer = 0.0f;

    [Header("VFXs")]
    [SerializeField] private GameObject[] _leftMeleeVFXs;
    [SerializeField] private GameObject[] _rightMeleeVFXs;

    private int _comboState = 0;
    private bool _isMeleeInProgress = false; // should check if in the middle of attack
    private bool _isComboInProgress = false; // should check if player is in combo state
    private Vector3 _meleeCollidersOriginalPos = new(0.0f, 0.0f, 0.0f);
    private Vector3 _meleeColliderOriginalRotation = new(0.0f, 180.0f, 0.0f);
    private Coroutine _handleCombo = null;
    private Coroutine _slashRoutine = null;

    private void Start()
    {
        _comboState = 0;
        _animator.SetBool("IsMeleeInProgress", _isMeleeInProgress);
        _animator.SetBool("IsComboInProgress", _isComboInProgress);
        _animator.SetFloat("ComboState", _comboState);
        _comboTimer = _comboTime;
    }

    private bool IsTimeSlowed()
    {
        return Time.timeScale != 1;
    }
    private void ResetMelee()
    {
        if (_comboState < 2 && _isComboInProgress)
            _comboState++;
        else
            _comboState = 0;

        _animator.SetFloat("ComboState", _comboState);

        _isMeleeInProgress = false;
        _animator.SetBool("IsMeleeInProgress", _isMeleeInProgress);

        _canMelee = true;
    }
    private IEnumerator HandleCombo()
    {
        while (_isComboInProgress)
        {
            _comboTimer -= Time.unscaledDeltaTime; // decrease time ignoring time scale.
            if (_comboTimer <= 0)
            {
                _comboTimer = _comboTime;

                _isComboInProgress = false;
                _animator.SetBool("IsComboInProgress", _isComboInProgress);

                _comboState = 0;
                _animator.SetFloat("ComboState", _comboState);

                _handleCombo = null;
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator SlashWindUp(Rigidbody2D rb2D, bool isArmFacingRight)
    {
        if (_isMeleeInProgress || !_canMelee) yield break; // make sure this coroutine cannot run if player shouldn't attack

        _canMelee = false; // cannot use melee again until true
        _comboTimer = _comboTime;

        if (_comboState < 1) // if first attack of the combo initiate combo from the first slash
        {
            _isComboInProgress = true;
            _animator.SetBool("IsComboInProgress", _isComboInProgress);
            _animator.SetFloat("ComboState", _comboState);
            _handleCombo = StartCoroutine(HandleCombo());
        }
        else if (_isComboInProgress)
        {
            StopCoroutine(_handleCombo);
            _handleCombo = StartCoroutine(HandleCombo());
        }

        rb2D.velocity = Vector2.zero; // make sure player is stopped in place before attacking
        _isMeleeInProgress = true; // check if currently attacking
        _animator.SetBool("IsMeleeInProgress", _isMeleeInProgress); // do actual slash animation
        _slashPitch = Random.Range(_slashPitchRange.x, _slashPitchRange.y);

        for (int i = 0; i < _meleeHits.Length; i++)
        {
            Vector2 colliderOffset = _meleeHits[i].MeleeCollider.offset;

            if (isArmFacingRight)
                colliderOffset.x = Mathf.Abs(colliderOffset.x);
            else
                colliderOffset.x = -Mathf.Abs(colliderOffset.x);

            _meleeHits[i].MeleeCollider.offset = colliderOffset;
        }

        if (IsTimeSlowed())
            yield return new WaitForSecondsRealtime(_windUpTime);
        else
            yield return new WaitForSeconds(_windUpTime);

        if (_comboState > 1)
            _slashRoutine = StartCoroutine(LaunchSlashContact(rb2D, isArmFacingRight)); // start next coroutine [LaunchSlashContact]
        else
            _slashRoutine = StartCoroutine(SlashContact(isArmFacingRight)); // start next coroutine [SlashContact]
        yield break;
    }
    private IEnumerator SlashContact(bool isArmFacingRight)
    {
        if (!_isMeleeInProgress || _canMelee) yield break; // make sure this coroutine cannot run if player shouldn't attack
        yield return null;

        GameObject currentMeleeVFX = isArmFacingRight ? _rightMeleeVFXs[_comboState] : _leftMeleeVFXs[_comboState];
        currentMeleeVFX.SetActive(true);

        _meleeAS.clip = _meleeAC;
        _meleeAS.pitch = _slashPitch;
        _meleeAS.PlayOneShot(_meleeAC);

        Player_MeleeHit currentHitCollider = _meleeHits[_comboState];
        currentHitCollider.MeleeCollider.enabled = true;
        currentHitCollider.MeleeCollider.gameObject.SetActive(true);
        currentHitCollider.transform.SetParent(null);

        if (IsTimeSlowed())
            yield return new WaitForSecondsRealtime(_contactTime);
        else
            yield return new WaitForSeconds(_contactTime);

        _slashRoutine = StartCoroutine(SlashRecovery(currentHitCollider, currentMeleeVFX));
        yield break;
    }
    private IEnumerator LaunchSlashContact(Rigidbody2D rb2D, bool isArmFacingRight)
    {
        if (!_isMeleeInProgress || _canMelee) yield break; // make sure this coroutine cannot run if player shouldn't attack
        yield return null;

        _isLanuching = true;
        GameObject currentMeleeVFX = isArmFacingRight ? _rightMeleeVFXs[_comboState] : _leftMeleeVFXs[_comboState];
        currentMeleeVFX.SetActive(true);

        _meleeAS.clip = _meleeAC;
        _meleeAS.pitch = _slashPitch;
        _meleeAS.PlayOneShot(_meleeAC);

        Player_MeleeHit currentHitCollider = _meleeHits[_comboState];
        currentHitCollider.MeleeCollider.enabled = true;
        currentHitCollider.MeleeCollider.gameObject.SetActive(true);
        currentHitCollider.transform.SetParent(null);

        Vector2 launchDirection = isArmFacingRight ? Vector2.right : Vector2.left;
        float launchDuration = _launchSlashDistance / _launchSlashSpeed;
        float elpasedTime = 0.0f;
        while (elpasedTime < launchDuration)
        {
            rb2D.velocity = launchDirection * _launchSlashSpeed;
            currentHitCollider.transform.position = rb2D.position;
            elpasedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        rb2D.velocity = Vector2.zero;

        if (IsTimeSlowed())
            yield return new WaitForSecondsRealtime(_launchColliderTime);
        else
            yield return new WaitForSeconds(_launchColliderTime);

        _slashRoutine = StartCoroutine(SlashRecovery(currentHitCollider, currentMeleeVFX));
        yield break;
    }
    private IEnumerator SlashRecovery(Player_MeleeHit currentHitCollder, GameObject currentMeleeVFX)
    {
        if (!_isMeleeInProgress || _canMelee) yield break; // make sure this coroutine cannot run if player shouldn't attack
        yield return null;

        currentHitCollder.MeleeCollider.gameObject.SetActive(false);
        currentHitCollder.MeleeCollider.enabled = false;
        currentHitCollder.transform.SetParent(_meleeCollidersParent);
        currentHitCollder.transform.localPosition = _meleeCollidersOriginalPos;
        currentHitCollder.transform.rotation = Quaternion.Euler(_meleeColliderOriginalRotation);
        currentHitCollder.transform.localScale = Vector3.one;
        currentMeleeVFX.SetActive(false);
        yield return null;

        if (_isLanuching)
        {
            if (IsTimeSlowed())
                yield return new WaitForSecondsRealtime(_launchRecoveryTime);
            else
                yield return new WaitForSeconds(_launchRecoveryTime);
            _isLanuching = false;
        }
        else
        {
            if (IsTimeSlowed())
                yield return new WaitForSecondsRealtime(_recoveryTime);
            else
                yield return new WaitForSeconds(_recoveryTime);
        }

        ResetMelee();

        _slashRoutine = null;
        yield break;
    }

    public override void UseMelee(Rigidbody2D rb2D, bool isArmFacingRight)
    {
        _slashRoutine = StartCoroutine(SlashWindUp(rb2D, isArmFacingRight));
    }
}