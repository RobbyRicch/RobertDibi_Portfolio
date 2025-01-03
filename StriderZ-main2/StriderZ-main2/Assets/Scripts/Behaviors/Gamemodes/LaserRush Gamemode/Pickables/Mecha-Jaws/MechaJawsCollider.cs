using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaJawsCollider : MonoBehaviour
{
    [SerializeField] private float StunTime = 2;
    [SerializeField] private MechaJaws _mechaJawsRef;
    [SerializeField] private GameObject _mechJawsModel;
    public bool triggered = false;

    private float _timer = 0;
    private bool _paralyzed = false;
    private List<GameObject> _allParalyzedPlayers = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;
            StunEffect(other.gameObject);
            //Debug.Log("got bit");
            //_mechaJawsRef.Explode();
            //GetComponentInChildren<Renderer>().enabled = false;
            _mechJawsModel.SetActive(false);
        }
    }
    private void Update()
    {
        if (_paralyzed)
        {
            if (_timer < StunTime)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _timer = 0;
                foreach (var player in _allParalyzedPlayers)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    AttractorController grappleController = player.GetComponent<AttractorController>();
                    DoStun(playerController, grappleController);
                }
                Destroy(_mechaJawsRef.gameObject);
            }
        }
    }
    void StunEffect(GameObject player)
    {
        SoundManager.Instance.PlayStunSound(SoundManager.Instance.StunHitSounds);
        PlayerController playerController = player.GetComponent<PlayerController>();
        AttractorController grappleController = player.GetComponent<AttractorController>();
        _paralyzed = true;
        _allParalyzedPlayers.Add(player);
        playerController.IsStunned = true;
        grappleController.enabled = false;
        playerController.Rb.velocity = Vector3.zero;
        playerController.Rb.velocity = Vector3.ClampMagnitude(playerController.Rb.velocity, 0);
        Vector3 pos = playerController.transform.position;
        playerController.InputHandler.Data.SharkBiteVFX.Play();
        playerController.transform.position = pos;
        playerController.Rb.velocity = Vector3.ClampMagnitude(playerController.Rb.velocity, 0);
        playerController.Rb.velocity = Vector3.zero;

    }
    private void DoStun(PlayerController playerController, AttractorController grappleController)
    {
        playerController.IsStunned = false;
        grappleController.enabled = true;
        _paralyzed = false;
        //Destroy(_mechaJawsRef.gameObject);
    }
}
