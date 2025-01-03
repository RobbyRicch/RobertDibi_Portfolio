using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDestroy : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(DestroyCheck(_timeToDestroy));
    }

    private IEnumerator DestroyCheck(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
