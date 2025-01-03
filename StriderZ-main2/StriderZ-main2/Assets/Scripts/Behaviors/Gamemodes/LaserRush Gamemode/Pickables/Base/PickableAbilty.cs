using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;


public abstract class PickableAbilty : MonoBehaviour
{
    [SerializeField] protected string Name;
    [SerializeField] protected int PickupIconNum;
    [SerializeField] protected Material PickupIconMat;
    [SerializeField] protected GameObject PickUpObtainVFX;
    [SerializeField] protected GameObject PickUpObtainAgainVFX;
    [SerializeField] protected GameObject PickUpAvailableVFX;
    protected const string PlayerTag = "Player";
    protected bool IsOnPlayer = false, IsActive = false;
    public GameObject availableVFX;
    public bool IsUsed = false;
    protected bool _isPickedUp = false;
    protected bool _isInArena = false;
    public bool isPickedUp { get { return _isPickedUp; } }
    public bool IsInArena { get { return _isInArena; } set { _isInArena = value; } }
    public PickupRandomizer pickupRandomizer;
    protected PlayerInputHandler Player;
    [SerializeField] protected GameObject PickupModel;
    [SerializeField] protected BoxCollider PickupCollider;
    protected bool _disablePickup = false;

    abstract public void Use();
    abstract protected PickableAbilty AddPickable(GameObject playerGO);
    abstract protected PickableAbilty AddPickableChild(GameObject playerGO);
    virtual protected void Activate() { }
    private void OnEnable()
    {
        EventManager.OnPlayerDeath += OnPlayerDeath;
        EventManager.OnPlayerVictory += OnPlayerVictory;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerDeath -= OnPlayerDeath;
        EventManager.OnPlayerVictory -= OnPlayerVictory;
    }
    protected void InitializePickupItem(PickableAbilty pickupItem, bool isUsed, bool isActive)
    {
        pickupItem.IsOnPlayer = true;
        pickupItem.IsUsed = isUsed;
        pickupItem.IsActive = isActive;
        //pickupItem.availableVFX = availableVFX;
    }
    protected PickableAbilty PickUp(PlayerInputHandler player, bool isUsed, bool isActive)
    {
        if (!IsOnPlayer && !player.Data.PickupItem)
        {
            //UIManager.Instance.SetPlayerActivePickup(playerData.ID, PickupIconNum);
            //playerData.PlayerWorldUI.SetPlayerActivePickup(PickupIconNum);
            //playerData.PickupItem = AddPickable(playerData.gameObject);
            player.Data.PickupItem = AddPickableChild(player.gameObject);
            //InitializePickupItem(playerData.PickupItem, isUsed, isActive);
            SoundManager.Instance.PlayPickUpSound(SoundManager.Instance.PickUpSound);
            ChangeObtainedIcon(player.PlayerWorldUI);
            SpawnPickupObtainedAgain(player.Data);
            if (_isInArena)
                StartCoroutine(pickupRandomizer.Respawn(transform));
            return player.Data.PickupItem;
        }
        else if (player.Data.PickupItem)
        {
            // logic for what happens if already pickable exists
            return null;
        }

        return null;
    }
    public void SpawnPickupObtainedAgain(PlayerData playerData)
    {
        ChangeObtainedIcon(playerData.PlayerWorldUI);
        GameObject vfx = Instantiate(PickUpObtainAgainVFX, playerData.transform.position, Quaternion.identity, playerData.transform);
        Destroy(vfx, 2f);
    }
    private void ChangeObtainedIcon(PlayerWorldUI playerWorldUI)
    {
        if (PickUpObtainVFX)
        {
            PickUpObtainVFX.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = PickupIconMat;
        }
        if (PickUpObtainAgainVFX)
        {
            PickUpObtainAgainVFX.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = PickupIconMat;
        }
    }
    protected void CheckGrabbingPickable()
    {
        if (transform.GetChild(transform.childCount - 1).CompareTag("Hand"))
        {
            transform.GetChild(transform.childCount - 1).GetComponent<AttractorHand>().CancelAttractor();
        }
    }

    
    IEnumerator RemoveTransformAvailableVFX(PlayerInputHandler player)
    {
        yield return null;
        for (int i = 0; i < player.Controller.BodyTilter.IgnoreTransforms.Count; i++)
        {
            if (player.Controller.BodyTilter.IgnoreTransforms[i] == availableVFX.transform)
            {
                player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
                break;
            }
        }
        Destroy(player.Data.PickupItem.availableVFX);
        Destroy(player.Data.PickupItem.gameObject);
        player.Data.PickupItem = null;
    }
    private void OnPlayerDeath(PlayerInputHandler player)
    {
        if (player.Data.PickupItem != null && player.Data.PickupItem == this)
            StartCoroutine(RemoveTransformAvailableVFX(player));
    }
    private void OnPlayerVictory(PlayerInputHandler player)
    {
        _disablePickup = true;
    }
    /*private void OnDestroy()
    {
        if (IsOnPlayer)
            return;

        // error: NullReferenceException: Object reference not set to an instance of an object PickableAbilty.OnDestroy()(at Assets / Scripts / Behaviors / Pickables / Base / PickableAbilty.cs:74)

        if (Player.Attractor.IsLeftGrabbingPickup(transform)) 
            Player.Attractor.CancelAttractorLeft(true);
        else if (Player.Attractor.IsRightGrabbingPickup(transform))
            Player.Attractor.CancelAttractorRight(true);
    }*/
}
