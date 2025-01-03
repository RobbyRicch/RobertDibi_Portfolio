using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToPoof : MonoBehaviour
{
    public bool FromPlayer;
    public bool ToDestroy;
    private bool _initiate;
    public float SetTime;
    private float _timer;
    public bool IgnoreGround;
    public bool IgnoreEnemy;
    public bool IsBasicAttack;

    private BasicAttack _basicAttackRef;
    private void Awake()
    {
        _basicAttackRef = GetComponentInParent<BasicAttack>();
    }
    private void OnDisable()
    {
        _timer = 0;
        _initiate = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (_initiate)
        {
            _timer += Time.deltaTime;
            if (_timer >= SetTime)
            {
                if (ToDestroy)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _timer = 0;
                    _initiate = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void Initiate() { _initiate = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && FromPlayer && !IgnoreEnemy || other.CompareTag("Player") && !FromPlayer || other.CompareTag("Ground") && !IgnoreGround)
        {
            _timer = SetTime;
            if (IsBasicAttack)
            {
                GameObject feedback = Instantiate(_basicAttackRef._basicProjectileFeedback, transform.position + new Vector3(0,0,0.5f),Quaternion.identity);
                feedback.SetActive(true);
            }
        }
    }
}
