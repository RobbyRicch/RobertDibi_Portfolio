using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechaJaws : MonoBehaviour
{
    public PlayerInputHandler ThrowingPlayer { get; set; }
    [SerializeField] private GameObject Explosion;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator AttackAnim;
    [SerializeField] private SphereCollider SphereCollider;
    [SerializeField] private MechaJawsCollider JawsCollider;
    [SerializeField] private GameObject _mechJawsModel;
    public PlayerInputHandler LockedOnPlayer;
    private bool _attack = false;
    private bool _exloded = false;

    private void Start()
    {
        agent.enabled = true;
        LockedOnPlayer = LaserRushGameMode.Instance.PlayerPlacement[0];
        SoundManager.Instance.PlayMechaJaws(SoundManager.Instance.MechaJawsSound);

    }
    private void Update()
    {
        LockedOnPlayer = LaserRushGameMode.Instance.PlayerPlacement[0];
    }
    private void FixedUpdate()
    {
        if (LockedOnPlayer == null)
            return;
        float distanceFromPlayer = Vector3.Distance(transform.position, LockedOnPlayer.transform.position);
        if (distanceFromPlayer < 10)
        {
            _attack = true;
        }
        if (!_attack)
        {
            agent.SetDestination(LockedOnPlayer.transform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            AttackAnim.SetBool("Attack", true);
            if (!_exloded)
            {
                Explode();
            }
        }
        if (AttackAnim.GetCurrentAnimatorStateInfo(0).IsName("Anim_Mecha_Attack"))
        {
            if (AttackAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
            {
                SphereCollider.enabled = true;
            }
            if (AttackAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                SphereCollider.enabled = false;
                if (!JawsCollider.triggered)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public void Explode()
    {
        _exloded = true;
        GameObject exp = Instantiate(Explosion, transform.GetChild(0).position + (transform.GetChild(0).forward *10), Quaternion.identity);
        Destroy(exp, 2);
    }
}
