using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeBase : MonoBehaviour
{
    [Header("Base Components")]
    [SerializeField] protected Animator _animator;

    [Header("Base Data")]
    [SerializeField] protected float _windUpTime = 0.1f;
    [SerializeField] protected float _contactTime = 0.2f;
    [SerializeField] protected float _recoveryTime = 0.1f;
    [SerializeField] protected float _launchSlashDistance = 30.0f;
    [SerializeField] protected float _launchSlashSpeed = 10.0f;
    [SerializeField] protected float _launchColliderTime = 0.4f;
    [SerializeField] protected float _launchRecoveryTime = 0.3f;
    protected bool _isLanuching = false;

    [Header("Base Sound")]
    [SerializeField] protected AudioSource _meleeAS;
    [SerializeField] protected AudioClip _meleeAC;
    protected Vector2 _slashPitchRange = new(0.75f, 1.25f);
    protected float _slashPitch = 1.0f;

    protected bool _canMelee = true;
    public bool CanMelee { get => _canMelee; set => _canMelee = value; }

    public abstract void UseMelee(Rigidbody2D rb2D, bool isArmFacingRight);
}
