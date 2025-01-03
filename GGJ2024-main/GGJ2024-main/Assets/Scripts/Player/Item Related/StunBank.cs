using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBank : MonoBehaviour
{
    [Header("Input")]
    public KeyCode Stun = KeyCode.E;

    [Header("Pickups")]
    public int availablePickups = 0;
    public int maxPickups = 2;

    [Header("Stun Behavior")]
    [SerializeField] float _stunTimer;
    [SerializeField] PuppetMasterController _puppetMaster;

    [Header("Blayer Sdan Ve ev EX")]
    [SerializeField] ParticleSystem _stunAbilityPS;

    private void Update()
    {
        if (Input.GetKey(Stun) && availablePickups > 0)
        {
            availablePickups--;
            _stunAbilityPS.Play();
            StartCoroutine(StunTimer());
        }
    }

    IEnumerator StunTimer()
    {
        _puppetMaster._stunned = true;
        _puppetMaster._stunPS.Play();

        float timer = _stunTimer;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _puppetMaster._stunned = false;
        _puppetMaster._stunPS.Stop();
    }
}
