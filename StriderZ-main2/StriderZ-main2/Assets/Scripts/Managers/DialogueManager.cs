using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public RawImage portraitFill;
    public RawImage textBackFill;
    public TMP_Text dialogueText;
    public Image characterPortraitImage;
    public List<string> dialogueList = new List<string>();
    public List<Sprite> characterPortraits = new List<Sprite>();
    public List<AudioClip> dialogueSounds = new List<AudioClip>();
    public List<Color> fillColors = new List<Color>();
    public AudioSource audioSource;

    public float textSpeed = 20.0f;

    private int currentDialogueIndex = 0;
    private int currentTextIndex = 0;
    private Coroutine textAppearCoroutine;

    public bool shouldTurnOffAfterConversation = false;
    public int turnOffAfterDialogueIndex = -1;

    public bool activateObjectAfterChat = false;
    public GameObject objectToActivate;

    private bool isDialogueActive = true; // Control whether the dialogue manager is active or not

    private void Start()
    {
        ShowDialogue(0);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Return))
        {
            SkipToNextDialogue();
        }
    }

    public void ShowDialogue(int index)
    {
        if (isDialogueActive && index >= 0 && index < dialogueList.Count)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.PlayOneShot(dialogueSounds[index]);
            portraitFill.color = fillColors[index];
            textBackFill.color = fillColors[index];
            characterPortraitImage.sprite = characterPortraits[index];
            characterPortraitImage.gameObject.SetActive(true);
            dialogueText.text = "";
            currentDialogueIndex = index;
            currentTextIndex = 0;
            textAppearCoroutine = StartCoroutine(AppearText());
        }
        else
        {
            HideDialogue();
        }

        // Check if turning off is required
        if (shouldTurnOffAfterConversation && index == turnOffAfterDialogueIndex)
        {
            shouldTurnOffAfterConversation = false;
            gameObject.SetActive(false); // Deactivate the whole DialogueManager GameObject
        }
    }

    private IEnumerator AppearText()
    {
        while (currentTextIndex < dialogueList[currentDialogueIndex].Length)
        {
            dialogueText.text += dialogueList[currentDialogueIndex][currentTextIndex];
            currentTextIndex++;
            yield return new WaitForSeconds(1 / textSpeed);
        }

        // Check if activating an object is required after the chat ends
        if (activateObjectAfterChat && currentDialogueIndex == dialogueList.Count - 1)
        {
            objectToActivate.SetActive(true);
        }
    }

    public void SkipToNextDialogue()
    {
        if (textAppearCoroutine != null)
        {
            StopCoroutine(textAppearCoroutine);
        }

        dialogueText.text = dialogueList[currentDialogueIndex];

        // Check if the next dialogue will activate the object
        if (activateObjectAfterChat && currentDialogueIndex + 1 == dialogueList.Count)
        {
            objectToActivate.SetActive(true);
        }

        ShowDialogue(currentDialogueIndex + 1);
    }

    public void HideDialogue()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        characterPortraitImage.gameObject.SetActive(false);
        dialogueText.text = "";
        currentTextIndex = 0;
        if (textAppearCoroutine != null)
        {
            StopCoroutine(textAppearCoroutine);
        }
    }

    // Method to enable or disable the dialogue manager
    public void SetDialogueActive(bool isActive)
    {
        isDialogueActive = isActive;
    }
}
