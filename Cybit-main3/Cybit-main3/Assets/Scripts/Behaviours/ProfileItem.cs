using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileItem : MonoBehaviour
{
    private const string _idText = "Save #";
    private const string _playTimeText = "Play Time: ";
    private const string _deathsText = "Deats: ";
    private const string _mapText = "Map: ";

    private string _idActualText;
    private string _playActualTimeText;
    private string _deathsActualText;
    private string _mapActualText;

    [Header("UI Handling")]
    [SerializeField] private GameObject _createProfilePanel;
    [SerializeField] private GameObject _characterNameOverlay;

    [SerializeField] private GameObject _selectProfilePanel;
    public GameObject SelectProfilePanel => _selectProfilePanel;

    [SerializeField] private Button _selectProfileBtn;
    public Button SelectProfileBtn => _selectProfileBtn;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _characterNameTMP;
    [SerializeField] private TextMeshProUGUI _idTMP;
    [SerializeField] private TextMeshProUGUI _playTimeTMP;
    [SerializeField] private TextMeshProUGUI _deathsTMP;
    [SerializeField] private TextMeshProUGUI _mapTMP;

    [Header("Images")]
    [SerializeField] private Image _pistolSprite;
    [SerializeField] private Image _knifeSprite;
    [SerializeField] private Image _shotgunSprite;
    [SerializeField] private Image _minigunSprite;
    [SerializeField] private Image _flameThrowerSprite;

    [Header("New Profiles")]
    [SerializeField] private TMP_InputField _enterNameField;

    private Profile _profileData;

    public void InitializeItem(Profile profileData/*, GunPrimary[] _tier1PrimaryPrefabs, GunSideArm[] _tier1SideArmPrefabs*/)
    {
        _profileData = profileData;
        Debug.Log("Initializing " + profileData.Name + "...");

        // combine strings
        _idActualText = _idText + profileData.Id.ToString();
        _playActualTimeText = _playTimeText + profileData.PlayTime.ToString();
        _deathsActualText = _deathsText + profileData.DeathCount.ToString();
        _mapActualText = _mapText + UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(profileData.MaxLevelReachedID).name;

        // assign text to tmp
        _characterNameTMP.text = profileData.Name;
        _characterNameOverlay.SetActive(true);
        Debug.Log("Activated " + _characterNameOverlay.name);

        _idTMP.text = _idActualText;
        _playTimeTMP.text = _playActualTimeText;
        _deathsTMP.text = _deathsActualText;
        _mapTMP.text = _mapActualText;

        // show weapons
        ShowWeaponSprites();
        _createProfilePanel.SetActive(false);
        Debug.Log("Deactivated " + _createProfilePanel.name);
    }
    public void InitializeItem()
    {
        _profileData = new(null, -1);

        // combine strings
        _idActualText = _idText + _profileData.Id.ToString();
        _playActualTimeText = _playTimeText + _profileData.PlayTime.ToString();
        _deathsActualText = _deathsText + _profileData.DeathCount.ToString();
        _mapActualText = _mapText + UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(_profileData.MaxLevelReachedID).name;

        // assign text to tmp
        _characterNameTMP.text = _profileData.Name;
        _characterNameOverlay.SetActive(false);
        Debug.Log("Activated " + _characterNameOverlay.name);

        _idTMP.text = _idActualText;
        _playTimeTMP.text = _playActualTimeText;
        _deathsTMP.text = _deathsActualText;
        _mapTMP.text = _mapActualText;

        // show weapons
        ShowWeaponSprites();
        _createProfilePanel.SetActive(true);
        _profileData = null;
        Debug.Log("Deactivated " + _createProfilePanel.name);
    }

    private void ShowWeaponSprites()
    {
        // need to add here every new weapon type
        Dictionary<string, Image> gunSpritesDict = new Dictionary<string, Image>
        {
            { "Pistol", _pistolSprite },
            { "Knife", _knifeSprite },
            { "Shotgun", _shotgunSprite },
            { "Minigun", _minigunSprite },
            { "FlameThrower", _flameThrowerSprite }
        };

        List<string> keys = gunSpritesDict.Keys.ToList();  // Convert the dictionary keys to an array
        for (int i = 0; i < keys.Count; i++)
        {
            if (_profileData.UnlockableGunIds.ContainsKey(keys[i]))
            {
                gunSpritesDict[keys[i]].color = Color.white;
            }
        }
    }

    public void ChangeProfile()
    {
        if (_profileData != null) SaveManager.Instance.ChangeProfile(_profileData.Name);
    }
    public void CreateProfile()
    {
        if (_enterNameField.text == "")
        {
            Debug.Log("Invalid Name");
            return;
        }
        else if (SaveManager.Instance.LoadedProfileNames.Any(name => name == _enterNameField.text))
        {
            Debug.Log($"Profile with name '{_enterNameField.text}' already exists.");
            _characterNameOverlay.SetActive(false);
            _createProfilePanel.SetActive(true);
            return;
        }
        else
        {
            Profile newProfile = SaveManager.Instance.CreatNewProfile(_enterNameField.text);
            InitializeItem(newProfile);
            _characterNameOverlay.SetActive(true);
            _createProfilePanel.SetActive(false);
        }
    }
    public void DeleteProfile()
    {
        SaveManager.Instance.DeleteProfile(_profileData.Name);
    }
}