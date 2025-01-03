using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertManager : MonoBehaviour
{
    private static AlertManager _instance;
    public static AlertManager Instance => _instance;

    [SerializeField] private int _countdownCount = 3;
    [SerializeField] private float _timeBetweenCounts = 0.9f;

    private IEnumerator _countdown;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        _countdown = Countdown();
    }

    public void PlayCountdownPopUp()
    {
        if (UIManager.Instance)
        {
            StartCoroutine(_countdown);
        }
    }
    private IEnumerator Countdown()
    {
        int count = _countdownCount;

        /* all this Time.timeScale < 1 is not a good solution */

        UIManager.Instance.SetCountdown(true);
        UIManager.Instance.ChangeCountdownSprite(count);
        yield return new WaitForSeconds(_timeBetweenCounts);

        count--;
        UIManager.Instance.ChangeCountdownSprite(count);
        yield return new WaitForSeconds(_timeBetweenCounts);

        count--;
        UIManager.Instance.ChangeCountdownSprite(count);

        // play gates animation

        yield return new WaitForSeconds(_timeBetweenCounts);

        count--;
        SoundManager.Instance.PlayAnnouncerSound(SoundManager.Instance.Go);
        UIManager.Instance.ChangeCountdownSprite(count);
        yield return new WaitForSeconds(_timeBetweenCounts);

        UIManager.Instance.SetCountdown(false);
        _countdown = null;
        _countdown = Countdown();

        EventManager.InvokeGameModeBegin();
        /*yield return new WaitForSeconds(Deathline.Instance.TimeToActivateFaterCountdown);

        Deathline.Instance.StartDeathline();*/
    }

    [SerializeField] private float _victoryPopUpTime = 2f, _winRoundPopUpTime = 2f;
    [SerializeField] private float _winRoundDelay = 1f, _winRoundAlertDuration = 1f;
    [SerializeField] private string _countDownAlertFinishText = "Go!";

    public IEnumerator PlayWinRoundAlert(int playerID)
    {
        yield return new WaitForSeconds(_winRoundDelay);

        //UIManager.Instance.WinRoundUIParent.SetActive(true);
        //UIManager.Instance.PlayersVictoryUI[playerID].enabled = true;
        yield return new WaitForSeconds(_winRoundAlertDuration);

        //UIManager.Instance.PlayersVictoryUI[playerID].enabled = false;
        //UIManager.Instance.WinRoundUIParent.SetActive(false);
    }
    public void PlayPopUpForSeconds(float seconds, string text)
    {
        //UIManager.Instance.PopUp.SetActive(true);
        //UIManager.Instance.PopUp.GetComponentInChildren<TextMeshProUGUI>().text = text;
        //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.PopUp, seconds));
    }
    public void PlayPopUpForSeconds(GameObject uiObject, float seconds)
    {
        //UIManager.Instance.PopUp.SetActive(true);
        uiObject.SetActive(true);
        //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.PopUp, seconds));
    }
    public void PlayCountPopUp(GameObject uiObject, RawImage alertImage, float seconds)
    {
        //UIManager.Instance.CountDownUIParent.SetActive(true);
        uiObject.SetActive(true);
        alertImage.enabled = true;
        //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.PopUp, seconds));
    }
    
    //public void PlayPopUpForSeconds(AlertTypes alertType, int playerID)
    //{
    //    switch (alertType)
    //    {
    //        case AlertTypes.Victory:
    //            //UIManager.Instance.VictoryUIParent.SetActive(true);
    //            //UIManager.Instance.WinRoundUIParent.SetActive(true);
    //            //UIManager.Instance.PlayersVictoryUI[playerID].enabled = true;
    //            //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.WinRoundUIParent, UIManager.Instance.PlayersVictoryUI[playerID], _victoryPopUpTime));
    //            //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.VictoryUIParent, _victoryPopUpTime));
    //            break;
    //        case AlertTypes.WinRound:
    //            //UIManager.Instance.WinRoundUIParent.SetActive(true);
    //            //UIManager.Instance.PlayersVictoryUI[playerID].enabled = true;
    //            //StartCoroutine(CloseAlertAfterSeconds(UIManager.Instance.WinRoundUIParent, UIManager.Instance.PlayersVictoryUI[playerID], _winRoundPopUpTime));
    //            break;
    //    }
    //}

    private IEnumerator CloseAlertAfterSeconds(GameObject alrtParent, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        alrtParent.SetActive(false);
    }
    private IEnumerator CloseAlertAfterSeconds(GameObject alrtParent, RawImage alertImage, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        alrtParent.SetActive(false);
        alertImage.enabled = false;
    }
    
}
