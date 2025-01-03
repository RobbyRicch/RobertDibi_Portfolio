using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAura : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject AuraCollider;

    [SerializeField] private float TimeActive = 4f;
    [SerializeField] private float _coolDown = 6f;
    private float _cooldownTimer = 0;
    private bool startCoolDown = false;
    private void Start()
    {
        _cooldownTimer = _coolDown;
    }

    // Update is called once per frame
    void Update()
    {
        if (_cooldownTimer >= _coolDown && inputManager.FireAura)
        {
            inputManager.FireAura = false;
            AuraCollider.SetActive(true);
            startCoolDown = true;
            StartCoroutine(ActiveTime());
            SoundManager.Instance.PlaySound(SoundManager.SoundType.FireAura);

        }
        else
        {
            inputManager.FireAura = false;
            if (!startCoolDown)
                _cooldownTimer += Time.deltaTime;
        }
    }

    IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(TimeActive);
        AuraCollider.SetActive(false);
        startCoolDown = false;
        _cooldownTimer = 0;
    }
}
