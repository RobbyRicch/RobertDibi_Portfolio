using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CoreLights_Manager : MonoBehaviour
{
    [Header("Light Refrences")]
    [SerializeField] private List<Light2D> _lights;
    [SerializeField] private List<Animator> _lightAnimators;

    [Header("States")]
    public bool _breathing;
    public bool _aggressive;
    public bool _weak;
    public bool _attacking;

    [Space(5)]
    [HorizontalLine]
    [Header("Alive/Dead")]
    [SerializeField] private bool _isAlive;

    [Header("CoreWarden_AI Reference")]
    [SerializeField] private CoreBoss_AI _bossScriptReference;


    private void Start()
    {
        _isAlive = true;

    }

    private void breathingState()
    {
        EnableOneState(_breathing, _aggressive, _weak, _attacking);
        foreach (Animator lightAnimator in _lightAnimators)
        {
            lightAnimator.SetBool("Breathing", true);
        }
    }

    //instead of writing 4 lines of disabeling and enabling just use this real quick ~ Robert
    private void EnableOneState(bool boolToEnable, bool disable1, bool disable2, bool disable3)
    {
        boolToEnable = true;
        disable1 = false;
        disable2 = false;
        disable3 = false;
    }
}
