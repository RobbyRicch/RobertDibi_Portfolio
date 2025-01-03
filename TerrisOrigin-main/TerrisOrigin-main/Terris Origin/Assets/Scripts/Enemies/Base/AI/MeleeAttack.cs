using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeComboState
{
    First = 1,
    Second,
    Third,
}

public class MeleeAttack : MonoBehaviour
{
    [System.Serializable]
    public class ComboTimer
    {
        [SerializeField] private float _activeStart;
        [SerializeField] private float _activeDuration;
        public int DamageAmount;

        public float ActiveStart { get => _activeStart; }
        public float ActiveDuration { get => _activeDuration; }
    }


    [SerializeField] private Collider _weaponCollider1;
    [SerializeField] private Collider _weaponCollider2;
    private Damager _weaponDamager1;
    private Damager _weaponDamager2;

    [SerializeField] private bool _comboTwoWeapons;
    [SerializeField] private int _comboLength = 1;
    public MeleeComboState ComboState = MeleeComboState.First;

    [SerializeField] List<ComboTimer> _comboTimers = new List<ComboTimer>();
    private float _timer;
    private bool _isActive;
    private bool _wasActive;
    private bool _isAttacking;

    private void Awake()
    {
        _weaponDamager1 = _weaponCollider1.GetComponent<Damager>();
        if (_weaponCollider2 != null) _weaponDamager2 = _weaponCollider2.GetComponent<Damager>();
    }

    // Update is called once per frame
    void Update()
    {
        AttackState();
    }

    public void DoAttack()
    {
        _isActive = true;
    }

    private void ToggleComboState()
    {
        if (((int)ComboState) < _comboLength)
        {
            ComboState++;
        }
        else if (((int)ComboState) >= _comboLength)
        {
            ComboState = MeleeComboState.First;
        }
    }

    public void SetSpecialState(MeleeComboState state)
    {
        ComboState = state;
    }

    private void AttackState()
    {
        if (_isActive)
        {
            if (_timer >= _comboTimers[(int)ComboState - 1].ActiveStart + _comboTimers[(int)ComboState - 1].ActiveDuration)
            {
                _isActive = false;
                _wasActive = true;
                _isAttacking = false;
            }
            else if (_timer >= _comboTimers[(int)ComboState - 1].ActiveStart)
            {
                _isAttacking = true;
                // While Attacking Is Active
                ComboHandler();
            }
            _timer += Time.deltaTime;
        }
        else
        {

            // Control Combo State
            if (_wasActive)
            {
                _wasActive = false;
                ToggleComboState();
            }

            // Reset Attack Values
            _timer = 0;
            _isAttacking = false;
            _weaponCollider1.enabled = _isAttacking;
            if (_weaponCollider2 != null) _weaponCollider2.enabled = _isAttacking;
        }
    }

    private void ComboHandler()
    {
        _weaponDamager1.DamageAmount = _comboTimers[(int)ComboState - 1].DamageAmount;
        if (_weaponCollider2 != null) _weaponDamager2.DamageAmount = _comboTimers[(int)ComboState - 1].DamageAmount;


        if (!_comboTwoWeapons)
        {
            _weaponCollider1.enabled = _isAttacking;
        }
        else
        {
            if (((int)ComboState) == 1)
            {
                _weaponCollider1.enabled = _isAttacking;
            }
            else if (((int)ComboState) == 2)
            {
                if (_weaponCollider2 != null) _weaponCollider2.enabled = _isAttacking;
            }
            else
            {
                _weaponCollider1.enabled = _isAttacking;
                if (_weaponCollider2 != null) _weaponCollider2.enabled = _isAttacking;
            }
        }
    }
}
