using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Katana_Weapon : Weapon
{
    [Header("Stats")]
    [SerializeField] private float _swingTime;
    [SerializeField] private float _cooldown;
    [SerializeField] private bool _canSwingAgain;

    [Header("Bools")]
    [SerializeField] private bool _isOnGround;


    [Header("Timing")]
    [SerializeField] private float _timeToActivateDamageCollider;

    [Header("Refrences")]
    [SerializeField] private Animator _handsAnimatorRef;
    [SerializeField] private AudioSource _katanaAudioSource;
    [SerializeField] public BoxCollider2D _damageTrigger;
    [SerializeField] private BoxCollider2D _pickupCollider;
    [SerializeField] private BoxCollider2D _physicsCollider;
    [SerializeField] private Rigidbody2D _katanaRb;
    [SerializeField] public Transform _carryPivot;
    [SerializeField] public HandManager _handRef;

    private Dave_Controller _playerRef;
    private BoxCollider2D _playerCircleCollider;


    private void Start()
    {
        _damageTrigger.enabled = false;
        _handRef = FindObjectOfType<HandManager>();

        // Find the player’s CircleCollider2D (assuming it's on a GameObject named "Player")
        _playerRef = FindObjectOfType<Dave_Controller>();
        _playerCircleCollider = _playerRef._physicsCollider;

        // Ignore collisions between the katana's damage trigger and the player's circle collider
        Physics2D.IgnoreCollision(_damageTrigger, _playerCircleCollider);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dave") && _isOnGround)
        {


            if (!_handRef._holdingRightItem)
            {
                Debug.Log("Dave is touching Katana");

                // Adjust scale based on hand flip state
                if (_handRef._rightHandIsFlipped)
                {
                    gameObject.transform.localScale = new Vector3(-Mathf.Abs(gameObject.transform.localScale.x), gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x), gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                }

                HandlePickup(_handRef);
                _isOnGround = false;
                Debug.Log($"Current rightHandItem: {_handRef.rightHandItem}");
            }
            else
            {
                Debug.Log("Dave already has a weapon. Do you want to switch?");
            }
        }
    }

    private void HandlePickup(HandManager handRef)
    {
        _katanaRb.freezeRotation = true;
        _katanaRb.bodyType = RigidbodyType2D.Static;
        _physicsCollider.enabled = false;
        _pickupCollider.enabled = false;
        // Parent the hammer to the right hand
        transform.SetParent(handRef.weaponPos);
        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(0, 0, currentPosition.z);
        transform.localRotation = Quaternion.identity;
        _katanaRb.freezeRotation = true;
        _katanaRb.isKinematic = true;
        handRef.rightHandItem = this;
        handRef._holdingRightItem = true;
        // Assign the hammer as the equipped right-hand item
    }

    public override void Use()
    {
        StartCoroutine(AnimatorSwingTrigger());
        StartCoroutine(ActivateSwingCollider(_timeToActivateDamageCollider, _swingTime));

        Debug.Log("ChingChing");
    }

    private IEnumerator ActivateSwingCollider(float time, float timeToReset)
    {
        yield return new WaitForSeconds(time);
        _damageTrigger.enabled = true;
        yield return new WaitForSeconds(timeToReset);
        _damageTrigger.enabled = false;
    }

    private IEnumerator AnimatorSwingTrigger()
    {
        _handsAnimatorRef.SetTrigger("Pierce");
        _handRef._followCrosshair = false;
        yield return new WaitForSeconds(0.5f);
        _handRef._followCrosshair = true;
        _handsAnimatorRef.ResetTrigger("Pierce");

    }
}
