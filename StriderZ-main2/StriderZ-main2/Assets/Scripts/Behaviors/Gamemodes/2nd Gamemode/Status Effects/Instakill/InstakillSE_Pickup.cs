using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstakillSE_Pickup : PickableStatusEffect
{
    protected PlayerInputHandler Player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            Player = other.GetComponent<PlayerInputHandler>();
            Player.GetComponentInChildren<DeflectorAbility>().CurrentStatusEffect = StatusEffect;
            InstakillSE_Pickup InstakillPickupOnPlayer = PickUp(Player.Data) as InstakillSE_Pickup;
            InitializeInstakillOnPlayer(InstakillPickupOnPlayer);
        }
    }
    private void InitializeInstakillOnPlayer(InstakillSE_Pickup instakillPickupOnPlayer)
    {
        instakillPickupOnPlayer.Player = Player;
        instakillPickupOnPlayer.StatusEffect = StatusEffect;
        instakillPickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        instakillPickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        instakillPickupOnPlayer.availableVFX = availableVFX;
    }
    protected override PickableStatusEffect AddPickable(GameObject playerGO)
    {
        InstakillSE_Pickup pickable = playerGO.AddComponent<InstakillSE_Pickup>();
        return pickable;
    }

    protected override void Effect(GameObject Puck)
    {
        Destroy(availableVFX);

        Destroy(this);
    }
}
