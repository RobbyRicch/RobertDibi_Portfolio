using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("Door Refs")]
    [SerializeField] private Animator animatorRef;

    [Header("Door States")]
    [SerializeField] private bool canBeOperatedByPlayer;
    [SerializeField] private bool canBeOperatedByEnemy;
    [SerializeField] private List<string> enemyTags;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeOperatedByPlayer)
        {
            animatorRef.SetBool("ShouldOpen", true);
            animatorRef.SetBool("ShouldClose", false);

        }
        if (IsEnemy(collision.gameObject.tag) && canBeOperatedByEnemy)
        {
            animatorRef.SetBool("ShouldOpen", true);
            animatorRef.SetBool("ShouldClose", false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeOperatedByPlayer)
        {
            animatorRef.SetBool("ShouldOpen", true);
            animatorRef.SetBool("ShouldClose", false);

        }
        if (IsEnemy(collision.gameObject.tag) && canBeOperatedByEnemy)
        {
            animatorRef.SetBool("ShouldOpen", true);
            animatorRef.SetBool("ShouldClose", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeOperatedByPlayer)
        {
            animatorRef.SetBool("ShouldOpen",false);
            animatorRef.SetBool("ShouldClose", true);
        }

        if (IsEnemy(collision.gameObject.tag) && canBeOperatedByEnemy)
        {
            animatorRef.SetBool("ShouldOpen", false);
            animatorRef.SetBool("ShouldClose", true);
        }
    }

    private bool IsEnemy(string tag)
    {
        return enemyTags.Contains(tag);
    }
}
