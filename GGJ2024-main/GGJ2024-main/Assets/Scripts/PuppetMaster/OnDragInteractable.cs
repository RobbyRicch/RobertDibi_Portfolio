using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class OnDragInteractable : MonoBehaviour, Iinteractable
{
    [SerializeField] float _distance = 2f;
    [SerializeField] float _cdTimer = 2f;
    [SerializeField] PuppetMasterController _puppetMaster;
    [SerializeField] DragInteractableState.DragAxis _axis;

    Transform _master;
    Vector3 _pos;
    Vector3 _dragAxis;
    bool _isDragging;
    [SerializeField] bool _interacted;
    private void Awake()
    {
        _puppetMaster = GameObject.FindObjectOfType<PuppetMasterController>();
    }
    private void Start()
    {
        _pos = transform.position;
        _master = _puppetMaster.transform;
        this._interacted = false;
    }

    public void OnInteract()
    {
        _isDragging = true;
    }

    public void OnRelease()
    {
        _isDragging = false;
        this._interacted = true;
        InteractCD();
    }

    public void InteractCD()
    {
        StartCoroutine(InteractCDCor());
    }

    private void Update()
    {
        DragMovement();
    }

    private void DragMovement()
    {
        if (_isDragging && _puppetMaster._canInteract && !this._interacted)
        {
            switch (_axis)
            {
                case DragInteractableState.DragAxis.Horizontal:

                    Vector3 distA = new Vector3(_pos.x, _pos.y, _pos.z + _distance);
                    Vector3 distB = new Vector3(_pos.x, _pos.y, _pos.z - _distance);

                    if (distA.z > _master.position.z && distB.z < _master.position.z)
                    {
                        _dragAxis = new Vector3(_pos.x, _pos.y, _master.position.z);
                    }
                    break;
                case DragInteractableState.DragAxis.Vertical:

                    Vector3 _distA = new Vector3(_pos.x, _pos.y + _distance, _pos.z);
                    Vector3 _distB = new Vector3(_pos.x, _pos.y - _distance, _pos.z);

                    if (_distA.y > _master.position.y && _distB.y < _master.position.y)
                    {
                        _dragAxis = new Vector3(_pos.x, _master.position.y, _pos.z);
                    }
                    break;
            }

            transform.position = _dragAxis;
        }
    }

    IEnumerator InteractCDCor()
    {
        float timer = _cdTimer;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _puppetMaster._canInteract = true;
    }
}
