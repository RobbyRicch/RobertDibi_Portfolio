using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Video;

public class OverlayEffectsManager : MonoBehaviour
{
    SceneChanger SceneChangerRef;

    [Header("Start Level Properties")]
    public VideoPlayer _levelStartEvent;
    public bool _shouldplayStartEvent;

    [Header("Player Lose Properties")]
    public VideoPlayer _loseEvent;

    [Header("Player Win Level Properties")]
    public VideoPlayer _levelCompleteEvent;



    private void Awake()
    {
        if (_shouldplayStartEvent)
        {
            _levelStartEvent.gameObject.SetActive(true);
            _levelStartEvent.Play();
            StartCoroutine(StartOverlayTimer());
        }
    }

    IEnumerator StartOverlayTimer()
    {
        yield return new WaitForSeconds(5.5f);
        _levelStartEvent.GetComponent("VideoPlayer").gameObject.SetActive(false);
    }

    public IEnumerator LoseStateOverlayTimer()
    {
        yield return new WaitForSeconds(2f);
        
        _loseEvent.GetComponent("VideoPlayer").gameObject.SetActive(false);
    }

}
