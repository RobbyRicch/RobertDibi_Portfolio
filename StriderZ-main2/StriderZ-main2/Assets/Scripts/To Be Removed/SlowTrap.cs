using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrap : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const float _desiredYPos = 0.5f, _originYPos = 0.55f;

    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private MeshRenderer _slowTrapRenderer;
    [SerializeField] private Material _slowTrapOnMaterial, _slowTrapOffMaterial;
    [SerializeField] private float _slowTrapDistance, _slowTrapActivationDelay = 0.75f;

    private void OnTriggerEnter(Collider other)
    {
        // check if trigger is with player
        if (other.gameObject.CompareTag(_playerTag))
        {
            // set new pressure plate position
            transform.position = new Vector3(transform.position.x, _desiredYPos, transform.position.z);
            _slowTrapRenderer.material = _slowTrapOnMaterial;

            // start spawn coroutine
            StartCoroutine(SpawnSlowingObstacle(_slowTrapActivationDelay));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // check if trigger is with player
        if (other.gameObject.CompareTag(_playerTag))
        {
            // set original pressure plate position
            transform.position = new Vector3(transform.position.x, _originYPos, transform.position.z);
            _slowTrapRenderer.material = _slowTrapOffMaterial;
        }
    }

    private IEnumerator SpawnSlowingObstacle(float seconds)
    {
        Vector3 targetPos = new (transform.position.x + _slowTrapDistance, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(seconds);

        Debug.Log("Instansiating Spikes");
        Instantiate(_obstaclePrefab, targetPos, Quaternion.identity);
        StopCoroutine(SpawnSlowingObstacle(0));
    }
}
