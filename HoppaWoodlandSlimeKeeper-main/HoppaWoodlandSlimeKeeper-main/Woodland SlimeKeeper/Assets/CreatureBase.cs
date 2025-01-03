using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] public string _creatureName;
    [SerializeField] public int _EvoStage;
    [SerializeField] public int _slimesFed;
    [SerializeField] public bool _isHatched;

    [Header("Stage 1")]
    [SerializeField] public GameObject _stage1GO;
    [SerializeField] public Animator _stage1Animator;

    [Header("Stage 2")]
    [SerializeField] public GameObject _stage2GO;
    [SerializeField] public Animator _stage2Animator;

    [Header("Stage 3")]
    [SerializeField] public GameObject _stage3GO;
    [SerializeField] public Animator _stage3Animator;

    [Header("Components")]
    [SerializeField] public TextMeshProUGUI _WorldSpaceText;
    [SerializeField] public Animator _currentAnimator;
    [SerializeField] public Creature_Inventory _currentInventory;
    private void Awake()
    {
        _currentInventory = FindObjectOfType<Creature_Inventory>();
        _currentInventory._currentCreature = this;
    }
    private void OnEnable()
    {
       
    }

    private void Update()
    {
        if (_isHatched && _slimesFed < 3)
        {
            _stage1GO.SetActive(true);

            _stage2GO.SetActive(false);

            _stage3GO.SetActive(false);
            _currentAnimator = _stage1Animator;
            _EvoStage = 1;

        }

        if (_isHatched && _slimesFed >= 3 && _slimesFed < 6)
        {
            _stage1GO.SetActive(false);

            _stage2GO.SetActive(true);

            _stage3GO.SetActive(false);

            _EvoStage = 2;
            _currentAnimator = _stage2Animator;

        }

        if (_isHatched && _slimesFed >= 6 && _slimesFed < 9)
        {
            _stage1GO.SetActive(false);

            _stage2GO.SetActive(false);

            _stage3GO.SetActive(true);
            _EvoStage = 3;
            _currentAnimator = _stage3Animator;


        }
    }

   
}
