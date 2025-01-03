using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float timeToChange;
    public bool shouldHaveDelay;
    public void GoToGameplay()
    {

        if (shouldHaveDelay)
        {
        StartCoroutine(GoToGameplaySceneWithDelay());

        }
        else
        {
            SceneManager.LoadScene("Game_Level_Scene");

        }
    }

    public void GoToMenu()
    {

        if (shouldHaveDelay)
        {
            StartCoroutine(GoToMenuSceneWithDelay());
        }
        else
        {
            SceneManager.LoadScene("Main_Menu");

        }
    }


    public IEnumerator GoToGameplaySceneWithDelay()
    {
        yield return new WaitForSeconds(timeToChange);
        SceneManager.LoadScene("RobbyTests_Scene");

    }

    public IEnumerator GoToMenuSceneWithDelay()
    {
        yield return new WaitForSeconds(timeToChange);
        SceneManager.LoadScene("Main_Menu");

    }
}
