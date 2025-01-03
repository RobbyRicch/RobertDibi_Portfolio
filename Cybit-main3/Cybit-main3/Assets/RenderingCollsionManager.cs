using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderingCollsionManager : MonoBehaviour
{
    [Header("BoxColliders To Switch Between")]
    [SerializeField] private BoxCollider2D OutsideCollider;
    [SerializeField] private BoxCollider2D InsideCollider;

    [Header("SpriteRenderers Order To Change To")]
    [SerializeField] List<SpriteRenderer> renderers;
    [SerializeField] private bool shouldChangeSpriteOrder;
    [SerializeField] private int targerLayer;
    [SerializeField] private int originalLayer;

    [Header("is Player Inside/Outside")]
    [SerializeField] private bool isInside;

    [Header("is Enemy Inside/Outside")]
    [SerializeField] private bool enemyInside;
    [SerializeField] private List<string> enemyTags;
    private int originalSortingOrder;
    [SerializeField] int newSortingOrder;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerCollisionChanger"))
        {
            isInside = true;
        }

        if (IsEnemy(collision.gameObject.tag))
        {
            enemyInside = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInside)
        {
            InsideCollider.enabled = true;
            OutsideCollider.enabled = false;

            if (shouldChangeSpriteOrder)
            {
                foreach (SpriteRenderer renderer in renderers)
                {

                    renderer.sortingOrder = targerLayer;


                }
            }
        }

        if (enemyInside && IsEnemy(collision.gameObject.tag))
        {
            originalSortingOrder = collision.GetComponent<SpriteRenderer>().sortingOrder;
            collision.GetComponent<SpriteRenderer>().sortingOrder = newSortingOrder;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerCollisionChanger"))
        {

            isInside = false;

            InsideCollider.enabled = false;
            OutsideCollider.enabled = true;
            if (shouldChangeSpriteOrder)
            {
                foreach (SpriteRenderer renderer in renderers)
                {

                    renderer.sortingOrder = originalLayer;


                }
            }


        }

        if (IsEnemy(collision.gameObject.tag))
        {
            enemyInside = false;
            collision.GetComponent<SpriteRenderer>().sortingOrder = originalSortingOrder;

        }

    }

    private bool IsEnemy(string tag)
    {
        return enemyTags.Contains(tag);
    }

}