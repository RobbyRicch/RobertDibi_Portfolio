using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulse : MonoBehaviour
{
    [SerializeField] private GameObject _impulseHitEffect;
    [SerializeField] private float _radius = 5f, _force = 1500f;
    private void Awake()
    {
        Explode();
    }
    private void Explode()
    {
        _impulseHitEffect.SetActive(true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        foreach (Collider nearByObject in colliders)
        {
            if (nearByObject.CompareTag("Puck"))
                continue;
            Rigidbody _rb = nearByObject.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                //_rb.AddExplosionForce(_force, transform.position, _radius);

                Vector3 dir = transform.position - nearByObject.transform.position;
                Vector3 dirNoY = new Vector3(dir.x, 0, dir.z);
                _rb.AddForce(-dirNoY.normalized * _force, ForceMode.Impulse);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
