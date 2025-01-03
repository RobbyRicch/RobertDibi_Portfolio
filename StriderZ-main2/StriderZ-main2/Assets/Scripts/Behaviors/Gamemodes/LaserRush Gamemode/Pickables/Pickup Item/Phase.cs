using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _phaseTime = 2f;
    private Rigidbody rb;
    private int _useCount = 3;
    private void OnEnable()
    {
        //EventManager.OnPhase += OnPlayerPhase;
    }
    private void OnDisable()
    {
        //EventManager.OnPhase -= OnPlayerPhase;
    }

    public void OnPlayerPhase()
    {
        if (_useCount == 0)
            return;

        _useCount--;
        rb.isKinematic = true;
        _playerController.IsStunned = true;
        //_playerController.PhaseVFX.Play();
        StartCoroutine(DoPhase(_phaseTime));
    }

    private IEnumerator DoPhase(float sec)
    {
        yield return new WaitForSeconds(sec);

        rb.isKinematic = false;
        /*if (_playerController.PhaseVFX.isPlaying)
            _playerController.PhaseVFX.Stop();*/

        _playerController.IsStunned = false;
    }
    private void OnValidate()
    {
        rb = _playerController.Rb;
    }
}
