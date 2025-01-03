using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tz_MovePoint : MonoBehaviour
{
    [Header("Goal : Use this as a move to objective")]
    [Header("Refrences")]
    [SerializeField] private Tz_PhaseManager _checkpointRefs;
    [SerializeField] private int _whichCheckpoint;
    [SerializeField] private Color _triggeredColor;
    [SerializeField] private SpriteRenderer _currentRenderer;
    [SerializeField] private bool _hasBeenTriggered;
    [SerializeField] private bool _isFinalTrigger;
    [SerializeField] private GameObject _finalPoint;
    [SerializeField] private AudioSource _pointAudioSource;
    [SerializeField] private AudioClip _completeAC;
    [SerializeField] private Animator _animatorRef;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_hasBeenTriggered)
        {
            _hasBeenTriggered = true;
            _currentRenderer.color = _triggeredColor;
            _checkpointRefs._checkPoints[_whichCheckpoint] = true;
            StartCoroutine(TriggerReset(0.15f));
            _pointAudioSource.pitch = 1 + 1;
            _pointAudioSource.PlayOneShot(_completeAC);

            if (_isFinalTrigger) return;

            // Check if all checkpoints are true
            if (_checkpointRefs._checkPoints.All(checkpoint => checkpoint))
            {
                _finalPoint.SetActive(true);
                Debug.Log("All checkpoints triggered!");
            }

            
        }

        if (collision.gameObject.CompareTag("Player") && _isFinalTrigger)
        {
            _checkpointRefs._movementFinished = true;
            
        }
    }

    private IEnumerator TriggerReset(float time)
    {
        _animatorRef.SetTrigger("Pulse");
        yield return new WaitForSeconds(time);
        _animatorRef.ResetTrigger("Pulse");

    }
}
