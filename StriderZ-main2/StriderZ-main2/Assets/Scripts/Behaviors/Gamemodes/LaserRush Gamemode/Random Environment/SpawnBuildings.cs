using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuildings : MonoBehaviour
{
    private static SpawnBuildings _instance;
    public static SpawnBuildings Instance => _instance;

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private LayerMask ObsticleLayer;
    Vector3 Edge;
    Vector3 Offset;
    int rand;
    GameObject newBlock;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        envManager = EnvironmentManager.Instance;
        Edge.x = boxCollider.size.x;
        Edge.z = boxCollider.size.z;
        Offset = new Vector3(-Edge.x / 2, 0, -Edge.z / 2);
    }
    // Update is called once per frame
    void Update()
    {
        if (CheckIfUnderTrack())
        {
            rand = Random.Range(0, envManager.SmallBlocksList.Length);
            newBlock = Instantiate(envManager.SmallBlocksList[rand], transform.position + Offset, envManager.SmallBlocksList[rand].transform.rotation);
        }
        else
        {
            rand = Random.Range(0, envManager.BlocksList.Length);
            newBlock = Instantiate(envManager.BlocksList[rand], transform.position + Offset, envManager.BlocksList[rand].transform.rotation);
        }

        if (Offset.x <= (Edge.x / 2))
        {
            if (Offset.z < (Edge.z / 2))
            {
                Offset.z += 400;
            }
            else
            {
                Offset.x += 200;
                Offset.z = -Edge.z / 2;
            }
        }
        else
        {
            Destroy(newBlock);
            enabled = false;
        }
        envManager.Blocks.Add(newBlock);

    }

    bool CheckIfUnderTrack()
    {
        if (Physics.CheckBox(transform.position + Offset, new Vector3(100, 100, 200),Quaternion.identity,ObsticleLayer))
        {
            Debug.Log("something in it");
            return true;
        }
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Offset, new Vector3(200, 200, 400));
    }
}
