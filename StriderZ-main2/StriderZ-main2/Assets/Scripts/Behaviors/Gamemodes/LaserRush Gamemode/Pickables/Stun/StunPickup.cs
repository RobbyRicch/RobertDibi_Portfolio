using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPickup : PickableAbilty
{
    [SerializeField] protected float StunTime = 1.0f;
    private void Update()
    {
        if (IsUsed)
        {
            if (Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].CheckAnimationEnded(Pickups.DRS))
            {
                Player.Controller.IsUsingPickupRight = false;
                Player.Controller.IsUsingPickupLeft = false;
            }
        }
    }
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
                StunPickup stunPickupOnPlayer = PickUp(Player, false, false) as StunPickup;
                InitializeStunOnPlayer(stunPickupOnPlayer);
            }
            else
            {
                Player.Data.PickupItem.SpawnPickupObtainedAgain(Player.Data);
            }
        }
    }
    private void InitializeStunOnPlayer(StunPickup stunPickupOnPlayer)
    {
        stunPickupOnPlayer.Player = Player;
        stunPickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        stunPickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        stunPickupOnPlayer.availableVFX = availableVFX;
        stunPickupOnPlayer.StunTime = StunTime;
        pickupRandomizer = null;
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
        PickableAbilty pickable = playerGO.AddComponent<StunPickup>();
        return pickable;
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
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayDRSAnim(false);
            StartStun();
        }
        else if (!Player.Attractor.IsUsingLeftAttractor && Player.Controller.IsUsingPickupLeft)
        {
            //Player.Controller.IsUsingPickupLeft = true;
            Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
            IsUsed = true;
            Destroy(availableVFX);
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayDRSAnim(true);
            StartStun();
        }
    }

    private void StartStun()
    {
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            PlayerInputHandler player = allPlayersAlive[i];
            if (player != Player)
            {
                SoundManager.Instance.PlayStunSound(SoundManager.Instance.StunHitSounds);
                PlayerController playerController = player.GetComponent<PlayerController>();
                GrappleController grappleController = player.GetComponent<GrappleController>();

                playerController.InputHandler.Data.StrikeVFX.Play();
                StartCoroutine(DoStun(playerController, grappleController, StunTime));
            }
        }
    }
    private IEnumerator DoStun(PlayerController playerController, GrappleController grappleController, float sec)
    {
        yield return new WaitForSeconds(1);
        playerController.InputHandler.Data.StunVFX.Play();
        playerController.IsRooted = true;
        //grappleController.enabled = false;
        playerController.Rb.velocity = Vector3.zero;
        playerController.Rb.velocity = Vector3.ClampMagnitude(playerController.Rb.velocity, 0);
        playerController.transform.position = playerController.transform.position;

        //playerController.transform.position = pos;
        /*playerController.Rb.velocity = Vector3.ClampMagnitude(playerController.Rb.velocity, 0);
        playerController.Rb.velocity = Vector3.zero;*/
        yield return new WaitForSeconds(sec);

        playerController.IsRooted = false;
        //grappleController.enabled = true;
        Destroy(this.gameObject);
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
