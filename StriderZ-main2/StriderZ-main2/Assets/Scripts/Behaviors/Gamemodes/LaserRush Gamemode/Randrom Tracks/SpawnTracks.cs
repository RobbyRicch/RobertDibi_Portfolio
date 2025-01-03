using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTracks : MonoBehaviour
{
    private int rand;
    private bool _isEndless = false;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerSetupManager.Instance == null)
            return;

        if (PlayerSetupManager.Instance.AllPlayersSetupData.Count == 1)
        {
            _isEndless = true;
        }
        
        RandomTracksTemplate.Instance.Tracks.Add(this.gameObject);
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                RandomTracksTemplate.Instance.NavPoints.Add(transform.GetChild(i));
            }
        }
        RandomTracksTemplate.Instance.NavPoints.Add(transform);
        //Invoke("Spawn", 0.05f);
        Spawn();
        if (!gameObject.CompareTag("SpawnArea"))
        {
            RandomTracksTemplate.Instance.PlacementPillars.Add(GetComponentInParent<TrackStartPointHolder>().EndPoint.gameObject);
        }
    }

    void Spawn()
    {
        if (RandomTracksTemplate.Instance.Tracks.Count <= RandomTracksTemplate.Instance.FirstTracksToSpawn)
        {
            rand = Random.Range(0, RandomTracksTemplate.Instance.FirstTracksList.Count);

            GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.FirstTracksList[rand], transform.position, transform.rotation);

            //float dis = Vector3.Distance(transform.position, newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position);
            float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
            float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
            float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
            newPrefab.transform.position += new Vector3(disX, disY, disZ);

            if (RandomTracksTemplate.Instance.UseRailing)
                newPrefab.GetComponent<TrackStartPointHolder>().Railing.SetActive(true);

            if (newPrefab.CompareTag("Connector"))
            {
                RandomTracksTemplate.Instance.ConnectorCounter++;
            }
            if (RandomTracksTemplate.Instance.ConnectorCounter == 2)
            {
                foreach (var track in RandomTracksTemplate.Instance.FirstTracksList)
                {
                    if (track.CompareTag("Connector"))
                    {
                        RandomTracksTemplate.Instance.FirstTracksList.Remove(track);
                    }
                }
                RandomTracksTemplate.Instance.ConnectorCounter = 0;
            }
        }
        else if (RandomTracksTemplate.Instance.Tracks.Count <= RandomTracksTemplate.Instance.MiddleTracksToSpawn + RandomTracksTemplate.Instance.FirstTracksToSpawn)
        {
            rand = Random.Range(0, RandomTracksTemplate.Instance.MiddleTracksList.Count);

            GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.MiddleTracksList[rand], transform.position, transform.rotation);

            //float dis = Vector3.Distance(transform.position, newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position);
            float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
            float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
            float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
            newPrefab.transform.position += new Vector3(disX, disY, disZ);

            if (RandomTracksTemplate.Instance.UseRailing)
                newPrefab.GetComponent<TrackStartPointHolder>().Railing.SetActive(true);

            if (newPrefab.CompareTag("Connector"))
            {
                RandomTracksTemplate.Instance.ConnectorCounter++;
            }
            if (RandomTracksTemplate.Instance.ConnectorCounter == 2)
            {
                foreach (var track in RandomTracksTemplate.Instance.MiddleTracksList)
                {
                    if (track.CompareTag("Connector"))
                    {
                        RandomTracksTemplate.Instance.MiddleTracksList.Remove(track);
                    }
                }
                RandomTracksTemplate.Instance.ConnectorCounter = 0;
            }
        }
        else if (RandomTracksTemplate.Instance.Tracks.Count <= RandomTracksTemplate.Instance.LastTracksToSpawn + RandomTracksTemplate.Instance.MiddleTracksToSpawn + RandomTracksTemplate.Instance.FirstTracksToSpawn)
        {
            rand = Random.Range(0, RandomTracksTemplate.Instance.LastTracksList.Count);

            GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.LastTracksList[rand], transform.position, transform.rotation);

            //float dis = Vector3.Distance(transform.position, newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position);
            float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
            float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
            float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
            newPrefab.transform.position += new Vector3(disX, disY, disZ);

            if (RandomTracksTemplate.Instance.UseRailing)
                newPrefab.GetComponent<TrackStartPointHolder>().Railing.SetActive(true);

            if (newPrefab.CompareTag("Connector"))
            {
                RandomTracksTemplate.Instance.ConnectorCounter++;
            }
            if (RandomTracksTemplate.Instance.ConnectorCounter == 2)
            {
                foreach (var track in RandomTracksTemplate.Instance.LastTracksList)
                {
                    if (track.CompareTag("Connector"))
                    {
                        RandomTracksTemplate.Instance.LastTracksList.Remove(track);
                    }
                }
                RandomTracksTemplate.Instance.ConnectorCounter = 0;
            }
        }
        else if (!RandomTracksTemplate.Instance.fork)
        {
            if (_isEndless)
            {
                GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.ForkEndless, transform.position, transform.rotation);
                float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
                float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
                float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
                newPrefab.transform.position += new Vector3(disX, disY, disZ);
                RandomTracksTemplate.Instance.fork = true;
                if (RandomTracksTemplate.Instance.UseRailing)
                    newPrefab.GetComponent<TrackStartPointHolder>().Railing.SetActive(true);
                StartCoroutine(waitForBake());
            }
            else
            {
                GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.Fork, transform.position, transform.rotation);
                float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
                float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
                float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
                newPrefab.transform.position += new Vector3(disX, disY, disZ);
                RandomTracksTemplate.Instance.fork = true;
                if (RandomTracksTemplate.Instance.UseRailing)
                    newPrefab.GetComponent<TrackStartPointHolder>().Railing.SetActive(true);
            }
        }
        else if(!_isEndless)
        {
            GameObject newPrefab = Instantiate(RandomTracksTemplate.Instance.Arena, transform.position, transform.rotation);
            float disZ = transform.position.z - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.z;
            float disX = transform.position.x - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.x;
            float disY = transform.position.y - newPrefab.GetComponent<TrackStartPointHolder>().StartPoint.position.y;
            newPrefab.transform.position += new Vector3(disX, disY, disZ);
            //RandomTracksTemplate.Instance.spawnBuildings.enabled = true;
            //PlayerManager.Instance.PlacementPoints();
            RandomTracksTemplate.Instance.FinishedGeneration = true;
            InitializeDolly();
            StartCoroutine(waitForBake());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerManager.Instance.MoveCurrentPlacementP();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
    IEnumerator waitForBake()
    {
        yield return new WaitForEndOfFrame();
        RandomTracksTemplate.Instance.navmeshSurface.BuildNavMesh();
    }

    private void InitializeDolly()
    {
        for (int i = 1; i < RandomTracksTemplate.Instance.Tracks.Count; i++)
        {
            RandomTracksTemplate.Instance.DollyLeftWaypoints.Add(RandomTracksTemplate.Instance.Tracks[i].GetComponentInParent<TrackStartPointHolder>().DollyWaypoints[0]);
            RandomTracksTemplate.Instance.DollyRightWaypoints.Add(RandomTracksTemplate.Instance.Tracks[i].GetComponentInParent<TrackStartPointHolder>().DollyWaypoints[1]);
        }
        RandomTracksTemplate.Instance.DollyRightWaypoints.Reverse();
    }
}
