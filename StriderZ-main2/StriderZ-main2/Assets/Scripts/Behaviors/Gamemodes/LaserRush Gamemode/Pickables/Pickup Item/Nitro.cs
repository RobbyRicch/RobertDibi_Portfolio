using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _nitroTime = 2f;
    [SerializeField] private float addedSpeed = 100;
    private void OnEnable()
    {
        //EventManager.OnNitro += OnPlayerNitro;
    }
    private void OnDisable()
    {
        //EventManager.OnNitro -= OnPlayerNitro;
    }

    public void OnPlayerNitro()
    {
        _playerController.IsStunned = true;
        // need to make PlayerData.Speed to get and set
        //_playerController.PlayerData.Speed += addedSpeed;
        //_playerController.NitroVFX.Play();
        StartCoroutine(DoNitro(_nitroTime));
    }

    private IEnumerator DoNitro(float sec)
    {
        yield return new WaitForSeconds(sec);

        /*if (_playerController.NitroVFX.isPlaying)
            _playerController.NitroVFX.Stop();*/

        _playerController.IsStunned = false;
    }
}
