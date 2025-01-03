using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpeedCurve : MonoBehaviour
{
    private Rigidbody rb;
    private float timeElapsed;
    private bool started = false;
    private Vector3 startPosition;
    //[SerializeField] private float speed = 40f;
    [SerializeField] private AnimationCurve speedCurve;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!started)
        {
            started = true;
            timeElapsed = 0;
            startPosition = transform.position;
            rb.velocity = Vector3.zero;
        }
        else
        {
            timeElapsed += Time.deltaTime;
            rb.velocity = transform.forward * speedCurve.Evaluate(timeElapsed);
            //Debug.Log(timeElapsed);
            if (timeElapsed >=0.2f)
            {
                transform.SetParent(null);
            }
        }
    }
    private void OnDisable()
    {
        started = false;
    }
}
