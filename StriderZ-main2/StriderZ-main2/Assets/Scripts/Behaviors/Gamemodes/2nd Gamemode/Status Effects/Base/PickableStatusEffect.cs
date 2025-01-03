using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickableStatusEffect : MonoBehaviour
{
    [SerializeField] protected PuckStatusEffects StatusEffect;
    [SerializeField] protected int PickupIconNum;
    [SerializeField] protected GameObject PickUpObtainVFX;
    [SerializeField] protected GameObject PickUpAvailableVFX;
    protected const string PlayerTag = "Player";
    protected bool IsOnPlayer = false;
    public GameObject availableVFX;

    abstract protected PickableStatusEffect AddPickable(GameObject playerGO);
    abstract protected void Effect(GameObject Puck);
    protected void InitializePickupItem(PickableStatusEffect pickupItem)
    {
        pickupItem.IsOnPlayer = true;
        //pickupItem.availableVFX = availableVFX;
    }
    protected PickableStatusEffect PickUp(PlayerData playerData)
    {
        if (!IsOnPlayer && !playerData.PickupItem)
        {
            if (PickUpAvailableVFX != null)
            {
                availableVFX = Instantiate(PickUpAvailableVFX, playerData.gameObject.transform.position, Quaternion.identity, playerData.gameObject.transform);
                playerData.gameObject.GetComponent<BodyTilt>().IgnoreTransforms.Add(availableVFX.transform);
            }
            playerData.PickupStatusEffectItem = AddPickable(playerData.gameObject);
            InitializePickupItem(playerData.PickupStatusEffectItem);
            SoundManager.Instance.PlayPickUpSound(SoundManager.Instance.PickUpSound);
            //playerData.PlayerWorldUI.SetPlayerActivePickup(PickupIconNum);
            ChangeObtainedIcon(playerData.PlayerWorldUI);
            GameObject vfx = Instantiate(PickUpObtainVFX, playerData.transform.position, Quaternion.identity, playerData.transform);
            Destroy(vfx, 5f);
            Destroy(gameObject);
            return playerData.PickupStatusEffectItem;
        }
        else if (playerData.PickupItem)
        {
            // logic for what happens if already pickable exists
            return null;
        }

        return null;
    }
    private void ChangeObtainedIcon(PlayerWorldUI playerWorldUI)
    {
        if (PickUpObtainVFX)
        {
            //PickUpObtainVFX.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = playerWorldUI._pickupObtainIcons[PickupIconNum];
        }
    }
}
