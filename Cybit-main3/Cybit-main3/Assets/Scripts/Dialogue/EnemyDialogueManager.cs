using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EnemyDialogueManager : MonoBehaviour
{
    [Header("In-Game UI Enemy References")]
    public TextMeshProUGUI _enemyNameTextRef;
    public TextMeshProUGUI _enemyDialogueTextRef;
    public TextMeshProUGUI _enemyWorldTextRef;
    public Image _enemyPortraitImgRef;
    public Animator _animatorRef;
    public AudioSource _audioSourceRef;

    [Header("Current Enemy Dialogue Handler")]
    private Queue<string> _sentenceQueue;
    private List<AudioClip> _voiceLines;
    private Dialogue _currentDialogue;
    public bool _isDialogueRunning;
    public bool _isWorldDialogue;
    public bool _isSkippable;

    [Header("Settings")]
    public float _timeAfterSentence = 1.0f; // Time to wait after typing each sentence before moving to the next
    public float _typingSpeed = 0.05f; // Delay between typing each character




    private void Awake()
    {
        _sentenceQueue = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting Convo");
        _enemyNameTextRef.text = dialogue._name;
        _enemyPortraitImgRef.sprite = dialogue._portrait;
        _sentenceQueue.Clear();
        _voiceLines = dialogue._voiceLines;
        _currentDialogue = dialogue;
        _isWorldDialogue = dialogue._isWorldDialogue;
        _isSkippable = dialogue._Skippable;
        _timeAfterSentence = dialogue._timeBetweenSenteces;

        /*if (!_isWorldDialogue)
            _animatorRef.SetBool("Enter", true);*/

        foreach (string sentence in dialogue._sentences)
        {
            _sentenceQueue.Enqueue(sentence);
        }
        _isDialogueRunning = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentenceQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentenceQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        int sentenceIndex = _currentDialogue._sentences.Length - _sentenceQueue.Count - 1;
        if (sentenceIndex < _voiceLines.Count && _voiceLines[sentenceIndex] != null)
        {
            _audioSourceRef.clip = _voiceLines[sentenceIndex];
            _audioSourceRef.Play();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        _enemyDialogueTextRef.text = "";
        if (_enemyWorldTextRef != null)
        {
            _enemyWorldTextRef.text = "";

        }

        if (!_isWorldDialogue)
        {
            foreach (char letter in sentence.ToCharArray())
            {
                _enemyDialogueTextRef.text += letter;
                yield return new WaitForSeconds(_typingSpeed); // Add delay here

            }

        }
        else
        {
            foreach (char letter in sentence.ToCharArray())
            {
                _enemyWorldTextRef.text += letter;
                yield return new WaitForSeconds(_typingSpeed); // Add delay here
            }
        }

        // Wait for a set time after the sentence is fully typed before moving to the next one
        yield return new WaitForSeconds(_timeAfterSentence);

        DisplayNextSentence();
    }

    public void EndDialogue()
    {
        Debug.Log("End Of Convo");


        _enemyWorldTextRef.text = "";



        _audioSourceRef.Stop();
        _isDialogueRunning = false;
    }

}
