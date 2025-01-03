using Unity.VisualScripting;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public GameObject blockedDoor;
    public GameObject nextWaypoint;
    public Sprite checkMarkImage;
    public AudioClip checkMarkSound;
    public float destroyDelay = 2.0f;
    public float checkMarkHeight = 8.0f; // Height above the waypoint

    public void Start()
    {
        PlayerManager.Instance.AllPlayers[0].Controller.IsReady = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            if (blockedDoor != null)
            {
                Destroy(blockedDoor);
            }

            if (nextWaypoint != null)
            {
                nextWaypoint.SetActive(true);
            }

            SpawnCheckMarkAboveWaypoint();
        }
    }

    private void SpawnCheckMarkAboveWaypoint()
    {
        GameObject checkMarkObject = new GameObject("CheckMark");
        Vector3 spawnPosition = transform.position + Vector3.up * checkMarkHeight;
        checkMarkObject.transform.position = spawnPosition;

        SpriteRenderer spriteRenderer = checkMarkObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = checkMarkImage;

        AudioSource audioSource = checkMarkObject.AddComponent<AudioSource>();
        audioSource.clip = checkMarkSound;
        audioSource.Play();

        Destroy(checkMarkObject, destroyDelay);
    }
}
