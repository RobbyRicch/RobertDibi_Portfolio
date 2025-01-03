using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveBallPickup : PickableAbilty
{
    [SerializeField] protected GameObject ShoveBallPrefab;
    [SerializeField] private float _forwardOffset = 3.0f;
    [SerializeField] private Material _shoveBallMat;

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
                    availableVFX = Instantiate(PickUpAvailableVFX, Player.gameObject.transform.position, Player.Data.ModelData.transform.rotation, Player.Data.ModelData.gameObject.transform);
                }
                ShoveBallPickup shoveBallPickupOnPlayer = PickUp(Player, false, false) as ShoveBallPickup;
                InitializeDeceleratorOnPlayer(shoveBallPickupOnPlayer);
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
        if (Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].CheckAnimationEnded(Pickups.Decelerator)) // maybe need to lose the +1
        {
            Player.Controller.IsUsingPickupRight = false;
            Player.Controller.IsUsingPickupLeft = false;
            Destroy(this.gameObject);
        }
    }
    private void InitializeDeceleratorOnPlayer(ShoveBallPickup shoveBallPickupOnPlayer)
    {
        shoveBallPickupOnPlayer.ShoveBallPrefab = ShoveBallPrefab;
        shoveBallPickupOnPlayer.Player = Player;
        shoveBallPickupOnPlayer.availableVFX = availableVFX;
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
        PickableAbilty pickable = playerGO.AddComponent<DeceleratorPickup>();
        return pickable;
    }
    public override void Use()
    {
        if (IsUsed)
            return;

        if (!Player.Attractor.IsUsingRightAttractor && Player.Controller.IsUsingPickupRight)
        {
            //Player.Controller.IsUsingPickupRight = true;
            //Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
            IsUsed = true;
            SoundManager.Instance.PlaySpikesSound(SoundManager.Instance.SpikesUseSound);
            Destroy(availableVFX);
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].RightGrenade.GetComponent<Renderer>().material = _shoveBallMat;
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayGranadeAnim(false);
            Invoke("ThrowDecelerator", 0.55f);
        }
        else if (!Player.Attractor.IsUsingLeftAttractor && Player.Controller.IsUsingPickupLeft)
        {
            //Player.Controller.IsUsingPickupLeft = true;
            //Player.Controller.BodyTilter.IgnoreTransformsNoY.Remove(availableVFX.transform);
            IsUsed = true;
            SoundManager.Instance.PlaySpikesSound(SoundManager.Instance.SpikesUseSound);
            Destroy(availableVFX);
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].LeftGrenade.GetComponent<Renderer>().material = _shoveBallMat;
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayGranadeAnim(true);
            Invoke("ThrowDecelerator", 0.55f);
        }
    }

    private void ThrowDecelerator()
    {
        Vector3 offset = Player.Data.ProjectileTr.transform.position + Player.Controller.CrosshairParent.forward * _forwardOffset / 2;
        GameObject shoveBallGO = Instantiate(ShoveBallPrefab, offset, Player.Controller.CrosshairParent.transform.rotation);
        ShoveBall shoveBall = shoveBallGO.GetComponent<ShoveBall>();

        shoveBall.ThrowingPlayer = Player;
    }
}
