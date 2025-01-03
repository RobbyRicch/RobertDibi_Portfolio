using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRandomizer : MonoBehaviour
{
    [SerializeField] private PickableStructProb[] Pickups;
    [SerializeField] private Transform[] PickupsTransforms;
    [SerializeField] private bool _isInArena = false;
    [SerializeField] private float _spawnCoolDown;
    private GameObject _currentPickup;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Spawn", 0.1f);
    }
    void Spawn()
    {
        foreach (var t in PickupsTransforms)
        {
            _currentPickup = Instantiate(ChoosePickupByChance(), t.position, t.rotation);
            PickableAbilty PA = _currentPickup.GetComponentInChildren<PickableAbilty>();
            PA.IsInArena = _isInArena;
            PA.pickupRandomizer = this;
        }
    }
    public IEnumerator Respawn(Transform t)
    {
        _currentPickup = null;
        Vector3 newPos = new Vector3(t.position.x,t.position.y,t.position.z);
        yield return new WaitForSeconds(_spawnCoolDown);
        newPos = new Vector3(newPos.x, PickupsTransforms[0].position.y, newPos.z);
        _currentPickup = Instantiate(ChoosePickupByChance(), newPos, Quaternion.identity);
        PickableAbilty PA = _currentPickup.GetComponentInChildren<PickableAbilty>();
        PA.IsInArena = _isInArena;
        PA.pickupRandomizer = this;
    }

    private GameObject ChoosePickupByChance()
    {
        float totalPercentage = 0f;
        List<float> cumulativePercentages = new List<float>();

        foreach (var pickup in Pickups)
        {
            totalPercentage += pickup.Probability;
            cumulativePercentages.Add(totalPercentage);
        }

        float randomValue = Random.Range(0f, totalPercentage);

        for (int i = 0; i < Pickups.Length; i++)
        {
            if (randomValue <= cumulativePercentages[i])
            {
                return Pickups[i].PickupObj;
            }
        }

        // This point should never be reached
        return null;
    }
}
