using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    public event EventHandler OnDeathOccured;
    public event EventHandler OnDamageOccured;

    public HealthSystem _healthSystem;

    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [Range(0, 2)]
    [SerializeField] private float _damageModifier = 0.5f;
    [SerializeField] private bool _useModifier = false;
    [SerializeField] private bool _canUseModifier = true;
    [SerializeField] private GameObject _buffEffect;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private bool isPlayer;
    [SerializeField] private EnemyAI EnemyAIRef;
    public Vector3 HitDirection { get; set; }

    private Damager tempDamagerOverTime;
    private float _timerDOT;
    private bool _isCollidingDOT;
    private bool _doDOT;
    [SerializeField] private float StackingWindow = 5f;
    [SerializeField] float Timer = 0;
    [SerializeField] AbilitiesSelection.ElementType firstType;


    private void Awake()
    {
        _healthSystem = new HealthSystem(_maxHP);
        if (_healthBar != null) _healthBar.Setup(_healthSystem);
        _healthSystem.OnDamaged += _healthSystem_OnDamaged;
        _healthSystem.OnDeath += _healthSystem_OnDeath;
        EnemyAIRef = GetComponentInParent<EnemyAI>();
        if (_buffEffect != null) _buffEffect.SetActive(false);
    }

    private void _healthSystem_OnDamaged(object sender, EventArgs e)
    {
        if (OnDamageOccured != null) OnDamageOccured(this, EventArgs.Empty);
    }

    private void _healthSystem_OnDeath(object sender, EventArgs e)
    {
        tempDamagerOverTime = null;
        _isCollidingDOT = false;
        if (OnDeathOccured != null) OnDeathOccured(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        _currentHP = _healthSystem.CurrentHealth;
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }

        DamageOverTime();
    }


    public void ToggleHealthBar(bool state)
    {
        if (_healthBar != null) _healthBar.gameObject.SetActive(state);
    }

    public void ToggleModifier(bool state)
    {
        if (_canUseModifier)
        {
            _useModifier = state;
        }

        if (_buffEffect != null) _buffEffect.SetActive(state);
    }

    private void TakeDamage(GameObject damagerObj)
    {
        // Get Damage Info from Damager GameObject
        Damager tempDamager = damagerObj.GetComponent<Damager>();

        if (gameObject.tag == "Enemy" && tempDamager.CanAffect == CanAffect.Enemy)
        {
            if (Timer > 0)
            {
                if (tempDamager.Element != firstType)
                {
                    //Need to update Damager to StackingElement Damager

                    Timer = 0;
                }
                else
                    Timer = StackingWindow;
            }
            else
            {
                Timer = StackingWindow;
                firstType = tempDamager.Element;
            }
        }
        else if (gameObject.tag == "Player" && tempDamager.CanAffect == CanAffect.Player)
        {
            RegisterIndicator(tempDamager.transform);
        }


        // Check if GameObject can be Affected by Damager
        if (gameObject.tag == "Player" && tempDamager.CanAffect == CanAffect.Player || gameObject.tag == "Enemy" && tempDamager.CanAffect == CanAffect.Enemy || tempDamager.CanAffect == CanAffect.Both)
        {
            // If Damager is One Hit
            if (tempDamager.DamagerType == DamagerType.OneHit)
            {
                if (_useModifier)
                {
                    _healthSystem.Damage((int)(tempDamager.DamageAmount * _damageModifier));
                }
                else
                {
                    _healthSystem.Damage(tempDamager.DamageAmount);
                }

                if (tempDamager.CanKnockback)
                {
                    Knockback(tempDamager);
                }
            }
            // If Damager is Over Time
            else if (tempDamager.DamagerType == DamagerType.OverTime)
            {
                tempDamagerOverTime = tempDamager;
                _doDOT = true;

            }
            // If Damager is Insta Death
            else if (tempDamager.DamagerType == DamagerType.InstaDeath)
            {
                _healthSystem.Damage(_maxHP);
            }
        }

    }

    private void OnHit(Transform t)
    {
        Damager tempDamager = t.GetComponent<Damager>();
        RaycastHit hit;
        if (Physics.Raycast(t.position, t.forward, out hit) && !isPlayer && tempDamager.CanAffect == CanAffect.Enemy)
        {
            GameObject paricles = Instantiate(particlePrefab, hit.point + (hit.normal * 0.05f), Quaternion.LookRotation(hit.normal), transform.parent);
            paricles.transform.localPosition = Vector3.zero;
            Destroy(paricles, 2f);
        }
    }

    private void DamageOverTime()
    {
        if (tempDamagerOverTime == null)
        {
            tempDamagerOverTime = null;
            _isCollidingDOT = false;
            _doDOT = false;
        }
        else
        {
            if (!tempDamagerOverTime.gameObject.activeInHierarchy)
            {
                tempDamagerOverTime = null;
                _isCollidingDOT = false;
                _doDOT = false;
            }
        }

        if (_doDOT && tempDamagerOverTime != null && _isCollidingDOT)
        {
            if (_timerDOT <= tempDamagerOverTime.DamageTimeInterval)
            {
                _timerDOT += Time.deltaTime;
            }
            else
            {
                if (gameObject.tag == "Player" && tempDamagerOverTime.CanAffect == CanAffect.Player)
                {
                    RegisterIndicator(tempDamagerOverTime.transform);
                }

                if (_useModifier)
                {
                    _healthSystem.Damage((int)(tempDamagerOverTime.DamageAmount * _damageModifier));
                }
                else
                {
                    _healthSystem.Damage(tempDamagerOverTime.DamageAmount);
                }
                _timerDOT = 0;
            }
        }
        else
        {
            _timerDOT = 0;
        }
    }


    //KnockBack
    // --------------------
    private void Knockback(Damager damager)
    {
        if (EnemyAIRef)
        {
            if (EnemyAIRef.IsKnockbackable)
            {
                //EnemyAIRef.Knockback(1, 1, transform.forward * -1);
                Vector3 knockBackDir = damager.transform.position - transform.position;
                knockBackDir.y = 0;
                knockBackDir = knockBackDir.normalized;
                EnemyAIRef.Knockback(damager.KnockbackStunTime, damager.KnockbackPower * EnemyAIRef.KnockbackForceModifier, knockBackDir);
            }
        }
    }


    // Collision Handling
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damager"))
        {
            if (gameObject.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<Damager>().UsedBy != null)
                {
                    other.gameObject.GetComponent<Damager>().UsedBy.TryGetComponent(out EnemyAI enemyai);
                    if (enemyai != null) enemyai.HitSuccess = true;
                }
            }
            TakeDamage(other.gameObject);
            OnHit(other.gameObject.transform);
            if (gameObject.CompareTag("Enemy"))
            {
                if (other.gameObject.GetComponent<Damager>().CanAffect == CanAffect.Enemy)
                {
                    HitDirection = other.transform.position - transform.position;
                    HitDirection = HitDirection.normalized;
                    //EnemyAIRef.Knocked = true;
                    //EnemyAIRef.KnockbackTimer = EnemyAIRef.KnockbackTime;
                }
            }
            //if (gameObject.layer == 7)
            //{
            //    HitDirection = other.transform.position - transform.position;
            //    HitDirection = HitDirection.normalized;
            //    EnemyAIRef.Knocked = true;
            //    EnemyAIRef.KnockbackTimer = EnemyAIRef.KnockbackTime;
            //}
            /*if (other.gameObject.layer == 11)
            {
                other.GetComponent<RicochetLightning>().GotHit = true;
            }*/
        }

        if (other.GetComponent<Damager>() == tempDamagerOverTime)
        {
            _isCollidingDOT = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Damager>() == tempDamagerOverTime)
        {
            tempDamagerOverTime = null;
            _isCollidingDOT = false;
            _doDOT = false;
        }
    }

    // Registering To Player Indicator
    private void RegisterIndicator(Transform transform)
    {
        //if (!DamageIndicatorSystem.CheckIfObjInSight(transform))
        //{
        DamageIndicatorSystem.CreateIndicator(transform);
        //}
    }
}
