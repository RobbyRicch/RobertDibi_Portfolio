using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHenchmanData", menuName = "Scriptable Objects/Enemy Types/AI Henchman", order = 2)]
public class AI_SO_Henchman : AI_SO
{
    [SerializeField] private AudioClip _patrolingAC;
    public AudioClip PatrolingAC => _patrolingAC;

    [SerializeField] private AudioClip _attackAC;
    public AudioClip AttackAC => _attackAC;

    [SerializeField] private AudioClip _spotPlayerAC;
    public AudioClip SpotPlayerAC => _spotPlayerAC;
}
