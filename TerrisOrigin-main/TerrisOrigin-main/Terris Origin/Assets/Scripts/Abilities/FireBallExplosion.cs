using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallExplosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem Explosion;
    public GameObject ExplosionArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Explosion.Play();
            ExplosionArea.SetActive(true);
        }
    }
}
