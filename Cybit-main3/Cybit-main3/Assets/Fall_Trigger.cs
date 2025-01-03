using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Trigger : MonoBehaviour
{
    [SerializeField] private Player_Controller _playerObject;
    public Player_Controller PlayerObject { get => _playerObject; set => _playerObject = value; }

    [SerializeField] private GameObject _restorePoint;
    [SerializeField] private bool _shouldfall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _shouldfall)
        {
            _playerObject.FadeOut();
            _playerObject.IsInputDisabled = true;
            Debug.Log("player fell");
            StartCoroutine(RestorePlayerFromFall());
        }
    }

    public IEnumerator RestorePlayerFromFall()
    {
        yield return new WaitForSeconds(1f);
        _playerObject.transform.position = _restorePoint.transform.position;
        yield return new WaitForSeconds(1f);
        _playerObject.FadeIn();
        _playerObject.IsInputDisabled = false;
    }
}
