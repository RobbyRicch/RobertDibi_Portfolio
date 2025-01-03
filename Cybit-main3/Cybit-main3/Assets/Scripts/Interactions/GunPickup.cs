using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private GunBase _gunToPickup;

    [SerializeField] private BoxCollider2D _collider;
    public BoxCollider2D Collider => _collider;

    [SerializeField] private Transform _worldTr;
    public Transform WorldTr { get => _worldTr; set => _worldTr = value; }

    private delegate void HandlePickupAction(Player_Controller playerController);
    private HandlePickupAction _handlePickup;

    private const string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_playerTag))
        {
            Player_Controller playerController = collision.GetComponent<Player_Controller>();
            if (!playerController)
                return;

            if (!playerController.IsPickingUp)
                HandlePickup(playerController);
        }
    }

    private void TriggerReplacePrimaryGunPrompt(Player_Controller playerController)
    {
        // get input from player
        bool isTrue = true;

        if (!isTrue)
            return;

        playerController.ReplacePrimaryGun(_gunToPickup as GunPrimary, _worldTr);
    }
    private void TriggerReplaceSideArmPrompt(Player_Controller playerController)
    {
        // get input from player
        bool isTrue = true;

        if (!isTrue)
            return;

        playerController.ReplaceSideArm(_gunToPickup as GunSideArm, _worldTr);
    }
    private void PickupPrimary(Player_Controller playerController)
    {
        if (playerController.PrimaryGun != null)
            TriggerReplacePrimaryGunPrompt(playerController);
        else
            playerController.ReplacePrimaryGun(_gunToPickup as GunPrimary, _worldTr);
    }
    private void PickupSideArm(Player_Controller playerController)
    {
        if (playerController.SideArm != null)
            TriggerReplaceSideArmPrompt(playerController);
        else
            playerController.ReplaceSideArm(_gunToPickup as GunSideArm, _worldTr);
    }
    private void Pickup(Player_Controller playerController)
    {
        if (_handlePickup != null)
            _handlePickup.Invoke(playerController);
    }
    private void ChoosePickupMethod(Player_Controller playerController)
    {
        if (_gunToPickup.SlotType == GunType.Primary)
            _handlePickup = PickupPrimary;
        else
            _handlePickup = PickupSideArm;

        CinemachineShake cameraShake = playerController.CurrentVirtualCamera.GetComponent<CinemachineShake>();

        if (_gunToPickup is GunShotgun shotgun)
            shotgun.CameraShake = cameraShake;
        else if (_gunToPickup is GunHeatable heatable)
            heatable.CameraShake = cameraShake;
    }
    private void HandlePickup(Player_Controller playerController)
    {
        playerController.StartResetPickup();
        ChoosePickupMethod(playerController);
        Pickup(playerController);
    }
}