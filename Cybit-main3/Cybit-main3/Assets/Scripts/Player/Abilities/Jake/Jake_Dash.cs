using Cinemachine;
using System.Collections;
using UnityEngine;

public class Jake_Dash : DashBase
{
    [Header("Components")]
    [SerializeField] private GameObject _dashVFX;
    [SerializeField] private Rigidbody2D _playerRb;
    [SerializeField] private CinemachineVirtualCamera _playerVirtualCam;
    [SerializeField] private Player_Controller _playerController;
    [SerializeField] private Player_Data _playerData;
    [SerializeField] private AudioSource _dashAudioSource;
    [SerializeField] private AudioClip _dashAC;

    [Header("Data")]
    [SerializeField] private float _dashSpeed = 10f;
    public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }

    [SerializeField] private float _dashStaminaCost;
    public float DashStaminaCost { get => _dashStaminaCost; set => _dashStaminaCost = value; }

    [SerializeField] private float _fadeDuration = 1.0f;
    public float FadeDuration { get => _fadeDuration; set => _fadeDuration = value; }

    [SerializeField] private bool _isDashing;

    public bool IsDashing => _isDashing;


    [Header("Fluff")]
    [SerializeField] private float _bloomIntensity = 2.0f;
    [SerializeField] private float _bloomDuration = 0.4f;
    [SerializeField] private AnimationCurve _bloomCurve;

    private IEnumerator FadeOutSprite(GameObject sprite)
    {
        SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            //Debug.LogError("SpriteRenderer component not found on GameObject: " + sprite.name);
            yield break;
        }

        Color startColor = renderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < _fadeDuration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / _fadeDuration);
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        renderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(sprite);
        //_isDashing = false;
    }

    private void Dash()
    {
        if (_playerController.CurrentStamina < _dashStaminaCost || !_playerController.Melee.CanMelee || _playerController.IsInputDisabled)
            return;
        _isDashing = true;
        _dashAudioSource.pitch = Random.Range(1f, 4.5f);
        _dashAudioSource.PlayOneShot(_dashAC);
        _playerVirtualCam.Follow = null;
        GameObject dashDummyVfx = Instantiate(_dashVFX, gameObject.transform.position, Quaternion.identity);

        _playerController.Animations.DoBloom(_bloomIntensity, _bloomDuration, _bloomCurve);
        _playerController.UseStamina(_dashStaminaCost);

        Vector2 addedVelocity = new Vector2(_playerController.CurrentMoveVector.x * _dashSpeed, _playerController.CurrentMoveVector.y * _dashSpeed);
        _playerRb.AddForce(addedVelocity, ForceMode2D.Force);
        StartCoroutine(FadeOutSprite(dashDummyVfx));
    }

    public override void UseDash()
    {
        Dash();

        if (!_playerController.IsInCutscene)
        {
            _playerController.StartResetCameraTarget();

        }
    }
}
