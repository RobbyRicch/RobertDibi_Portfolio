using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public string[] _sentences;

    public string _name;
    public Sprite _portrait;
    public List<AudioClip> _voiceLines; // Voice lines should match the order of sentences
    public bool _isWorldDialogue;
    public bool _Skippable;
    public float _timeBetweenSenteces;
    public float _moveToNextDelay;

    // Add these fields
    public GameObject _nextDialogue;
    public bool _isPlayerSpeakingNext;
    public bool _isOtherSpeakingNext;
    public DialogueManager _dialogueManager;
}
