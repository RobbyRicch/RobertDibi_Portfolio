using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TZ_ChangeScene : MonoBehaviour
{
    [Header("New Scene - *WRITE SCENE NAME*")]
    [SerializeField] private string _sceneToGoTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(_sceneToGoTo);
            //SceneManager.LoadScene(_sceneToGoTo);
        }
    }
}