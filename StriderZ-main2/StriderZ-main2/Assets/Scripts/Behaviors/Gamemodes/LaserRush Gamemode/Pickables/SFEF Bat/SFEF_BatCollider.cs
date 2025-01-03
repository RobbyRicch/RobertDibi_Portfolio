using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFEF_BatCollider : MonoBehaviour
{
    private PlayerInputHandler _usingPlayer;
    [SerializeField] private GameObject HitParticle;
    //[SerializeField] private Collider BatColliderRef;
    private void OnTriggerEnter(Collider other)
    {
        _usingPlayer = GetComponentInParent<PlayerInputHandler>();
        if (other.CompareTag("Player") && other.GetComponent<PlayerInputHandler>() != _usingPlayer)
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            GameObject hitPar = Instantiate(HitParticle, transform.position, transform.rotation,null);
            SoundManager.Instance.PlayPickUpSound(SoundManager.Instance.BatHitSound);
            //player.Controller.Rb.AddForce(_usingPlayer.Controller.ModelHandler.AllModels[_usingPlayer.Data.ModelNum+1].transform.forward * 200,ForceMode.Impulse);
            player.Controller.Rb.AddForce(_usingPlayer.transform.GetChild((int)_usingPlayer.SetupData.ChosenModelType).forward * 200,ForceMode.Impulse);
            Destroy(hitPar,1f);
        }
    }
}
