using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MusicNotePickup : MonoBehaviour
{
    public enum NoteState { pickable, channelable }

    [SerializeField] NoteState state;
    [SerializeField] GameObject _channelEffect;
    public float timerCount;
    float progress = 0;
    bool _canPress = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPress = true;
            Debug.Log("in trigger");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPress = false;

        }
    }

    private void Update()
    {
        if (_canPress)
        {
            if (Input.GetKey(KeyCode.E) && progress <= timerCount)
            {
                progress += Time.deltaTime;
                _channelEffect.SetActive(true);
                UIManager.Instance.BarProgress(progress / timerCount);
                UIManager.Instance._musicSlider.gameObject.SetActive(true);
            }
            else if (progress >= 0)
            {
                DecelerateBar();
            }
        }
        else if (progress >= 0 && _canPress == false)
        {
            if (progress == 0)
            {
                UIManager.Instance._musicSlider.gameObject.SetActive(false);
            }
            DecelerateBar();
        }

        if (progress / timerCount >= 1)
        {
            progress = 0;
            UIManager.Instance.BarProgress(0);
            UIManager.Instance._musicSlider.gameObject.SetActive(false);
            switch (state)
            {
                case NoteState.pickable:
                    GameManager.Instance.AddNotes(this);
                    break;
                case NoteState.channelable:
                    GameManager.Instance.RemoveNotes();
                    this.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void DecelerateBar()
    {
        progress -= Time.deltaTime;
        UIManager.Instance.BarProgress(progress / timerCount);
        _channelEffect.SetActive(false);
    }
}
