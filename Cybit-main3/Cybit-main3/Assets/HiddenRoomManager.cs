using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenRoomManager : MonoBehaviour
{
    [Header("States")]
    //[SerializeField] private bool IsHidden;
    //[SerializeField] private bool shouldUnlock;
    [SerializeField] private bool shouldHideOnExit;

    [Header("Hidden/Revealed")]
    [SerializeField] private List<SpriteRenderer> ObjectsToHideOrReveal;


    [Header("Color Transition")]
    [SerializeField] private float colorTransitionDuration = 1f; // Duration of color transition in seconds
    [SerializeField] private Color targetColor;


    private bool isTransitioning = false;
    private void Start()
    {
        //IsHidden = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(ChangeColorSmoothly(targetColor, colorTransitionDuration));
        }
    }

    private IEnumerator ChangeColorSmoothly(Color targetColor, float duration)
    {
        //IsHidden = false;
        isTransitioning = true;

        Color initialColor = ObjectsToHideOrReveal[0].color; // Assuming all objects have the same initial color

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            for (int i = 0; i < ObjectsToHideOrReveal.Count; i++)
            {
                ObjectsToHideOrReveal[i].color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the target color is set exactly at the end
        for (int i = 0; i < ObjectsToHideOrReveal.Count; i++)
        {
            ObjectsToHideOrReveal[i].color = targetColor;
        }

        isTransitioning = false;
    }
}
