using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasePickup : PickableAbilty
{
    [SerializeField] protected float PhaseTime = 3.0f;
    [SerializeField] protected Material PhaseMat;
    //protected Material[] PlayerMaterials;
    //protected Material[] HelmetMaterials;
    //protected Material[] BodyMaterials;

    protected Collider[] Colliders;
    protected SkinnedMeshRenderer HelmetMesh, BodyMesh;
    protected SkinnedMeshRenderer[] LeftHandMesh, LeftAttractorHandMesh;
    protected SkinnedMeshRenderer[] RightHandMesh, RightAttractorHandMesh;
    public bool IsPhaseDone = false, IsHelmetPhaseDone = false, IsBodyPhaseDone = false;
    public Material[] HelmetMaterials, BodyMaterials, LeftHandMaterials, RightHandMaterials;

    private float _time = 0, _elapsedTime = 0;
    //test
    private float _timer = 0;
    private bool _doPhase = false;
    private bool _disablePhase = false;
    private bool _isUsed = false;


    protected Material[] AllOriginalMats;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) && !_isPickedUp)
        {
            if (_disablePickup)
                return;

            Player = other.GetComponent<PlayerInputHandler>();
            if (Player.Data.PickupItem == null)
            {
                _isPickedUp = true;

                if (Player.Attractor.IsLeftGrabbingPickup(transform))
                    Player.Attractor.CancelAttractorLeft(true);
                else if (Player.Attractor.IsRightGrabbingPickup(transform))
                    Player.Attractor.CancelAttractorRight(true);

                CheckGrabbingPickable();
                Colliders = Player.Coliders;
                HelmetMesh = Player.Data.ModelData.HelmetMesh;
                BodyMesh = Player.Data.ModelData.BodyMesh;
                LeftHandMesh = Player.Data.ModelData.LeftHandMeshes;
                RightHandMesh = Player.Data.ModelData.RightHandMeshes;
                LeftAttractorHandMesh = Player.Data.ModelData.LeftAttractorHandMeshes;
                RightAttractorHandMesh = Player.Data.ModelData.RightAttractorHandMeshes;
                HelmetMaterials = HelmetMesh.materials;
                BodyMaterials = BodyMesh.materials;
                AllOriginalMats = new Material[HelmetMesh.materials.Length];
                AllOriginalMats = HelmetMesh.materials;
                if (PickUpAvailableVFX != null)
                {
                    /*Transform currentModel = playerData.gameObject.transform.GetChild(playerData.ModelNum + 1).transform;
                    Quaternion modelRotation = new Quaternion(0, currentModel.rotation.y, 0, 0);*/
                    availableVFX = Instantiate(PickUpAvailableVFX, Player.gameObject.transform.position, Player.gameObject.transform.rotation, Player.gameObject.transform);
                    Player.Controller.BodyTilter.IgnoreTransforms.Add(availableVFX.transform);
                }
                PhasePickup phasePickupOnPlayer = PickUp(Player, false, false) as PhasePickup;
                InitializePhaseOnPlayer(phasePickupOnPlayer);
            }
            else
            {
                Player.Data.PickupItem.SpawnPickupObtainedAgain(Player.Data);
            }
        }
    }

    private void InitializePhaseOnPlayer(PhasePickup phasePickupOnPlayer)
    {
        phasePickupOnPlayer.Name = Name;
        phasePickupOnPlayer.Player = Player;
        phasePickupOnPlayer.Colliders = Colliders;
        phasePickupOnPlayer.HelmetMesh = HelmetMesh;
        phasePickupOnPlayer.BodyMesh = BodyMesh;
        phasePickupOnPlayer.LeftHandMesh = LeftHandMesh;
        phasePickupOnPlayer.LeftAttractorHandMesh = LeftAttractorHandMesh;
        phasePickupOnPlayer.RightHandMesh = RightHandMesh;
        phasePickupOnPlayer.RightAttractorHandMesh = RightAttractorHandMesh;
        phasePickupOnPlayer.HelmetMaterials = HelmetMaterials;
        phasePickupOnPlayer.BodyMaterials = BodyMaterials;
        phasePickupOnPlayer.LeftHandMaterials = LeftHandMaterials;
        phasePickupOnPlayer.RightHandMaterials = RightHandMaterials;
        phasePickupOnPlayer.PhaseTime = PhaseTime;
        phasePickupOnPlayer.PhaseMat = PhaseMat;
        //phasePickupOnPlayer.PlayerMaterials = PlayerMaterials;
        phasePickupOnPlayer.PickUpObtainVFX = PickUpObtainVFX;
        phasePickupOnPlayer.PickUpAvailableVFX = PickUpAvailableVFX;
        phasePickupOnPlayer.availableVFX = availableVFX;
        phasePickupOnPlayer._isPickedUp = _isPickedUp;
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
        PickableAbilty pickable = playerGO.AddComponent<PhasePickup>();
        return pickable;
    }
    public override void Use()
    {
        if (_isUsed)
            return;

        _isUsed = true;
        Player.Controller.IsUsingPickupRight = false;
        Player.Controller.IsUsingPickupLeft = false;
        SoundManager.Instance.PlayPhaseSound(SoundManager.Instance.PhaseUseSound);
        Player.Controller.BodyTilter.IgnoreTransforms.Remove(availableVFX.transform);
        Destroy(availableVFX);
        availableVFX = null;
        //Player.Controller.IsUsingPickup = true;
        //StartCoroutine(PlayAbility(PhaseTime));
        _doPhase = true;
    }
    private void Update()
    {
        if (_doPhase && !_disablePhase)
        {
            PlayGhost();
            _disablePhase = true;
        }
        if (_disablePhase)
        {
            if (_timer < PhaseTime)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                DisableGhost();
                _disablePhase = false;
                _doPhase = false;

                Destroy(this.gameObject);
                Player.Data.PickupItem = null;
            }
        }
    }
    private void PlayGhost()
    {
        //yield return new WaitForSeconds(0.001f);

        foreach (Collider collider in Colliders)
        {
            collider.isTrigger = true;
        }

        IsUsed = true;
        IsActive = true;

        // apply ghost
        if (Player)
        {
            Material[] helmetMats = new Material[AllOriginalMats.Length];
            Material[] bodyAndHandMats = new Material[AllOriginalMats.Length - 1];
            Material fingersMat = new Material(PhaseMat);

            for (int i = 0; i < helmetMats.Length; i++)
                helmetMats[i] = PhaseMat;

            for (int i = 0; i < bodyAndHandMats.Length; i++)
                bodyAndHandMats[i] = PhaseMat;

            HelmetMesh.materials = helmetMats;
            BodyMesh.materials = bodyAndHandMats;

            for (int i = 0; i < LeftHandMesh.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        LeftHandMesh[i].materials = bodyAndHandMats;
                        RightHandMesh[i].materials = bodyAndHandMats;
                        LeftAttractorHandMesh[i].materials = bodyAndHandMats;
                        RightAttractorHandMesh[i].materials = bodyAndHandMats;
                        break;

                    default:
                        LeftHandMesh[i].material = PhaseMat;
                        RightHandMesh[i].material = PhaseMat;
                        LeftAttractorHandMesh[i].material = PhaseMat;
                        RightAttractorHandMesh[i].material = PhaseMat;
                        break;
                }
            }
        }
    }
    private void DisableGhost()
    {
        //Player.Controller.IsUsingPickup = false;
        foreach (Collider collider in Colliders)
        {
            collider.isTrigger = false;
        }

        IsUsed = false;
        IsActive = false;

        _time = _elapsedTime = 0;

        /*Player.Data.BodyMesh.transform.parent.gameObject.SetActive(true);
        Player.Controller.ModelHandler.TempPhaseModel.SetActive(false);*/

        Material[] helmetMats = new Material[AllOriginalMats.Length];
        Material[] bodyAndHandMats = new Material[AllOriginalMats.Length - 1];
        Material fingersMat = new Material(AllOriginalMats[0]);

        helmetMats = AllOriginalMats;

        for (int i = 0; i < bodyAndHandMats.Length; i++)
            bodyAndHandMats[i] = helmetMats[i];

        HelmetMesh.materials = helmetMats;
        BodyMesh.materials = bodyAndHandMats;

        for (int i = 0; i < LeftHandMesh.Length; i++)
        {
            switch (i)
            {
                case 0:
                    LeftHandMesh[i].materials = bodyAndHandMats;
                    RightHandMesh[i].materials = bodyAndHandMats;
                    LeftAttractorHandMesh[i].materials = bodyAndHandMats;
                    RightAttractorHandMesh[i].materials = bodyAndHandMats;
                    break;

                default:
                    LeftHandMesh[i].material = fingersMat;
                    RightHandMesh[i].material = fingersMat;
                    LeftAttractorHandMesh[i].material = fingersMat;
                    RightAttractorHandMesh[i].material = fingersMat;
                    break;
            }
        }


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
