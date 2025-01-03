using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Profiling;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance = null;
    public static SaveManager Instance => _instance;

    [Header("File Storage Config")]
    private FileDataHandler _dataHandler;
    private const string _saveFolderName = "SaveFiles";
    private const string _profilesFolderName = "Profiles";

    // game save
    private GameData _gameData;
    private List<IGameSaveable> _gameSaveables;
    [SerializeField] private string saveFileName = "Save";

    // profile save
    private Profile _activeProfile;
    private List<IProfileSaveable> _profileSaveables;
    private string _profilesFolderPath = string.Empty;
    private int _maxProfileAmount = 4;

    private List<string> _loadedProfileNames;
    public List<string> LoadedProfileNames => _loadedProfileNames;

    private List<Profile> _loadedProfiles;
    public List<Profile> LoadedProfiles => _loadedProfiles;

    [Header("Level Manager Config")]
    [SerializeField] private Player_Controller _player;
    public Player_Controller Player => _player;

    [SerializeField] private CustomSceneManager _customSceneManager; // to delete, jackal, death menu, link integrity
    public CustomSceneManager SceneCustomManager => _customSceneManager;

    [SerializeField] private SkillTreeManagerNew _skillManager; // to delete, trainingRoom ref
    public SkillTreeManagerNew SkillManager => _skillManager;

    private float _runStartTime;

    [Header("Player Config")]
    private Transform _startingTr;
    private Vector3 _startingPos;
    private bool _reloaded = false;

    [SerializeField] private GunPrimary[] _allPrimaryGunPrefabs;
    [SerializeField] private GunSideArm[] _allSideArmPrefabs;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Another instance of 'saveManager' exists");
            Destroy(this);
        }
        _instance = this;
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex < 3) return; // ignore if scene is before the actual levels like in main menu or training/tutorial.

        EventManager.OnStartRun += OnStartRun;
        EventManager.OnEndRun += OnEndRun;
    }
    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().buildIndex < 3) return;
        
        EventManager.OnStartRun -= OnStartRun;
        EventManager.OnEndRun -= OnEndRun;
    }
    private void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        // general save
        _dataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
        _gameSaveables = FindAllGameSaveableObjects();

        // profile
        _profileSaveables = FindAllProfileSaveableObjects();
        LoadGame();
    }
    private void OnApplicationQuit()
    {
        if (_activeProfile != null) SaveProfile();
        SaveGame();
    }

    #region General Save & Load
    public void NewGame()
    {
        _gameData = new GameData();
        _dataHandler.Save(_gameData);
        // open profile creation screen
    }
    public void LoadGame()
    {
        _gameData = _dataHandler.Load();

        if (_gameData == null)
        {
            Debug.Log("No data found");
            NewGame();
        }

        foreach (IGameSaveable dataPersistence in _gameSaveables)
        {
            dataPersistence.LoadData(_gameData);
        }

        _loadedProfileNames = _gameData.ProfileNames;
        LoadAllProfiles();
    }
    public void SaveGame()
    {
        foreach (IGameSaveable dataPersistence in _gameSaveables)
        {
            dataPersistence.SaveData(ref _gameData);
        }
        _dataHandler.Save(_gameData);
    }

    private List<IGameSaveable> FindAllGameSaveableObjects()
    {
        IEnumerable<IGameSaveable> dataPersistences = FindObjectsOfType<MonoBehaviour>().OfType<IGameSaveable>();
        return new List<IGameSaveable>(dataPersistences);
    }
    #endregion

    #region Profile Save & Load
    private string GetNameFromProfileName(string profileName)
    {
        int lastSlashIndex = profileName.LastIndexOfAny(new char[] { '\\', '/' }); // find the last backslash or forward slash
        string fileNameWithExtension = profileName.Substring(lastSlashIndex + 1); // extract the file name with extension
        int lastDotIndex = fileNameWithExtension.LastIndexOf('.'); // find the last dot in the file name
        string fileNameWithoutExtension = fileNameWithExtension.Substring(0, lastDotIndex); // extract the file name without extension

        Debug.Log("Extracted File Name: " + fileNameWithoutExtension);
        return fileNameWithoutExtension;
    }
    private List<IProfileSaveable> FindAllProfileSaveableObjects()
    {
        IEnumerable<IProfileSaveable> _profileSaveables = FindObjectsOfType<MonoBehaviour>().OfType<IProfileSaveable>();
        return new List<IProfileSaveable>(_profileSaveables);
    }

    public void LoadAllProfiles()
    {
        // assign folder path correctly
        _profilesFolderPath = Path.Combine(Application.persistentDataPath, _saveFolderName + @"\"+_profilesFolderName);

        if (!Directory.Exists(_profilesFolderPath)) Directory.CreateDirectory(_profilesFolderPath); // if folder do not exist create one

        List<string> profileFilePathsList = new();
        profileFilePathsList = Directory.GetFiles(_profilesFolderPath).ToList(); // get all file names in the folder

        _loadedProfiles = new();
        _loadedProfileNames = new();
        _gameData.ProfileNames = new();
        if (profileFilePathsList.Count < 1)
        {
            _activeProfile = new(null, -1);
            _gameData.ActiveProfileName = string.Empty;
            return; // if no files than stop loading
        }
        for (int i = 0; i < profileFilePathsList.Count; i++)
        {
            string profileName = Path.GetFileName(profileFilePathsList[i]);
            _loadedProfiles.Add(_dataHandler.LoadProfile(profileName));

            if (_gameData.ProfileNames.All(name => name != _loadedProfiles[i].Name))
            {
                _loadedProfileNames.Add(_loadedProfiles[i].Name);
                _gameData.ProfileNames.Add(_loadedProfileNames[i]);
            }
            Debug.Log("Path: " + profileFilePathsList[i] + "Name: " + profileName);
        }
        ActivateProfile();

        EventManager.InvokeProfilesLoaded(_loadedProfiles);
    }
    public void ActivateProfile()
    {
        if (_gameData.ActiveProfileName != string.Empty)
        {
            for (int i = 0; i < _loadedProfiles.Count; i++)
            {
                if (_loadedProfiles[i].Name == _gameData.ActiveProfileName)
                {
                    _activeProfile = _loadedProfiles[i];
                    break;
                }
                Debug.Log("Active profile from last game doesn't exist anymore."); // shouldn't reach here
            }

            foreach (IProfileSaveable profileDataObject in _profileSaveables)
            {
                profileDataObject.LoadData(_activeProfile);
            }
        }
    }
    public void SaveProfile()
    {
        if (_activeProfile == null || _activeProfile.Name == null || _activeProfile.Name == string.Empty)
        {
            _gameData.ActiveProfileName = _activeProfile.Name;
            _gameData.ProfileNames = _loadedProfileNames;
            return;
        }
        // pass the data  to other scripts so they can update it
        foreach (IProfileSaveable profileDataObject in _profileSaveables)
        {
            profileDataObject.SaveData(ref _activeProfile);
        }

        // save that data to a file using dataHandler with correct name
        _dataHandler.SaveProfile(_activeProfile, _activeProfile.Name);

        // reload profile
        _activeProfile = _dataHandler.LoadProfile(_activeProfile.Name + ".json");
        _gameData.ActiveProfileName = _activeProfile.Name;

        if (_gameData.ProfileNames.All(name => name != _activeProfile.Name))
        {
            _gameData.ProfileNames.Add(_activeProfile.Name);
        }
        if (_loadedProfileNames.All(name => name != _activeProfile.Name))
        {
            _loadedProfileNames.Add(_activeProfile.Name);
        }
        if (_loadedProfiles != null && _loadedProfiles.All(profile => profile.Name != _activeProfile.Name))
        {
            _loadedProfiles.Add(_activeProfile);
        }

        _runStartTime = _activeProfile.PlayTime;
        Debug.Log("Saved " + _activeProfile.Name + ".");
    }
    public Profile CreatNewProfile(string newProfileName)
    {
        if (_loadedProfiles != null && _loadedProfiles.Count >= _maxProfileAmount)
        {
            Debug.Log("Can't create any more profiles, delete one.");
            return null;
        }

        _activeProfile = new Profile(newProfileName, _loadedProfileNames.Count);
        SaveProfile();
        SaveGame();
        return _activeProfile;
    }
    public void ChangeProfile(string profileName)
    {
        for (int i = 0; i < _loadedProfiles.Count; i++)
        {
            if (_loadedProfiles[i].Name == profileName)
            {
                _activeProfile = _loadedProfiles[i];
                break;
            }
            Debug.Log("Desired profile doesn't exist."); // shouldn't reach here
        }

        foreach (IProfileSaveable profileDataObject in _profileSaveables)
        {
            profileDataObject.LoadData(_activeProfile);
        }
    }
    public void DeleteProfile(string profileName)
    {
        // Find the profile by name
        string filePath = Path.Combine(Application.persistentDataPath, _saveFolderName + @"\" + _profilesFolderName + @"\" + profileName + ".json");
        Profile profileToDelete = _loadedProfiles.Find(profile => profile.Name == profileName);

        if (File.Exists(filePath))
        {
            try
            {
                if (_loadedProfiles.Contains(profileToDelete)) _loadedProfiles.Remove(profileToDelete);
                else return;

                if (profileToDelete == _activeProfile)
                    _activeProfile = _loadedProfiles.Count > 0 ? _loadedProfiles[_loadedProfiles.Count - 1] : _activeProfile = new(null, -1);

                File.Delete(filePath);
                Debug.Log($"Profile file '{filePath}' deleted successfully.");

                _gameData.ActiveProfileName = _activeProfile.Name;
                _gameData.ProfileNames = _loadedProfileNames;

                LoadAllProfiles();
                Debug.Log($"Profile '{profileToDelete.Name}' removed successfully.");
            }
            catch (IOException e)
            {
                Debug.Log($"Failed to delete profile file: {filePath}. Error: {e.Message}");
            }
        }
        else
        {
            Debug.Log($"Profile file '{filePath}' not found.");
        }
    }
    #endregion

    public void InitializeLevelData(Transform startingTr)
    {
        _startingTr = startingTr;
        _startingPos = startingTr.position;
    }

    public IEnumerator InstantiateWeaponsWithDelay(Transform newPrimaryTr, Transform newSideArmTr)
    {
        if (!_player.PrimaryGun)
        {
            for (int i = 0; i < _allPrimaryGunPrefabs.Length; i++)
            {
                if (_allPrimaryGunPrefabs[i].Uid != "" && _allPrimaryGunPrefabs[i].Uid == _activeProfile.EquippedPrimaryId)
                {
                    GunPrimary primary = Instantiate(_allPrimaryGunPrefabs[i], newPrimaryTr.position, Quaternion.identity);
                    GunPickup gunPickup = primary.GetComponentInChildren<GunPickup>();
                    gunPickup.WorldTr = newPrimaryTr;
                    Debug.Log("Create correct primary");
                    break;
                }
            }
        }
        yield return new WaitForSeconds(0.35f);

        if (!_player.SideArm)
        {
            for (int i = 0; i < _allSideArmPrefabs.Length; i++)
            {
                if (_allSideArmPrefabs[i].Uid != "" && _allSideArmPrefabs[i].Uid == _activeProfile.EquippedSideArmId)
                {
                    GunSideArm sideArm = Instantiate(_allSideArmPrefabs[i], newSideArmTr.position, Quaternion.identity);
                    GunPickup gunPickup = sideArm.GetComponentInChildren<GunPickup>();
                    gunPickup.WorldTr = newSideArmTr;
                    Debug.Log("Create correct side arm");
                    break;
                }
            }
        }
    }

    [ContextMenu("Total Game State Reset")]
    public void TotalGameStateReset()
    {
        File.Delete(Path.Combine(Application.persistentDataPath + saveFileName));
        _gameData = new();
        Debug.Log("game data deleted");
    }
    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        _gameSaveables = FindAllGameSaveableObjects();
        _profileSaveables = FindAllProfileSaveableObjects();
        LoadGame();
    }

    private void OnStartRun()
    {
        _runStartTime = DaimnonTools.GetDateTimeAsFloat(DateTime.Now);
    }
    private void OnEndRun()
    {
        float endRunTimestamp = DaimnonTools.GetDateTimeAsFloat(DateTime.Now);
        float runDuration = endRunTimestamp - _runStartTime;

        _activeProfile.PlayTime += runDuration;
        _runStartTime = 0;
    }
}