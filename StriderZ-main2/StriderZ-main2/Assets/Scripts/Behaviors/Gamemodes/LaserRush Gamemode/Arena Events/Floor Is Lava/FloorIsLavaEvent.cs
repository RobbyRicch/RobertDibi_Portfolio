using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorIsLavaEvent : MonoBehaviour
{
    public AnimationCurve scaleCurve; // Curve defining the scaling behavior
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private float _bounceForce = 20.0f, _dangerTime = 10.0f;
    [SerializeField] private int _maxDangerCount = 2;

    private float finalScale = 1;
    private Vector3 finalScale1 = Vector3.one;
    private float graphValue;

    private float startTime;
    private float timePassed;
    private void Start()
    {
        startTime = Time.time;
    }
    private void FixedUpdate()
    {
        graphValue = scaleCurve.Evaluate(timePassed);
        _sphereCollider.gameObject.transform.localScale = finalScale1 * graphValue;

        timePassed += Time.deltaTime;
    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            player.Controller.SetDangerTime();

            if (player.Data.DangerCounter > _maxDangerCount)
                player.Controller.IsAlive = false;
            else
            {
                Vector3 collisionNormal = other.transform.position - transform.position;
                collisionNormal.Normalize();
                //Vector3 oppositeDirection = -collisionNormal;
                player.Controller.Rb.AddForce(collisionNormal * _bounceForce, ForceMode.Impulse);
                player.Data.LavaVFX.Play();
            }
        }
    }*/
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();

            //if (player.Data.DangerCounter > _maxDangerCount)
            player.Controller.IsAlive = false;
            player.Data.LavaVFX.Play();
            /*else
            {
                Vector3 collisionNormal = other.transform.position - transform.position;
                collisionNormal.Normalize();
                //Vector3 oppositeDirection = -collisionNormal;
                player.Controller.Rb.AddForce(-collisionNormal * _bounceForce, ForceMode.Impulse);
                player.Data.LavaVFX.Play();
            }*/
        }
    }
}
