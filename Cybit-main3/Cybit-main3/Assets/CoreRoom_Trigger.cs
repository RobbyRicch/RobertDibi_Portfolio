using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreRoom_Trigger : MonoBehaviour
{
    [Header("Boss Refrences")]
    [SerializeField] private GameObject _staticBoss;
    [SerializeField] private GameObject _livingBoss;
    [SerializeField] private bool _shouldActivateBoss;
    [SerializeField] private float _timeToSwitchBossGO;
    [SerializeField] private Animator _stableCoreAnimator;
    [SerializeField] private BoxCollider2D _triggerBox;

    [Header("Dialogue GO's")]
    [SerializeField] private GameObject _annCoreEntryDialogueGO;

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(delayedBossActivation(_timeToSwitchBossGO));
            _staticBoss.SetActive(true);
            _livingBoss.SetActive(false);
            _triggerBox.enabled = false;
        }
    }

    private IEnumerator delayedBossActivation(float timeToChangeBoss)
    {
        _annCoreEntryDialogueGO.SetActive(true);
        yield return new WaitForSeconds(timeToChangeBoss);
        BossChange();
    }

    private void BossChange()
    {
        _staticBoss.SetActive(false);
        _livingBoss.SetActive(true);
    }
}
