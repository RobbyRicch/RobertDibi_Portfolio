using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MobEnemy : Enemy
{
    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {

        Instantiate(ResourcePrefab, Target.position, Quaternion.identity);
        Instantiate(DeathVFX, Target.position, Quaternion.identity);

    }
}
