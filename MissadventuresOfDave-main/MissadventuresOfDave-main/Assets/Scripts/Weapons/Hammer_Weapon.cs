using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.XR;

public class Hammer_Weapon : Weapon
{
    [Header("Stats")]
    [SerializeField] public int _hammerDmg;
    [SerializeField] private float _swingTime;
    [SerializeField] private float _cooldown;
    [SerializeField] private bool _canSwingAgain;

    [Header("Bools")]
    [SerializeField] private bool _isOnGround;
    

    [Header("Timing")]
    [SerializeField] private float _timeToActivateDamageCollider;

    [Header("Refrences")]
    [SerializeField] private Animator _hammerAnimatorRef;
    [SerializeField] private AudioSource _hammerAudioSource;
    [SerializeField] public BoxCollider2D _damageTrigger;
    [SerializeField] private BoxCollider2D _pickupCollider;
    [SerializeField] private BoxCollider2D _physicsCollider;
    [SerializeField] private Rigidbody2D _hammerRb;
    [SerializeField] public Transform _carryPivot;
    [SerializeField] public HandManager _handRef;

    private Dave_Controller _playerRef;
    private BoxCollider2D _playerCircleCollider;

    private void Start()
    {
        _damageTrigger.isTrigger = false;
        _handRef = FindObjectOfType<HandManager>();

        // Ignore collisions between the katana's damage trigger and the player's circle collider
        Physics2D.IgnoreCollision(_damageTrigger, _playerCircleCollider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dave") && _isOnGround)
        {
            Debug.Log("current hand ref - : " + _handRef);

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
        _hammerRb.freezeRotation = true;
        _hammerRb.bodyType = RigidbodyType2D.Static;
        _physicsCollider.enabled = false;
        _pickupCollider.enabled = false;
        // Parent the hammer to the right hand
        transform.SetParent(handRef.weaponPos);
        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(0, 0, currentPosition.z);
        transform.localRotation = Quaternion.identity;
        _hammerRb.freezeRotation = true;
        _hammerRb.isKinematic = true;
        handRef.rightHandItem = this;
        handRef._holdingRightItem = true;        // Assign the hammer as the equipped right-hand item
    }

    public override void Use()
    {
        StartCoroutine(AnimatorSwingTrigger());

        Debug.Log("YOU WANT ME TO PUT THE HAMMER DOWN?");
    }

    private void SwingHammer()
    {
        StartCoroutine(ActivateSwingCollider(_timeToActivateDamageCollider, _swingTime));

    }

    private IEnumerator ActivateSwingCollider(float time, float timeToReset)
    {
        yield return new WaitForSeconds(time);
        _damageTrigger.isTrigger = true;
        yield return new WaitForSeconds(timeToReset);
        _damageTrigger.isTrigger = false;
    }

    private IEnumerator AnimatorSwingTrigger()
    {
        _hammerAnimatorRef.SetTrigger("Swing");
        yield return new WaitForSeconds(0.5f);
        _hammerAnimatorRef.ResetTrigger("Swing");

    }
}
