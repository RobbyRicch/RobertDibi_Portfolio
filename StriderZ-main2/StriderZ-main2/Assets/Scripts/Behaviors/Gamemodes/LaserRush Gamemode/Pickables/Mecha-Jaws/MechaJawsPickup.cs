using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaJawsPickup : PickableAbilty
{
    [SerializeField] protected GameObject MechaJawsPrefab;
    [SerializeField] private float _forwardOffset = 3.0f;

    bool _playerPicked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (_disablePickup)
            return;

        if (other.CompareTag(PlayerTag) && !_isPickedUp)
        {
            Player = other.GetComponent<PlayerInputHandler>();
            if (Player.Data.PickupItem == null)
            {
                _isPickedUp = true;

                if (Player.Attractor.IsLeftGrabbingPickup(transform))
                    Player.Attractor.CancelAttractorLeft(true);
                else if (Player.Attractor.IsRightGrabbingPickup(transform))
                    Player.Attractor.CancelAttractorRight(true);

                CheckGrabbingPickable();
                if (PickUpAvailableVFX != null)
                {
                    /*Transform currentModel = playerData.gameObject.transform.GetChild(playerData.ModelNum + 1).transform;
                    Quaternion modelRotation = new Quaternion(0, currentModel.rotation.y, 0, 0);*/
                    availableVFX = Instantiate(PickUpAvailableVFX, Player.gameObject.transform.position, Player.gameObject.transform.rotation, Player.gameObject.transform);
                    Player.Controller.BodyTilter.IgnoreTransforms.Add(availableVFX.transform);
                }
                MechaJawsPickup MechaJawsPickupOnPlayer = PickUp(Player, false, false) as MechaJawsPickup;
                InitializeMechaJawsOnPlayer(MechaJawsPickupOnPlayer);
            }
            else
            {
                Player.Data.PickupItem.SpawnPickupObtainedAgain(Player.Data);
            }
        }
    }
    private void Update()
    {
        if (!IsUsed)
            return;
        if (Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].CheckAnimationEnded(Pickups.MeshaJaws))
        {
            Player.Controller.IsUsingPickupRight = false;
            Player.Controller.IsUsingPickupLeft = false;
            Destroy(this.gameObject);
        }
    }
    private void InitializeMechaJawsOnPlayer(MechaJawsPickup mechaJawsPickupOnPlayer)
    {
        mechaJawsPickupOnPlayer.MechaJawsPrefab = MechaJawsPrefab;
        mechaJawsPickupOnPlayer.Player = Player;
        mechaJawsPickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        mechaJawsPickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        mechaJawsPickupOnPlayer.availableVFX = availableVFX;
        pickupRandomizer = null;
    }
    public override void Use()
    {
        if (IsUsed)
            return;

        if (!Player.Attractor.IsUsingRightAttractor && Player.Controller.IsUsingPickupRight)
        {
            //Player.Controller.IsUsingPickupRight = true;
            Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
            IsUsed = true;
            Destroy(availableVFX);
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayJawsAnim(false);
            Invoke("StartJaws", 0.7f);
        }
        else if (!Player.Attractor.IsUsingLeftAttractor && Player.Controller.IsUsingPickupLeft)
        {
            //Player.Controller.IsUsingPickupLeft = true;
            Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
            IsUsed = true;
            Destroy(availableVFX);
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayJawsAnim(true);
            Invoke("StartJaws", 0.7f);
        }
    }
    private void StartJaws()
    {
        Vector3 offset = Player.Data.ProjectileTr.transform.position + Player.Controller.CrosshairParent.forward * _forwardOffset / 2;
        GameObject mechaJawsGO = Instantiate(MechaJawsPrefab, offset, Player.Controller.CrosshairParent.transform.rotation);
        MechaJaws mechaJaws = mechaJawsGO.GetComponent<MechaJaws>();

        mechaJaws.ThrowingPlayer = Player;
    }
    protected override PickableAbilty AddPickableChild(GameObject playerGO)
    {
        PickupModel.SetActive(false);
        PickupCollider.enabled = false;
        transform.SetParent(playerGO.transform);
        return this;
    }
    protected override PickableAbilty AddPickable(GameObject playerGO)
    {
        MechaJawsPickup pickable = playerGO.AddComponent<MechaJawsPickup>();
        return pickable;
    }
    /*private void OnDestroy()
    {
        if (IsOnPlayer)
            return;

        if (Player.Attractor.IsLeftGrabbingPickup(transform))
            Player.Attractor.CancelAttractorLeft(true);
        else if (Player.Attractor.IsRightGrabbingPickup(transform))
            Player.Attractor.CancelAttractorRight(true);
    }*/
}
