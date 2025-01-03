using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("In-Game UI References")]
    public TextMeshProUGUI _nameTextRef;
    public TextMeshProUGUI _dialogueTextRef;
    public TextMeshProUGUI _worldTextRef;
    public Image _portraitImgRef;
    public Animator _animatorRef;
    public AudioSource _audioSourceRef;

    [Header("Current Dialogue Handler")]
    private Queue<string> _sentenceQueue;
    private List<AudioClip> _voiceLines;
    private Dialogue _currentDialogue;
    public bool _isDialogueRunning;
    public bool _isWorldDialogue;
    public bool _isSkippable;

    [Header("Settings")]
    public float _timeAfterSentence = 1.0f; // Time to wait after typing each sentence before moving to the next
    public float _typingSpeed = 0.05f; // Delay between typing each character
    public float _endConversationDelay = 2.0f; // Delay before ending conversation if no next dialogue
    public bool _isPlayerSpeakingNext;
    public bool _isOtherSpeakingNext;

    private void Awake()
    {
        _sentenceQueue = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting Convo");
        _nameTextRef.text = dialogue._name;
        _portraitImgRef.sprite = dialogue._portrait;
        _sentenceQueue.Clear();
        _voiceLines = dialogue._voiceLines;
        _currentDialogue = dialogue;
        _isWorldDialogue = dialogue._isWorldDialogue;
        _isSkippable = dialogue._Skippable;
        _timeAfterSentence = dialogue._timeBetweenSenteces;
        _isPlayerSpeakingNext = dialogue._isPlayerSpeakingNext;
        _isOtherSpeakingNext = dialogue._isOtherSpeakingNext;
        _endConversationDelay = dialogue._moveToNextDelay;
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
        _dialogueTextRef.text = "";
        if (_worldTextRef != null)
        {
            _worldTextRef.text = "";
        }

        if (!_isWorldDialogue)
        {
            foreach (char letter in sentence.ToCharArray())
            {
                _dialogueTextRef.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }
        }
        else
        {
            foreach (char letter in sentence.ToCharArray())
            {
                _worldTextRef.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }
        }

        // Wait for a set time after the sentence is fully typed before moving to the next one
        yield return new WaitForSeconds(_timeAfterSentence);

        if (_sentenceQueue.Count == 0)
        {
            // Check if there is a next dialogue
            if (_currentDialogue._nextDialogue != null)
            {

                  // Handle canvas changes based on who is speaking next
                    if (_isPlayerSpeakingNext)
                    {
                        ManualCanvasFade();
                    }
                    else if (_isOtherSpeakingNext)
                    {
                        ManualCanvasSpeak();
                    }

                    // Wait for a brief delay before starting the next dialogue
                    yield return new WaitForSeconds(_endConversationDelay);
                _currentDialogue._nextDialogue.SetActive(true);


            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            DisplayNextSentence();
        }
    }
    public void EndDialogue()
    {
        Debug.Log("End Of Convo");

        _worldTextRef.text = "";

        _audioSourceRef.Stop();
        _isDialogueRunning = false;
    }

    public void ManualCanvasEnter()
    {
        _animatorRef.SetBool("Enter", true);
        _animatorRef.SetBool("IsSpeaking", true);
    }

    public void ManualCanvasFade()
    {
        _animatorRef.SetBool("IsSpeaking", false);
    }

    public void ManualCanvasSpeak()
    {
        _animatorRef.SetBool("IsSpeaking", true);
    }

    public void ManualCanvasExitNormal()
    {
        _animatorRef.SetBool("IsSpeaking", true);
        _animatorRef.SetBool("Enter", false);
    }

    public void ManualCanvasExitFaded()
    {
        _animatorRef.SetBool("IsSpeaking", false);
        _animatorRef.SetBool("Enter", false);
    }
}