using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SkillType
{
    Speedster,
    Bruiser,
    Gunslinger,
    Mixed,
    NoPath
}

public enum StatType
{
    TimeToMeleeHit,
    FocusCost,
    DodgeRate,
    DashSpeed,
    AmmoCount,
    MaxHealth,
    MeleeDamage,
    PickupHealthRate,
    Speed,
    ReloadTime,
    FireRate,
}

public class PassiveSkillTree_Manager : MonoBehaviour, IPersistable
{
    [Header("Settings")]
    [SerializeField] private int _mainPathSkillThreshold = 3;
    [SerializeField] private int _secondaryPathSkillsThreshold = 2;
    [SerializeField] private bool _isAlpha = false;
    [SerializeField] Image[] _treesBg;

    [Header("Component")]
    [SerializeField] private TextMeshProUGUI _currencyTMP;

    [Header("Scene Objects")]
    [SerializeField] private GameObject[] _pathsUI;
    [SerializeField] private Button _comboSpeedBruiseBtn;
    [SerializeField] private Button _comboSpeedGunBtn;
    [SerializeField] private Button _comboBruiseGunBtn;
    [SerializeField] private Button _comboBruiseSpeedBtn;
    [SerializeField] private Button _comboGunSpeedBtn;
    [SerializeField] private Button _comboGunBruiseBtn;

    [Header("Data")]
    private Player_Controller _playerController;
    public SkillTreePath SpeedsterPath { get; private set; }
    public SkillTreePath BruiserPath { get; private set; }
    public SkillTreePath GunslingerPath { get; private set; }

    public List<PassiveSkill> speedsterSkills;
    public List<PassiveSkill> bruiserSkills;
    public List<PassiveSkill> gunslingerSkills;
    public List<PassiveSkill> comboSkills;

    [SerializeField] private Color[] _treeColors = new Color[] { Color.white, Color.white, Color.white };

    private Dictionary<SkillType, SkillTreePath> paths;
    private HashSet<SkillType> activePaths = new HashSet<SkillType>();
    private SkillType mainPath = SkillType.NoPath;
    private SkillType secondaryPath = SkillType.NoPath;
    private bool _isMainPathSet = false;
    private bool _isSecondaryPathSet = false;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _meleeSpeedBonusTMPro;
    [SerializeField] private TextMeshProUGUI _focusCostReductionTMPro;
    [SerializeField] private TextMeshProUGUI _additionalHealthTMPro;
    [SerializeField] private TextMeshProUGUI _meleeDamageBonusTMPro;
    [SerializeField] private TextMeshProUGUI _reloadTimeBonusTMPro;
    [SerializeField] private TextMeshProUGUI _fireRateBonusTMPro;

    [Header("Save & Load related")]
    [SerializeField] private bool _shouldBeActiveOnStart = true;
    public bool ShouldBeActiveOnStart { get => _shouldBeActiveOnStart; set => _shouldBeActiveOnStart = value; }

    private float tempTimeScale = 0.0f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        EventManager.InvokePause();
        tempTimeScale = Time.timeScale;
        //Time.timeScale = 0;
    }
    private void Start()
    {
        SpeedsterPath = new SkillTreePath(SkillType.Speedster);
        BruiserPath = new SkillTreePath(SkillType.Bruiser);
        GunslingerPath = new SkillTreePath(SkillType.Gunslinger);

        paths = new Dictionary<SkillType, SkillTreePath>
        {
            { SkillType.Speedster, SpeedsterPath },
            { SkillType.Bruiser, BruiserPath },
            { SkillType.Gunslinger, GunslingerPath }
        };

        InitializeSkills();
        InitializeStats();

        EventManager.OnVendingMachineInteract += OnVendingMachineInteract;

    }
    private void OnDisable()
    {
        ToolltipManager.Instance.DestroyTooltip();
        //Time.timeScale = tempTimeScale;
    }
    private void OnDestroy()
    {
        EventManager.OnVendingMachineInteract -= OnVendingMachineInteract;


        foreach (PassiveSkill passiveSkill in speedsterSkills)
        {
            passiveSkill.isUnlocked = false;
        }

        foreach (PassiveSkill passiveSkill in bruiserSkills)
        {
            passiveSkill.isUnlocked = false;
        }

        foreach (PassiveSkill passiveSkill in gunslingerSkills)
        {
            passiveSkill.isUnlocked = false;
        }
    }

    private void InitializeStats()
    {
        _meleeSpeedBonusTMPro.text = "-";
        _focusCostReductionTMPro.text = "-";
        _additionalHealthTMPro.text = "-";
        _meleeDamageBonusTMPro.text = "-";
        _reloadTimeBonusTMPro.text = "-";
        _fireRateBonusTMPro.text = "-";
    }
    private void InitializeSkills()
    {
        foreach (PassiveSkill passiveSkill in speedsterSkills)
        {
            SpeedsterPath.AddSkill(passiveSkill);
        }

        foreach (PassiveSkill passiveSkill in bruiserSkills)
        {
            BruiserPath.AddSkill(passiveSkill);
        }

        foreach (PassiveSkill passiveSkill in gunslingerSkills)
        {
            GunslingerPath.AddSkill(passiveSkill);
        }
    }

    public void OpenMenu(Player_Controller player)
    {
        _playerController = player;
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateStatsUI();
        EventManager.InvokePause();
        tempTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }
    public void CloseMenu()
    {
        EventManager.InvokePause();
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _playerController.IsInputDisabled = false;
        Time.timeScale = tempTimeScale;
    }

    private string GetValueDifferencePercentage(float currentValue, float originalValue)
    {
        float percentage = Mathf.Abs((currentValue - originalValue) / originalValue * 100.0f);
        return percentage.ToString();
    }
    private void UpdateStatsUI()
    {
        _currencyTMP.text = _playerController.Data.Currency.ToString();

        /*_meleeSpeedBonusTMPro.text = "+ " + GetValueDifferencePercentage(_playerController.TimeToHitValue, _playerController.OriginalTimeToHitValue) + "%";*/
        _focusCostReductionTMPro.text = "- " + GetValueDifferencePercentage(_playerController.Focus.FocusCost, _playerController.Focus.OriginalFocusCost) + "%";
        _additionalHealthTMPro.text = "+ " + GetValueDifferencePercentage(_playerController.Data.MaxHealth, _playerController.Data.OriginalMaxHealth) + "%";
        _meleeDamageBonusTMPro.text = "+ " + GetValueDifferencePercentage(_playerController.Data.MeleeDamage, _playerController.Data.OriginalMeleeDamage) + "%";
        //_reloadTimeBonusTMPro.text = "- " + GetValueDifferencePercentage(_reloadTimeBonus, ) + "%";
        //_fireRateBonusTMPro.text = "+ " + GetValueDifferencePercentage(_fireRateBonus, ) + "%";
    }

    public bool UnlockComboSkill(int skillIndex)
    {
        PassiveSkill skill = comboSkills[skillIndex];
        if (!skill.isUnlocked && _playerController.Data.Currency >= skill.cost)
        {
            EventManager.InvokePayCurrency(skill.cost);

            skill.isUnlocked = true;
            _playerController.ApplySkill(skill.statToModify, skill.value);
            CheckAndUnlockMixedSkill(skillIndex, _playerController.Data.Currency);
            UpdateStatsUI();
            return true;
        }
        else
        {
            Debug.Log($"Couldn't resolve UnlockComboSkill, components:\b skillIndex: {skillIndex}, currency: {_playerController.Data.Currency}");
            return false;
        }
    }
    public bool UnlockSkill(SkillType pathType, int skillIndex)
    {
        SkillTreePath path = paths[pathType];
        PassiveSkill skill = path.Skills[skillIndex];

        if (!skill.isUnlocked && _playerController.Data.Currency >= skill.cost)
        {
            EventManager.InvokePayCurrency(skill.cost);

            skill.isUnlocked = true;
            _playerController.ApplySkill(skill.statToModify, skill.value);
            UpdateSkillButton(pathType, skillIndex);
            SetMainOrSecondaryPathIfNeeded(pathType);
            CheckAndUnlockMixedSkill(skillIndex, _playerController.Data.Currency);
            UpdateStatsUI();
            return true;
        }
        else
        {
            Debug.Log($"Couldn't resolve UnlockSkill, components:\b path: {pathType}, skillIndex: {skillIndex}, currency: {_playerController.Data.Currency}");
            return false;
        }
    }
    
    private void UpdateSkillButton(SkillType pathType, int skillIndex)
    {
        if ((pathType == mainPath && skillIndex >= _mainPathSkillThreshold) || (pathType == secondaryPath && skillIndex >= _secondaryPathSkillsThreshold))
            return;

        int nextSkillIndex = skillIndex + 1;

        if ((pathType == mainPath && nextSkillIndex >= _mainPathSkillThreshold) || (pathType == secondaryPath && nextSkillIndex >= _secondaryPathSkillsThreshold))
            return;

        if (nextSkillIndex < paths[pathType].Skills.Count)
        {
            paths[pathType].Skills[nextSkillIndex].isLocked = false;
            _pathsUI[(int)pathType].transform.GetChild(nextSkillIndex).GetComponent<Button>().interactable = true;
        }
    }
    private void SetMainOrSecondaryPathIfNeeded(SkillType pathType)
    {
        int unlockedSkillCount = paths[pathType].GetUnlockedSkillCount();

        if (unlockedSkillCount > 0)
            activePaths.Add(pathType);

        if (unlockedSkillCount >= _mainPathSkillThreshold && mainPath == SkillType.NoPath)
        {
            mainPath = pathType;

            foreach (Image bg in _treesBg)
                bg.color = _treeColors[(int)pathType];

            _isMainPathSet = true;
        }

        if (_isMainPathSet && !_isSecondaryPathSet)
        {
            UpdateSecondaryPath();
        } 

        if (activePaths.Count >= 2)
        {
            foreach (KeyValuePair<SkillType, SkillTreePath> kvp in paths)
            {
                SkillType currentPathType = kvp.Key;
                if (!activePaths.Contains(currentPathType))
                {
                    LockExtraPath(currentPathType);
                    break;
                }
            }
        }

        LockExtraPath();
    }
    private void UpdateSecondaryPath()
    {
        foreach (KeyValuePair<SkillType, SkillTreePath> kvp in paths)
        {
            SkillType pathType = kvp.Key;
            SkillTreePath path = kvp.Value;

            if (pathType != mainPath && path.GetUnlockedSkillCount() > 0)
            {
                secondaryPath = pathType;
                _isSecondaryPathSet = true;

                // lock extra skills in secondary path
                for (int i = _secondaryPathSkillsThreshold; i < paths[secondaryPath].Skills.Count; i++)
                {
                    _pathsUI[(int)secondaryPath].transform.GetChild(i).GetComponent<Button>().interactable = false;
                    paths[secondaryPath].Skills[i].isLocked = true;
                }
                break; // break after finding the secondary path
            }
        }
    }
    private void CheckAndUnlockMixedSkill(int skillIndex, int playerCurrency)
    {
        if (_isAlpha) return;

        bool allMainPathSkillsUnlocked = mainPath != SkillType.NoPath && AreAllSkillsUnlocked(paths[mainPath]);
        bool allSecondaryPathSkillsUnlocked = secondaryPath != SkillType.NoPath && AreAllSkillsUnlocked(paths[secondaryPath]);

        if (allMainPathSkillsUnlocked && allSecondaryPathSkillsUnlocked)
        {
            if (mainPath == SkillType.Speedster && secondaryPath == SkillType.Bruiser)
            {
                _comboSpeedBruiseBtn.interactable = true;
                Debug.Log("Unlocked Speedster/Bruiser");
            }
            else if (mainPath == SkillType.Speedster && secondaryPath == SkillType.Gunslinger)
            {
                _comboSpeedGunBtn.interactable = true;
                Debug.Log("Unlocked Speedster/Gunslinger");
            }
            else if (mainPath == SkillType.Bruiser && secondaryPath == SkillType.Speedster)
            {
                _comboBruiseSpeedBtn.interactable = true;
                Debug.Log("Unlocked Bruiser/Speedster");
            }
            else if (mainPath == SkillType.Bruiser && secondaryPath == SkillType.Gunslinger)
            {
                _comboBruiseGunBtn.interactable = true;
                Debug.Log("Unlocked Bruiser/Gunslinger");
            }
            else if (mainPath == SkillType.Gunslinger && secondaryPath == SkillType.Speedster)
            {
                _comboGunSpeedBtn.interactable = true;
                Debug.Log("Unlocked Gunslinger/Speedster");
            }
            else if (mainPath == SkillType.Gunslinger && secondaryPath == SkillType.Bruiser)
            {
                _comboGunBruiseBtn.interactable = true;
                Debug.Log("Unlocked Gunslinger/Bruiser");
            }
        }
    }
    private bool AreAllSkillsUnlocked(SkillTreePath path)
    {
        bool isMainPath = path.PathType == mainPath ? true : false;
        if (isMainPath)
        {
            for (int i = 0; i < _mainPathSkillThreshold +1; i++)
            {
                if (!path.Skills[i].isUnlocked)
                    return false;
            }
        }
        else
        {
            for (int i = 0; i < _secondaryPathSkillsThreshold; i++)
            {
                if (!path.Skills[i].isUnlocked)
                    return false;
            }
        }
        
        return true;
    }
    private void LockExtraPath(SkillType targetPathType)
    {
        if (!paths.TryGetValue(targetPathType, out SkillTreePath path))
            return;

        if (targetPathType != mainPath && targetPathType != secondaryPath)
        {
            foreach (PassiveSkill skill in path.Skills)
                skill.isLocked = true;

            _pathsUI[(int)targetPathType].SetActive(false);
        }
    }
    private void LockExtraPath()
    {
        if (mainPath == SkillType.NoPath || secondaryPath == SkillType.NoPath)
            return;

        foreach (KeyValuePair<SkillType, SkillTreePath> pair in paths)
        {
            SkillType pathType = pair.Key;
            SkillTreePath path = pair.Value;

            if (pathType != mainPath && pathType != secondaryPath)
            {
                foreach (PassiveSkill skill in path.Skills)
                    skill.isLocked = true;

                _pathsUI[(int)pathType].SetActive(false);
            }
        }
    }

    public void OnStartRun()
    {
        if (_shouldBeActiveOnStart)
            gameObject.SetActive(true);
    }
    public MonoBehaviour GetMonoBehaviour()
    {
        return this;
    }

    private void OnVendingMachineInteract(Player_Controller player)
    {
        _currencyTMP.text = player.Data.Currency.ToString();
        if (!gameObject.activeInHierarchy)
            OpenMenu(player);
        else
            CloseMenu();
    }

    [ContextMenu("Set Full Mode")]
    private void SetFullMode()
    {
        _mainPathSkillThreshold = 3;
        _secondaryPathSkillsThreshold = 2;
    }

    [ContextMenu("Set Demo Mode")]
    private void SetDemoMode()
    {
        _mainPathSkillThreshold = 1;
        _secondaryPathSkillsThreshold = 1;
    }
}