using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class NitroPickable : PickableAbilty
{
    [SerializeField] protected float NitroTime = 1.5f;
    [SerializeField] [Range(0f, 1f)] protected float NitroSpeedFactor = 0.3f;

    protected PlayerInputHandler Player;
    protected PlayerController Controller;

    //private float _time = 0, _elapsedTime = 0; 
    private float _addedSpeed = 0;

    /*private void Start()
    {
        _time = Time.time;
    }
    private void Update()
    {
        _elapsedTime = Time.time - _time;

        if (IsActive && _elapsedTime >= NitroTime)
        {
            Controller.AdditionalSpeed -= _addedSpeed;
            Player.Data.MainTrail.gameObject.SetActive(true);
            Player.Data.NitroTrail.gameObject.SetActive(false);
            Destroy(this);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) && !other.GetComponent<PlayerData>().PickupItem)
        {
            Player = other.GetComponent<PlayerInputHandler>();
            Controller = other.GetComponent<PlayerController>();
            NitroPickable nitroPickupOnPlayer = (PickUp(Player, false, false) as NitroPickable);
            InitializeNitroOnPlayer(nitroPickupOnPlayer);
        }
    }
    private void InitializeNitroOnPlayer(NitroPickable nitroPickupOnPlayer)
    {
        nitroPickupOnPlayer.Player = Player;
        nitroPickupOnPlayer.Controller = Controller;
        nitroPickupOnPlayer.NitroSpeedFactor = NitroSpeedFactor;
        nitroPickupOnPlayer.NitroTime = NitroTime;
        nitroPickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        nitroPickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        //nitroPickupOnPlayer.availableVFX = availableVFX;
    }
    public override void Use()
    {
        //Destroy(availableVFX);
        //_addedSpeed = Controller.AdditionalSpeed * NitroSpeedFactor;
        //Controller.AdditionalSpeed += _addedSpeed;
        //IsOnPlayer = true;
        //Player.Data.MainTrail.gameObject.SetActive(false);
        //Player.Data.NitroTrail.gameObject.SetActive(true);
        //IsActive = true;
        StopAllCoroutines();
        StartCoroutine(PlayAbility(NitroTime));
    }
    protected override PickableAbilty AddPickableChild(GameObject playerGO)
    {
        PickupModel.SetActive(false);
        transform.SetParent(playerGO.transform);
        return this;
    }
    protected override PickableAbilty AddPickable(GameObject playerGO)
    {
        PickableAbilty pickable = playerGO.AddComponent<NitroPickable>();
        return pickable;
    }

    private IEnumerator PlayAbility(float duration)
    {
        yield return new WaitForSeconds(0.001f);

        //Destroy(availableVFX);
        _addedSpeed = Controller.AdditionalSpeed * NitroSpeedFactor;
        Controller.AdditionalSpeed += _addedSpeed;
        IsOnPlayer = true;
        //Player.Data.MainTrail.gameObject.SetActive(false);
        Player.Data.NitroTrail.gameObject.SetActive(true);
        IsActive = true;

        yield return new WaitForSeconds(duration);

        Controller.AdditionalSpeed -= _addedSpeed;
        //Player.Data.MainTrail.gameObject.SetActive(true);
        Player.Data.NitroTrail.gameObject.SetActive(false);
        IsOnPlayer = false;
        IsActive = false;
        Destroy(this);
    }
}
