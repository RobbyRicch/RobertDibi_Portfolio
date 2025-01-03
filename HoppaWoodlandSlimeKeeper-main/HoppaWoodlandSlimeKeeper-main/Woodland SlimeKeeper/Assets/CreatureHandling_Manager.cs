using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHandling_Manager : MonoBehaviour
{

    [Header("The Current Creature On Display")]
    [SerializeField] public string _creatureName;
    [SerializeField] public int _currentEvoStage;


    [Header("Components")]
    [SerializeField] public Transform _displayTransform;
    [SerializeField] public GameObject _InventoryPanel;
    [SerializeField] public GameObject _currentCreatureGO;

    public void DisplayCreature()
    {
        if (_currentCreatureGO != null)
        {
            _currentCreatureGO.transform.position = _displayTransform.position;
        }
    }

    public void CopyCreatureStats()
    {

    }

}
