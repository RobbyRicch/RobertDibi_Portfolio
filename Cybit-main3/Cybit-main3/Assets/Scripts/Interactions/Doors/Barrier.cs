using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private AudioSource _audioSource;

    [Header("Data")]
    [SerializeField] private float _timeToOpen = 1.0f;
    public float TimeToOpen => _timeToOpen;

    private void OnEnable()
    {
        EventManager.OnBarrierDown += OnBarrierDown;
    }
    private void OnDisable()
    {
        EventManager.OnBarrierDown -= OnBarrierDown;
    }

    private IEnumerator OpenDoor()
    {
        _animator.SetBool("IsOpening", true);
        _audioSource.Play();
        yield return new WaitForSeconds(_timeToOpen);
        _collider.enabled = false;
        _collider.gameObject.SetActive(false);
    }

    private void OnBarrierDown(Barrier barrier)
    {
        if (barrier != this)
            return;

        StartCoroutine(OpenDoor());
    }
}
