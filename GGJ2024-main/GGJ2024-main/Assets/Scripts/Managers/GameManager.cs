using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Music Notes")]
    public List<MusicNotePickup> _musicNotesHandlers;
    [SerializeField] GameObject _channelPoints;

    [Header("HP Bar")]
    [SerializeField] public int health;

    [Header("Player Pos")]
    [SerializeField] Transform _startPos;
    [SerializeField] Transform _player;

    [Header("VFX")]
    [SerializeField] ParticleSystem _playerHit;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddNotes(MusicNotePickup musicNote)
    {
        if (_musicNotesHandlers.Contains(musicNote) == false)
        {
            _musicNotesHandlers.Add(musicNote);
            UIManager.Instance.AddScore();
            musicNote.gameObject.SetActive(false);
        }
        SendToBack();
    }

    public void RemoveNotes()
    {
        if (_musicNotesHandlers.Count > 0)
        {
            _musicNotesHandlers.Remove(_musicNotesHandlers[0]);
        }
        else
            CheckWin();
    }

    public void OnLoseHeart()
    {
        UIManager.Instance.RemoveHeart(health);
        _playerHit.Play();
        health--;

        if (health > 0)
        {
            AnimationManager.Instance.PlayHit();
        }
        else
        {
            health = 0;
            AnimationManager.Instance.PlayDeath();
            //lose ui
        }
    }

    void CheckWin()
    {

    }

    void SendToBack()
    {
        Debug.Log("Collect!!");
        _player.position = _startPos.position;
        UIManager.Instance._musicSlider.gameObject.SetActive(false);

        if (_musicNotesHandlers.Count == 3)
        {
            _channelPoints.SetActive(true);
        }
    }

}
