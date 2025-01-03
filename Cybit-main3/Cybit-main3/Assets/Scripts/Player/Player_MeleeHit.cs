using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MeleeHit : MonoBehaviour
{
    [SerializeField] private Player_Controller _controller;
    [SerializeField] private Player_Data _data;

    [SerializeField] private Collider2D _collider;
    public Collider2D MeleeCollider => _collider;

    [SerializeField] private float _knockbackPower = 3.0f;
    [SerializeField] private float _timeFreezeTime = 0.1f;
    [SerializeField] private bool _isFreezingTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && _collider.enabled && _collider.isActiveAndEnabled/* && _controller.IsMeleeInProgress*/)
        {
/*            EventManager.InvokeTimeFreeze(_timeFreezeTime);
*/
            Vector2 attackDirection = (other.transform.position - _controller.ArmPivot.position).normalized;

            if (other.TryGetComponent(out EnemeyAI enemyAI))
            {
                enemyAI.TakeDamage(attackDirection, _data.MeleeDamage, _knockbackPower);
                enemyAI.transform.Translate(attackDirection * _knockbackPower);
            }
            else if (other.TryGetComponent(out EnemyHenchman enemy))
            {
                enemy.TakeDamage(attackDirection, _data.MeleeDamage, _knockbackPower);
                enemy.transform.Translate(attackDirection * _knockbackPower);
            }
            else if (other.TryGetComponent(out GOBEnemy gobEnemy))
            {
                gobEnemy.TakeDamage(attackDirection, _data.MeleeDamage, _knockbackPower);
                gobEnemy.transform.Translate(attackDirection * _knockbackPower);
            }
            else if (other.TryGetComponent(out WardenHound_Minion jackalHound))
            {
                jackalHound.TakeDamage(attackDirection, _data.MeleeDamage, _knockbackPower);
                jackalHound.transform.Translate(attackDirection * _knockbackPower);
            }
            else if (other.TryGetComponent(out JackalWarden_AI jackalWarden))
            {
                jackalWarden.TakeDamage(attackDirection, _data.MeleeDamage, _knockbackPower);
                jackalWarden.transform.Translate(attackDirection * _knockbackPower);
            }
            else if (other.TryGetComponent(out Dummy dummy))
            {
                dummy.TakeDamage();
            }

        }

        if (other.gameObject.CompareTag("Canister") && _collider.enabled)
        {
            if (other.TryGetComponent(out HPCapsule_Interactable capsule) && !capsule._hasBeenShot && !capsule._isBroken)
            {
                capsule._hasBeenShot = true;
            }
            
        }

        //Debug.Log("Enemy slashed");
    }
}
