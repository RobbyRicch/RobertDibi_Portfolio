using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEnemeis : MonoBehaviour
{
    // General Fields
    [SerializeField] private bool _applyBuff;
    //[SerializeField] private bool _fromWaveHolderData = true;
    private List<EnemyAI> _enemiesList = new List<EnemyAI>();

    //[Header("From Wave Holder")]
    //[SerializeField] private WaveHolder _waveHolder;
    //private List<GameObject> _currentSpawnedEnemies = new List<GameObject>();

    //[Header("From Set Enemy")]
    [SerializeField] private float _buffRange;
    [SerializeField] private LayerMask _enemiesLayer;
    [SerializeField] private Collider _enemyMainCollider;
    [SerializeField] private EnemyAI _bufferEnemyRef;


    // Start is called before the first frame update
    void Start()
    {
        if (_bufferEnemyRef != null)
        {
            if (_bufferEnemyRef.IsBuffer)
            {
                _applyBuff = true;
                _bufferEnemyRef.EnemyHealthHandlerGet.OnDeathOccured += EnemyHealthHandlerGet_OnDeathOccured;
            }
        }
    }

    private void EnemyHealthHandlerGet_OnDeathOccured(object sender, System.EventArgs e)
    {
        _applyBuff = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (_fromWaveHolderData)
        //{
        //    // Refresh List
        //    if (_waveHolder != null)
        //    {
        //        _currentSpawnedEnemies = _waveHolder.SpawnedEnemies;
        //        _enemiesList.Clear();
        //        foreach (var enemyObj in _currentSpawnedEnemies)
        //        {
        //            _enemiesList.Add(enemyObj.GetComponent<EnemyAI>());
        //        }
        //    }

        //    // Find Active Buffer Enemy
        //    bool found = false;
        //    foreach (var enemy in _enemiesList)
        //    {
        //        if (enemy.IsBuffer && enemy.gameObject.activeInHierarchy)
        //        {
        //            found = true;
        //        }
        //    }

        //    if (found)
        //    {
        //        _applyBuff = true;
        //    }
        //    else
        //    {
        //        _applyBuff = false;
        //    }
        //}
        //else
        //{
        // Refresh List
        Collider[] colliders = Physics.OverlapSphere(transform.position, _buffRange, _enemiesLayer);

        foreach (var item in colliders)
        {
            if (item != _enemyMainCollider)
            {
                if (item.transform.parent.TryGetComponent(out EnemyAI enemyAI))
                {
                    if (!_enemiesList.Contains(enemyAI))
                    {
                        _enemiesList.Add(enemyAI);
                    }
                }
            }
        }
        //}


        ApplyingBuff();
    }


    private void ApplyingBuff()
    {
        // Buff Status
        if (_applyBuff)
        {
            foreach (var enemyHealth in _enemiesList)
            {
                enemyHealth.EnemyHealthHandlerGet.ToggleModifier(true);
            }
        }
        else
        {
            foreach (var enemyHealth in _enemiesList)
            {
                enemyHealth.EnemyHealthHandlerGet.ToggleModifier(false);
            }
        }
    }
}
