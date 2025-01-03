using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private bool triggerAnimation;
    [SerializeField] private Animator animatorToTrigger;
    [SerializeField] private string triggerName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        triggerAnimation = true;
    }

    private void Update()
    {
        if (triggerAnimation)
        {
            animatorToTrigger.SetTrigger(triggerName);
        }
    }
}
