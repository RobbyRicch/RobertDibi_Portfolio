using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actions;

/*manage activating the correct canvas and disactivating the others*/
public class UI_Manager : MonoBehaviour
{//on UI_Manager gameobject
    [SerializeField]
    private GameObject StartCanvas;
    [SerializeField]
    private GameObject TasksCanvas;
    [SerializeField]
    private GameObject UIGameCanvas;
    [SerializeField]
    private GameObject UIOutofEnergy;
    [SerializeField]
    private GameObject UIShop;
    [SerializeField]
    private GameObject UIUser;
    [SerializeField]
    private GameObject UILevelUp;
    [SerializeField]
    private GameObject UICollection;
    [SerializeField]
    private GameObject UIDaily;
    [SerializeField]
    private GameObject UIBTNBackToMix;

    [SerializeField]
    private bool uiIsOpen;
    public Actions.stateGame stateGame;

    [SerializeField]
    private Tween_Grow tween;
    private void OnEnable()
    {
        Actions.Timed_UI_Done += TimedUIIsDone;
        Actions.LevelNumberUpdate+=LevelUpUI;
    }

    private void OnDisable()
    {
        Actions.Timed_UI_Done -= TimedUIIsDone;
        Actions.LevelNumberUpdate -= LevelUpUI;
    }

    void Start()
    {
        StartCanvas.SetActive(true);
        //TasksCanvas.SetActive(false);
        UIOutofEnergy.SetActive(false);
        UIShop.SetActive(false);
        UILevelUp.SetActive(false);
        UIUser.SetActive(false);
        UICollection.SetActive(false);
        UIDaily.SetActive(false);
        
        UIGameCanvas.SetActive(false);
        //UIBTNBackToMix.SetActive(false);
        uiIsOpen = true;
        stateGame = Actions.stateGame.Start;
    }
    public void BTNStart()
    {
        stateGame = Actions.stateGame.Tasks;
        uiIsOpen = false;
        BTNToMix();
    }
    public void BTNToMix()
    {
        StartCanvas.SetActive(false);
        TasksCanvas.SetActive(true);
        UIGameCanvas.SetActive(true);
       // UIBTNBackToMix.SetActive(false);
    }

    public bool IsUIOpen()
    {
        Debug.Log("isOpen:" + uiIsOpen);
        return uiIsOpen;
    }

    public void UITasksActive(bool _isActive)
    {
        TasksCanvas.SetActive(_isActive);
    }

    private void TimedUIIsDone(bool _isTimedUIDone)
    {
        if (_isTimedUIDone)
        {
            //UITasksActive(false);
            UIBTNBackToMix.SetActive(true);
        }

    }

    public void SetUIOnSlime()
    {
        stateGame = Actions.stateGame.Slime;
        //StartCanvas.SetActive(false);
        TasksCanvas.SetActive(false);
        //UIGameCanvas.SetActive(true);
       // UIBTNBackToMix.SetActive(true);
    }

    public void OpenUIOutOfEnergy()
    {
        UIOutofEnergy.SetActive(true);
        tween.SetTweenFXToGO_scale(UIOutofEnergy);
    }
    public void OpenUserUI()
    {
        uiIsOpen = true;
        UIUser.SetActive(true);
        tween.SetTweenFXToGO_scale(UIUser);
    }

    public void CloseUserUI()
    {
        uiIsOpen = false;
        UIUser.SetActive(false);
    }
    public void OpenShopUI()
    {
        uiIsOpen = true;
        UIShop.SetActive(true);
        tween.SetTweenFXToGO_scale(UIShop);
    }

    public void CloseShopUI()
    {
        uiIsOpen = false;
        UIShop.SetActive(false);
    }
    private void LevelUpUI(int plusLvl)
    {
        OpenLevelUpUI();
    }
    public void OpenLevelUpUI()
    {
        uiIsOpen = true;
        UILevelUp.SetActive(true);
        tween.SetTweenFXToGO_scale(UILevelUp);
    }

    public void CloseLevelUpUI()
    {
        uiIsOpen = false;
        UILevelUp.SetActive(false);
    }
    public void OpenCollectionUI()
    {
        uiIsOpen = true;
        UICollection.SetActive(true);
        tween.SetTweenFXToGO_scale(UICollection);
    }

    public void CloseCollectionUI()
    {
        uiIsOpen = false;
        //tween.SetTweenFXToGO_scale(UICollection);
        UICollection.SetActive(false);
    }
}
