using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Player_Deflect : MonoBehaviour
{
    [Header("HUD RELATED - Deflect")]
    [SerializeField] public Animator _deflectHudAnimator;
    [SerializeField] private GameObject _deflectFill;

    [Header("SOUND RELATED - Deflect")]
    [SerializeField] private AudioSource _deflectHudAudioSource;
    [SerializeField] private AudioClip _deflectPingAC, _deflectActivateAC;

    [Header("Components")]
    [SerializeField] private Transform _meleeCollidersParent;
    [SerializeField] private Player_DeflectHit _deflectHit;
    private Coroutine _cooldownRoutine;

    [Header("Data")]
    [SerializeField] private float _deflectModeTime = 1.5f;
    [SerializeField] private float _deflectCooldown = 1.5f;
    public float DeflectCooldown { get => _deflectCooldown; set => _deflectCooldown = value; }
    private float _deflectCurrentCooldown = 0.0f;

    private bool _canDeflect = true;
    public bool CanDeflect { get => _canDeflect; set => _canDeflect = value; }

    private void InitiateDeflect(Player_Controller playerController, Player_Animations animations)
    {
        playerController.IsMovementOnlyDisabled = true;
        animations.Animator.SetBool("IsDeflecting", true);
        _deflectHudAnimator.ResetTrigger("Pulse");
        _deflectFill.SetActive(false);

        _deflectHit.Collider.enabled = true;
        _deflectHit.Collider.gameObject.SetActive(true);
    }
    private void CeasePerfectBlock(Player_Controller playerController, Player_Animations animations)
    {
        _deflectHit.Collider.enabled = false;
        _deflectHit.Collider.gameObject.SetActive(false);
        _deflectCurrentCooldown = _deflectCooldown;

        animations.Animator.SetBool("IsDeflecting", false);
        playerController.IsMovementOnlyDisabled = false;
        _cooldownRoutine = StartCoroutine(HandleCooldown());
    }
    private IEnumerator HandleCooldown()
    {
        while (!_canDeflect)
        {
            _deflectCurrentCooldown -= Time.deltaTime;

            if (_deflectCurrentCooldown <= 0)
            {
                _deflectCurrentCooldown = _deflectCooldown;
                _deflectFill.SetActive(true);
                _deflectHudAnimator.SetTrigger("Pulse");
                _canDeflect = true;
            }
            yield return null;
        }
    }
    private IEnumerator DeflectRoutine(Player_Controller playerController, Player_Animations animations)
    {
        if (!_canDeflect)
            yield break;

        _canDeflect = false;
        InitiateDeflect(playerController, animations);
        yield return new WaitForSecondsRealtime(_deflectModeTime);

        CeasePerfectBlock(playerController, animations);
    }

    public void FlipCollider(bool isArmFacingRight)
    {
        if (isArmFacingRight)
        {
            Vector3 newDeflectColliderScale = _deflectHit.Collider.transform.localScale;
            newDeflectColliderScale.x = -1.0f;
            _deflectHit.Collider.transform.localScale = newDeflectColliderScale;
        }
        else
        {
            Vector3 newDeflectColliderScale = _deflectHit.Collider.transform.localScale;
            newDeflectColliderScale.x = 1.0f;
            _deflectHit.Collider.transform.localScale = newDeflectColliderScale;
        }
    }
    public void Deflect(Player_Controller playerController, Player_Animations animations)
    {
        StartCoroutine(DeflectRoutine(playerController, animations));
    }
}
