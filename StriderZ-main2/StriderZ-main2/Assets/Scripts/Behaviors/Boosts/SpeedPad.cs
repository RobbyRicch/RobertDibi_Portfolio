using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    [SerializeField] protected float NitroTime = 1.5f;
    [SerializeField][Range(0f, 1f)] protected float NitroSpeedFactor = 0.3f;
    [SerializeField] private float pushValue;
    [SerializeField] private AudioSource speedPadAudio;
    [SerializeField] private AudioClip speedPadAudioClip;
    protected PlayerInputHandler Player;
    protected PlayerController Controller;

    private float _addedSpeed = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Player = other.GetComponent<PlayerInputHandler>();
            //Controller = other.GetComponent<PlayerController>();
            Use(other.GetComponent<PlayerInputHandler>(), other.GetComponent<PlayerController>());
        }
    }

    private void Use(PlayerInputHandler player, PlayerController controller)
    {
        controller.Rb.AddForce(transform.forward * pushValue,ForceMode.Impulse);
        speedPadAudio.PlayOneShot(speedPadAudioClip);

        _addedSpeed = controller.AdditionalSpeed * NitroSpeedFactor;
        controller.AdditionalSpeed += _addedSpeed;
        
        //player.Data.MainTrail.gameObject.SetActive(false);
        player.Data.NitroTrail.gameObject.SetActive(true);

        StartCoroutine(WaitForSpeed(player,controller));
    }
    IEnumerator WaitForSpeed(PlayerInputHandler player, PlayerController controller)
    {
        yield return new WaitForSeconds(NitroTime);

        controller.AdditionalSpeed -= _addedSpeed;
        //player.Data.MainTrail.gameObject.SetActive(true);
        player.Data.NitroTrail.gameObject.SetActive(false);
    }
}
