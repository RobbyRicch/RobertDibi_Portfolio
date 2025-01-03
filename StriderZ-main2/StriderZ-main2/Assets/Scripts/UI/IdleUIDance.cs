using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUIDance : MonoBehaviour
{
    [SerializeField] private float _timeBetweenDances = 3.0f;

    private float _timePassed = 0;

    private void Start()
    {
        StartCoroutine(Dance());
    }
    private void Update()
    {
        _timePassed += Time.deltaTime;
    }

    private IEnumerator Dance()
    {
        while (_timePassed < _timeBetweenDances)
            yield return null;

        yield return StartCoroutine(UIManager.Instance.PopUIObjectBehaviour(transform, 0.25f, UIManager.Instance.PopTargetSize, UIManager.Instance.PopPeakSize));
        yield return StartCoroutine(UIManager.Instance.UnPopUIObjectBehaviour(transform, 0.25f));
        _timePassed = 0;

        StartCoroutine(Dance());
    }
}
