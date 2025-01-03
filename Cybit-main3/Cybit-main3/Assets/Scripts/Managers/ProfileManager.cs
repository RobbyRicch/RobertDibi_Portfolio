using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private ProfileItem[] _profileItems;
    private bool _isInitialized = false;
    //[SerializeField] private GunPrimary[] _tier1PrimaryPrefabs; // should be a sprite dictionary to easily handle weapons with same name on different tier
    //[SerializeField] private GunSideArm[] _tier1SideArmPrefabs; // should be a sprite dictionary to easily handle weapons with same name on different tier

    private void OnEnable()
    {
        EventManager.OnProfilesLoaded += OnProfilesLoaded;
    }
    private void OnDisable()
    {
        EventManager.OnProfilesLoaded -= OnProfilesLoaded;
    }

    private void LoadProfilesUI(List<Profile> profilesLoaded)
    {
        if (profilesLoaded == null || profilesLoaded.Count < 1) return;

        profilesLoaded.Sort((x, y) => x.Id.CompareTo(y.Id));
        for (int i = 0; i < _profileItems.Length; i++)
        {
            if (i < profilesLoaded.Count)
            {
                int profileIndex = i;
                _profileItems[i].InitializeItem(profilesLoaded[i]);
                _profileItems[i].SelectProfileBtn.onClick.RemoveAllListeners();
                _profileItems[i].SelectProfileBtn.onClick.AddListener(() => SelectProfile(_profileItems[profileIndex]));
                Debug.Log("Loaded profile " + _profileItems[i].name + ", " + i);
            }
            else
            {
                _profileItems[i].InitializeItem();
            }
        }

        if (!_isInitialized)
        {
            gameObject.SetActive(false);
            _isInitialized = true;
        }
    }
    public void SelectProfile(ProfileItem profileItem)
    {
        for (int i = 0; i < _profileItems.Length; i++)
        {
            GameObject selectProfilePanel = _profileItems[i].SelectProfilePanel;
            selectProfilePanel.SetActive(_profileItems[i] == profileItem);
            Debug.Log("Selected profile " + profileItem + ", " + i + ", " + _profileItems[i] + (_profileItems[i] == profileItem));
        }
    }

    private void OnProfilesLoaded(List<Profile> profilesLoaded)
    {
        LoadProfilesUI(profilesLoaded);
    }
}
