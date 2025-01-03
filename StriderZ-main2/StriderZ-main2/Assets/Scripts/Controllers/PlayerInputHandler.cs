using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerSetupData _setupData;
    public PlayerSetupData SetupData => _setupData;

    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;

    [SerializeField] private PlayerController _controller;
    public PlayerController Controller => _controller;

    [SerializeField] private PlayerWorldUI _playerWorldUI;
    public PlayerWorldUI PlayerWorldUI => _playerWorldUI;

    [SerializeField] private Collider[] _colliders;
    public Collider[] Coliders => _colliders;

    [SerializeField] private AttractorController _attractor;
    public AttractorController Attractor => _attractor;

    [SerializeField] private float _emmisionMultiplier = 8.0f, _emmisionScreenMultiplier = 8.0f;
    public float EmmisionMultiplier => _emmisionMultiplier;
    public float EmmisionScreenMultiplier => _emmisionScreenMultiplier;

    [SerializeField] private bool _isPlayerInputsDisabled = false;
    public bool IsPlayerInputsDisable { get => _isPlayerInputsDisabled; set => _isPlayerInputsDisabled = value; }

    [SerializeField] private List<Material> _allMaterialInstances;
    public List<Material> AllMaterialInstances => _allMaterialInstances;

    private PlayerControls _controls;
    public PlayerControls Controls => _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    public void Initialize(PlayerSetupData newSetupData)
    {
        _setupData = newSetupData;
        _data.ID = newSetupData.ID;

        _setupData.Input.onActionTriggered += Input_onActionTriggered;
    }
    public void SetColorsOnModel()
    {
        ColorData colorData = _setupData.ColorData;

        #region Helmet
        // setup helmet materials
        Material[] newHelmetMats = new Material[]
        {
            _data.ModelData.HelmetMesh.materials[0], _data.ModelData.HelmetMesh.materials[1],
            _data.ModelData.HelmetMesh.materials[2], _data.ModelData.HelmetMesh.materials[3]
        };
        _allMaterialInstances.AddRange(newHelmetMats);

        // base material
        newHelmetMats[0].SetColor("_BaseColor", colorData.BaseBaseColor);
        newHelmetMats[0].SetColor("_EmissionColor", colorData.BaseEmissionColor);

        // emission material
        newHelmetMats[1].SetColor("_BaseColor", colorData.EmissionBaseColor);
        newHelmetMats[1].SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // detail material
        newHelmetMats[2].SetColor("_BaseColor", colorData.DetailBaseColor);
        newHelmetMats[2].SetColor("_EmissionColor", colorData.DetailEmissionColor);

        // Screen material
        newHelmetMats[3].SetColor("_FaceLineColor", colorData.FaceColor);

        // assign helmet materials
        _data.ModelData.HelmetMesh.materials = newHelmetMats;
        #endregion

        #region Body & Hands
        // setup body materials based on helmet materials
        Material[] newBodyHandsMats = new Material[]
        {
            newHelmetMats[0], newHelmetMats[1],
            newHelmetMats[2]
        };

        // assign body materials
        _data.ModelData.BodyMesh.materials = newBodyHandsMats;
        _allMaterialInstances.AddRange(newBodyHandsMats);

        // setup fingers material based on helmet material
        Material newFingerMat = new Material(newHelmetMats[0]);
        for (int i = 0; i < _data.ModelData.LeftHandMeshes.Length; i++)
        {
            switch (i)
            {
                case 0: // assign hands materials
                    _data.ModelData.LeftHandMeshes[i].materials = newBodyHandsMats;
                    _data.ModelData.RightHandMeshes[i].materials = newBodyHandsMats;
                    _data.ModelData.LeftAttractorHandMeshes[i].materials = newBodyHandsMats;
                    _data.ModelData.RightAttractorHandMeshes[i].materials = newBodyHandsMats;
                    break;
                default: // assign fingers material
                    _data.ModelData.LeftHandMeshes[i].material = newFingerMat;
                    _data.ModelData.RightHandMeshes[i].material = newFingerMat;
                    _data.ModelData.LeftAttractorHandMeshes[i].material = newFingerMat;
                    _data.ModelData.RightAttractorHandMeshes[i].material = newFingerMat;
                    break;
            }
        }
        _allMaterialInstances.Add(newFingerMat);
        #endregion

        // set aura color
        /*ParticleSystemRenderer auraParticleRenderer = _data.Aura.GetComponent<ParticleSystemRenderer>();
        Material coloredAuraMat = new Material(auraParticleRenderer.material);
        coloredAuraMat.color = _data.SecondaryColor;
        coloredAuraMat.SetColor("_EmissionColor", _data.SecondaryEmmisionColor / 2);
        auraParticleRenderer.material = coloredAuraMat;*/

        // set tip point color
        _playerWorldUI.ChangeEnergyColor(colorData.EmissionEmissionColor);

        SetTrailsColor(colorData);

        #region Effect Coloring
        ParticleSystemRenderer throwPaticleRenderer = _attractor.ThrowEffects[0].GetComponent<ParticleSystemRenderer>();
        SetThrowVfxColor(throwPaticleRenderer.trailMaterial, colorData);

        ParticleSystemRenderer onHandPaticleRenderer = _attractor.OnHandEffects[0].GetComponent<ParticleSystemRenderer>();
        SetOnHandVfxColor(onHandPaticleRenderer.trailMaterial, colorData);

        SetInAirBeamsColor(colorData);
        SetPullBeamsColor(colorData);
        SetPulledBeamsColor(colorData);
        SetSpeedLinesColor(colorData);
        #endregion

        /*Material coloredMat = new Material(templateParticleRenderer.trailMaterial);
        coloredMat.SetColor("_EmissionColor", colorData.EmissionColor);

        // set throw effect color
        for (int i = 0; i < _attractor.ThrowEffects.Length; i++)
        {
            ParticleSystemRenderer particleRenderer = _attractor.ThrowEffects[i].GetComponent<ParticleSystemRenderer>();
            ParticleSystem.MainModule mainModule = _attractor.ThrowEffects[i].main;
            mainModule.startColor = colorData.EmissionColor;
            particleRenderer.trailMaterial = coloredMat;
        }*/

        /*
        // set on hand effect color
        for (int i = 0; i < _attractor.OnHandEffects.Length; i++)
        {
            ParticleSystemRenderer particleRenderer = _attractor.OnHandEffects[i].GetComponent<ParticleSystemRenderer>();
            ParticleSystem.MainModule mainModule = _attractor.OnHandEffects[i].main;
            mainModule.startColor = colorData.EmissionColor;
            particleRenderer.trailMaterial = coloredMat;
        }*/

        /*// set beams colors
        Material inAirBeam = new Material(_attractor.InAirBeams[0].material);

        Material leftPullBeamOuter = new Material(_attractor.LeftPullBeams[0].material);
        Material leftPulledBeamOuter = new Material(_attractor.LeftPulledBeams[0].material);

        Material rightPullBeamOuter = new Material(_attractor.RightPullBeams[0].material);
        Material rightPulledBeamOuter = new Material(_attractor.RightPulledBeams[0].material);

        inAirBeam.SetColor("_EmissionColor", colorData.EmissionColor);

        leftPullBeamOuter.SetColor("_EmissionColor", colorData.EmissionColor);
        leftPulledBeamOuter.SetColor("_EmissionColor", colorData.EmissionColor);

        rightPullBeamOuter.SetColor("_EmissionColor", colorData.EmissionColor);
        rightPulledBeamOuter.SetColor("_EmissionColor", colorData.EmissionColor);

        // assign beam materials
        _attractor.InAirBeams[0].material = inAirBeam;
        _attractor.InAirBeams[1].material = inAirBeam;

        _attractor.LeftPullBeams[0].material = leftPullBeamOuter;
        _attractor.LeftPulledBeams[0].material = leftPulledBeamOuter;

        _attractor.RightPullBeams[0].material = rightPullBeamOuter;
        _attractor.RightPulledBeams[0].material = rightPulledBeamOuter;*/

        /*// speed lines color
        TrailRenderer[] _speedParticles = new TrailRenderer[_data.SpeedLines.transform.childCount];
        _speedParticles = _data.SpeedLines.transform.GetComponentsInChildren<TrailRenderer>();

        for (int i = 0; i < _speedParticles.Length; i++)
            _speedParticles[i].material = coloredMat;*/

    }

    private void Input_onActionTriggered(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        /* if (action name is matching spesific input map name) call this function from player controller */
        if (obj.action.name == _controls.Player.Start.name)
            _controller.OnStartBtn(obj);

        if (!GameManager.Instance.IsGamePaused)
        {
            if (obj.action.name == _controls.Player.Movement.name)
                _controller.OnMove(obj);

            if (!_isPlayerInputsDisabled)
            {
                if (obj.action.name == _controls.Player.Look.name)
                    _controller.OnLook(obj);

                else if (obj.action.name == _controls.Player.LeftTrigger.name)
                    _controller.OnLeftTrigger(obj);

                else if (obj.action.name == _controls.Player.RightTrigger.name)
                    _controller.OnRightTrigger(obj);

                else if (obj.action.name == _controls.Player.LeftShoulder.name)
                    _controller.OnLeftShoulderBtn(obj);

                else if (obj.action.name == _controls.Player.RightShoulder.name)
                    _controller.OnRightShoulderBtn(obj);

                else if (obj.action.name == _controls.Player.SouthButton.name)
                    _controller.OnSouthBtn(obj);

                else if (obj.action.name == _controls.Player.WestButton.name)
                    _controller.OnWestBtn(obj);

                else if (obj.action.name == _controls.Player.NorthButton.name)
                    _controller.OnNorthBtn(obj);

                else if (obj.action.name == _controls.Player.EastButton.name)
                    _controller.OnEastBtn(obj);

                else if (obj.action.name == _controls.Player.DPad.name)
                    _controller.OnDPad(obj);

                else if (obj.action.name == _controls.Player.EmotesKeyboard.name)
                    _controller.OnDPad(obj);
            }
        }
    }

    private void SetTrailsColor(ColorData colorData)
    {
        // set trail gradient
        Gradient gradient = new Gradient();
        gradient.alphaKeys = new GradientAlphaKey[4]
        {
            new GradientAlphaKey(0.9f, 0.0f), new GradientAlphaKey(0.9f, 0.1f),
            new GradientAlphaKey(0.3f, 0.2f), new GradientAlphaKey(0.0f, 1.0f)
        };
        gradient.colorKeys = new GradientColorKey[3] // wanted to try base + emission + detail
        {
            new GradientColorKey(colorData.EmissionEmissionColor, 0.0f), new GradientColorKey(colorData.EmissionEmissionColor, 0.1f),
            new GradientColorKey(colorData.EmissionEmissionColor, 1.0f)
        };

        // assign trail gradient
        _data.MainTrail.colorGradient = gradient;
    }
    private void SetThrowVfxColor(Material matToColor, ColorData colorData)
    {
        // set throw effect color
        Material coloredMat = new Material(matToColor);
        coloredMat.SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // assign throw effect color
        for (int i = 0; i < _attractor.ThrowEffects.Length; i++)
        {
            ParticleSystemRenderer particleRenderer = _attractor.ThrowEffects[i].GetComponent<ParticleSystemRenderer>();
            ParticleSystem.MainModule mainModule = _attractor.ThrowEffects[i].main;
            mainModule.startColor = colorData.EmissionEmissionColor;
            particleRenderer.trailMaterial = coloredMat;
        }
    }
    private void SetOnHandVfxColor(Material matToColor, ColorData colorData)
    {
        // set on hand effect color
        Material coloredMat = new Material(matToColor);
        coloredMat.SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // assign on hand effect color
        for (int i = 0; i < _attractor.OnHandEffects.Length; i++)
        {
            ParticleSystemRenderer particleRenderer = _attractor.OnHandEffects[i].GetComponent<ParticleSystemRenderer>();
            ParticleSystem.MainModule mainModule = _attractor.OnHandEffects[i].main;
            mainModule.startColor = colorData.EmissionEmissionColor;
            particleRenderer.trailMaterial = coloredMat;
        }
    }
    private void SetInAirBeamsColor(ColorData colorData)
    {
        // set in air beams colors
        Material inAirBeam = new Material(_attractor.InAirBeams[0].material);
        inAirBeam.SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // assign beam materials
        _attractor.InAirBeams[0].material = inAirBeam;
        _attractor.InAirBeams[1].material = inAirBeam;
    }
    private void SetPullBeamsColor(ColorData colorData)
    {
        Material leftPullBeamOuter = new Material(_attractor.LeftPullBeams[0].material);
        Material rightPullBeamOuter = new Material(_attractor.RightPullBeams[0].material);

        // set pull beams colors
        leftPullBeamOuter.SetColor("_EmissionColor", colorData.EmissionEmissionColor);
        rightPullBeamOuter.SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // assign pull beams colors
        _attractor.LeftPullBeams[0].material = leftPullBeamOuter;
        _attractor.RightPullBeams[0].material = rightPullBeamOuter;
    }
    private void SetPulledBeamsColor(ColorData colorData)
    {
        Material leftPulledBeamOuter = new Material(_attractor.LeftPulledBeams[0].material);
        Material rightPulledBeamOuter = new Material(_attractor.RightPulledBeams[0].material);

        // set pulled beams colors
        leftPulledBeamOuter.SetColor("_EmissionColor", colorData.EmissionEmissionColor);
        rightPulledBeamOuter.SetColor("_EmissionColor", colorData.EmissionEmissionColor);

        // assign pulled beams colors
        _attractor.LeftPulledBeams[0].material = leftPulledBeamOuter;
        _attractor.RightPulledBeams[0].material = rightPulledBeamOuter;
    }
    private void SetSpeedLinesColor(ColorData colorData)
    {
        TrailRenderer[] tempSpeedTrail = _data.SpeedLines.transform.GetComponentsInChildren<TrailRenderer>();
        TrailRenderer[] speedTrails = new TrailRenderer[tempSpeedTrail.Length];

        // set speed lines color
        Gradient gradient = new Gradient();
        Color color = colorData.EmissionEmissionColor;
        gradient.alphaKeys = new GradientAlphaKey[2] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
        gradient.colorKeys = new GradientColorKey[2] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) };

        for (int i = 0; i < speedTrails.Length; i++)
        {
            // assign speed lines color
            speedTrails[i] = tempSpeedTrail[i];
            speedTrails[i].colorGradient = gradient;
            _data.SpeedLines.transform.GetComponentsInChildren<TrailRenderer>()[i] = speedTrails[i];
        }
    }

    private void OnDestroy()
    {
        _setupData.Input.onActionTriggered -= Input_onActionTriggered;
    }
}