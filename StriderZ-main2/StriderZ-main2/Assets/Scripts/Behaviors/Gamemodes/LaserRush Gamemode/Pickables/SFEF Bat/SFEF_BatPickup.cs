using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFEF_BatPickup : PickableAbilty
{
    [SerializeField] private float Force = 5;
    [SerializeField] private GameObject SFEFBatObj;
    [SerializeField] private GameObject SFEFBatSwing;
    private Animator _batAnimator;
    private GameObject _newBat;
    private GameObject _newBatSwing;
    private bool _used = false;
    private void Update()
    {
        if (_used)
        {
            /*if (_batAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")|| _batAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Mirror"))
            {
                //Debug.Log(_batAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (_batAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    //Player.Controller.BodyTilter.IgnoreTransforms.Remove(_newBat.transform.GetChild(0).transform);
                    //Destroy(_newBat);
                    //Destroy(_newBatSwing);
                    Destroy(this);
                    _used = false;
                }
            }*/
            if (Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].CheckAnimationEnded(Pickups.Bat))
            {
                Player.Controller.IsUsingPickupRight = false;
                Player.Controller.IsUsingPickupLeft = false;
                Destroy(this.gameObject);
                _used = false;
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
                    availableVFX = Instantiate(PickUpAvailableVFX, Player.gameObject.transform.position, Player.Data.ModelData.transform.rotation, Player.Data.ModelData.gameObject.transform);
                    //Player.Controller.BodyTilter.IgnoreTransformsNoY.Add(availableVFX.transform);
                }
                SFEF_BatPickup batPickupOnPlayer = PickUp(Player, false, false) as SFEF_BatPickup;
                InitializeBatOnPlayer(batPickupOnPlayer);
            }
            else
            {
                Player.Data.PickupItem.SpawnPickupObtainedAgain(Player.Data);
            }
        }
    }

    private void InitializeBatOnPlayer(SFEF_BatPickup batPickupOnPlayer)
    {
        batPickupOnPlayer.Player = Player;
        batPickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        batPickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        batPickupOnPlayer.availableVFX = availableVFX;
        batPickupOnPlayer.Force = Force;
        batPickupOnPlayer.SFEFBatObj = SFEFBatObj;
        batPickupOnPlayer.SFEFBatSwing = SFEFBatSwing;
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
        PickableAbilty pickable = playerGO.AddComponent<SFEF_BatPickup>();
        return pickable;
    }
    public override void Use()
    {
        if (IsUsed)
            return;

        //Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
        //GameObject currentModel = Player.Controller.ModelHandler.AllModels[Player.Data.ModelNum + 1];
        Transform currentModelTransform = Player.transform.GetChild((int)Player.SetupData.ChosenModelType).transform;
        Vector3 currentModelRotation = new Vector3(0, currentModelTransform.rotation.eulerAngles.y, 0);
        //_newBat = Instantiate(SFEFBatObj,transform.position + (Vector3.up*2.5f),Quaternion.Euler(currentModelRotation), gameObject.transform);
        _newBatSwing = Instantiate(SFEFBatSwing, currentModelTransform.position + (transform.up * 2.5f), Quaternion.Euler(currentModelRotation), currentModelTransform);
        //Player.Controller.BodyTilter.IgnoreTransforms.Add(_newBat.transform.GetChild(0).transform);
        //_batAnimator = _newBat.GetComponentInChildren<Animator>();
        if (/*currentModelTransform.rotation.y >= 0 && currentModelTransform.rotation.y <=180 &&*/ !Player.Attractor.IsUsingRightAttractor && Player.Controller.IsUsingPickupRight)
        {
            //Player.Controller.IsUsingPickupRight = true;
            //Player.Controller.BodyTilter.IgnoreTransformsNoY.Remove(availableVFX.transform);
            _used = true;
            IsUsed = true;
            _newBatSwing.transform.Rotate(new Vector3(0, 0, 180));
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayBatAnim(false);
            //SoundManager.Instance.PlayPickUpSound(SoundManager.Instance.BatHitSound);
            Destroy(availableVFX);
            //_batAnimator.SetBool("Attack", true);
        }
        else if (/*currentModelTransform.rotation.y < 0 && currentModelTransform.rotation.y > -180 &&*/ !Player.Attractor.IsUsingLeftAttractor && Player.Controller.IsUsingPickupLeft)
        {
            //Player.Controller.IsUsingPickupLeft = true;
            //Player.Controller.BodyTilter.IgnoreTransformsNoY.Remove(availableVFX.transform);
            _used = true;
            IsUsed = true;
            Player.Data.PickupAnimationManagers[(int)Player.SetupData.ChosenModelType].PlayBatAnim(true);
            //SoundManager.Instance.PlayPickUpSound(SoundManager.Instance.BatHitSound);
            Destroy(availableVFX);
            //_batAnimator.SetBool("AttackMirror", true);
        }
        //_batAnimator.SetBool("Attack",true);

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
