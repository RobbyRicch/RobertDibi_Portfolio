using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPiece : MonoBehaviour
{
    [SerializeField] private int _timesToBreak = 3;
    int counter = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Puck"))
        {
            if(counter < _timesToBreak)
            {
                counter++;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
