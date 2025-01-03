using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuBtn;
    [SerializeField] private Animator _camReposition;
    [SerializeField] private TextMeshPro _inputText;
    [SerializeField] private AudioSource _bgMusic;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(_menuBtn);
    }

    private IEnumerator PlayCam()
    {
        _inputText.gameObject.SetActive(false);
        _bgMusic.Stop();
        _camReposition.SetTrigger("TransitionCam");
        yield return new WaitForSeconds(2.5f);
        CustomSceneManager.ChangeScene(1);
    }

    public void EnterGame()
    {
        StartCoroutine(PlayCam());
    }
}
