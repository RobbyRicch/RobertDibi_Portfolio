using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuppetMasterController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Camera _camera;
    [SerializeField] ParticleSystem _interactPS;
    public ParticleSystem _stunPS;
    public bool _stunned;

    [Header("Hit")]
    [SerializeField] LayerMask _playerMask;
    [SerializeField] float _cdHit;
    [SerializeField] public bool _canHit;

    [Header("Interactable Objects")]
    [SerializeField] float _cdTimerInteract = 1.5f;
    Iinteractable interact;
    public bool _canInteract;

    [Header("VFX")]
    [SerializeField] ParticleSystem _punchVFX;

    void Start()
    {
        Cursor.visible = false;
        _canInteract = true;
        _canHit = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && _canHit)
        {
            _stunned = true;
            _canHit = false;
            _animator.Play("Punch");
            /*            CheckHit();
            */
        }
        else if (!_stunned)
        {
            CanInteract();
        }

    }

    private void PuppetMasterMovement()
    {
        Vector3 screenPosition = Input.mousePosition;
        transform.position = new Vector3(1.25f, screenPosition.y, screenPosition.z);
        screenPosition.z = Vector3.Dot(transform.position - _camera.transform.position, _camera.transform.forward);

        Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);

        transform.position = worldPosition;
    }

    IEnumerator InteractTimer()
    {
        float timer = _cdTimerInteract;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _canInteract = false;
    }

    void CanInteract()
    {
        PuppetMasterMovement();

        if (Input.GetMouseButtonDown(0))
        {
            Ray myRay = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit myRaycastHit;

            if (Physics.Raycast(myRay, out myRaycastHit))
            {
                interact = myRaycastHit.collider.transform.GetComponent<Iinteractable>();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (interact != null)
            {
                interact.OnRelease();
            }
            interact = null;
        }

        if (interact != null)
        {
            interact.OnInteract();
            _interactPS.Play();
            StartCoroutine(InteractTimer());
        }
    }

    public void CheckHit()
    {
        bool isPlayer = Physics.CheckSphere(transform.position, 1f, _playerMask);
        if (isPlayer)
        {
            Instantiate(_punchVFX.gameObject, transform.position, transform.rotation);
            GameManager.Instance.OnLoseHeart();

        }
        _stunned = false;
        StopAllCoroutines();
        StartCoroutine(CDHit());
    }

    IEnumerator CDHit()
    {
        yield return new WaitForSeconds(_cdHit);
        _canHit = true;
    }

}
