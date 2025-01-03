using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Music Notes")]
    public int scorePoint = 0;
    [SerializeField] List<GameObject> _notes;
    [SerializeField] public Slider _musicSlider;
    [Range(0, 1)] float barProgress;
    //[SerializeField] TextMeshProUGUI _musicNotesScore;

    [Header("HP")]
    [SerializeField] List<GameObject> _hearts;


    public static UIManager Instance;



    //private void Update()
    //{
    //    _musicNotesScore.text = $"{scorePoint}/3 MusicNotes";
    //}

    public void AddScore()
    {
        _notes[scorePoint].SetActive(true);
        scorePoint++;
    }

    public void RemoveHeart(int health)
    {
        int h = health-1;
        if (h >= 0)
        {
            _hearts[h].SetActive(false);
        }

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void BarProgress(float progress)
    {
        _musicSlider.value = progress;
    }
}
