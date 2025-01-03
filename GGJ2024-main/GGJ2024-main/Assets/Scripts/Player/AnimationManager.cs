using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem PuppetHit;
    [SerializeField] private ParticleSystem PuppetStun;

    float _chrageJump;
    float _speed;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayRun(float speed)
    {
        _speed = speed;
        animator.SetFloat("Run", speed);

    }

    public void PlayJump(float charge)
    {
        _chrageJump = charge;
        animator.SetFloat("Charge", charge);
    }

    public void OnGrounded(bool isGround)
    {
        animator.SetBool("IsGrounded",isGround);
    }

    public void PlayTypeJump()
    {
        if(_chrageJump>15)
        {
            animator.Play("High Jump");
        }
        else
        {
            animator.Play("Short Jump");
        }
    }

    public void PlayHit()
    {
        animator.Play("Hit");
    }

    public void PlayDeath()
    {
        animator.Play("Death");
    }

    public void PlayHitParticle()
    {
        PuppetHit.Play();
    }

    public void PlayStunParticle()
    {
        PuppetStun.Play();
    }
}
