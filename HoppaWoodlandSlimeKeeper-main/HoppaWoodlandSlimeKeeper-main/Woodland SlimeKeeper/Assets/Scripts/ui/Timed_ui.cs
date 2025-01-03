using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*show a gameobject for 2 seconds or other, at the end rise an action*/
public class Timed_ui : MonoBehaviour
{
    public GameObject showMe;
    public float showTimed=2f;

    private void Start()
    {
        showMe.SetActive(false);
    }
    public void ShowMe_UI()
    {
        showMe.SetActive(true);
        StartCoroutine(ExampleCoroutine());
       
    }

    IEnumerator ExampleCoroutine()
    {

        yield return new WaitForSeconds(showTimed);
        showMe.SetActive(false); // Disable the text so it is hidden
        //Actions.SetNewTask(true);
        Actions.Timed_UI_Done(true);
    }
}
