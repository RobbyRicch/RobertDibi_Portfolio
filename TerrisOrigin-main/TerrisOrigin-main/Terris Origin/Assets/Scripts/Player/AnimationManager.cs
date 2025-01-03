using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private InputManager inputManager;

    public bool CanBasic = false;
    public bool CanSecondaryBasic = false;
    public bool CanDashAnim = false;
    public bool CanWrathAnim = false;

    public bool BasicAttackLoaded = false;
    public bool SecondaryBasicAttackP1Loaded = false;
    public bool SecondaryBasicAttackP2Loaded = false;
    public bool LightningDashLoaded = false;
    public bool LightningWrathLoaded = false;
    public bool LightningWrathP1Loaded = false;
    public bool LightningWrathP2Loaded = false;

    [SerializeField] private GameObject Abilities;

    private LightningDash lightningDash;
    private LightningAOE LightningWrath;
    private float rnd;

    private void Awake()
    {
        LightningWrath = Abilities.GetComponent<LightningAOE>();
        lightningDash = Abilities.GetComponent<LightningDash>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanBasic)
            BasicAttackAnims();
        if (CanSecondaryBasic)
            SecondaryBasicAttackAnims();

        if (LightningWrath.enabled && CanWrathAnim)
        {
            LightningWrathAnims();
        }
        if (lightningDash.enabled && CanDashAnim)
        {
            LightningDashAnims();
        }
    }

    private void BasicAttackAnims()
    {
        if (inputManager.AttackPressed || inputManager.AttackHold)
        {
            animator.SetBool("Basic", true);
            /*StartCoroutine(RandomRecoil());
            StopCoroutine(RandomRecoil());*/
            animator.SetFloat("Recoil", 0);
        }
        else
        {
            animator.SetBool("Basic", false);
            BasicAttackLoaded = false;
            CanBasic = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                BasicAttackLoaded = true;
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Recoil"))
        {
            BasicAttackLoaded = true;
        }
    }
    private void SecondaryBasicAttackAnims()
    {
        if (inputManager.SecondaryBasicHold)
        {
            animator.SetBool("SecondaryBasic", true);

        }
        else if (inputManager.SecondaryBasicHoldCanceled)
        {
            animator.SetBool("SecondaryBasic", false);
            SecondaryBasicAttackP2Loaded = true;
        }
        else
        {
            SecondaryBasicAttackP1Loaded = false;
            SecondaryBasicAttackP2Loaded = false;
            CanSecondaryBasic = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ChargeAttack_P1"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.68f && !animator.IsInTransition(0))
            {
                SecondaryBasicAttackP1Loaded = true;
            }
        }
    }
    private void LightningWrathAnims()
    {
        if (inputManager.WrathHold)
        {
            animator.SetBool("WrathP1", true);
        }
        else if (inputManager.WrathHoldCanceled)
        {
            animator.SetBool("WrathP2", true);
        }
        else
        {
            animator.SetBool("WrathP1", false);
            animator.SetBool("WrathP2", false);
            LightningWrathP1Loaded = false;
            LightningWrathP2Loaded = false;
            CanWrathAnim = false;
        }

        /*if (animator.GetCurrentAnimatorStateInfo(1).IsName("Wrath"))
        {
            if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.65f && !animator.IsInTransition(1))
            {
                LightningWrathLoaded = true;
            }
        }*/
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("WrathP1"))
        {
            if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1 && !animator.IsInTransition(1))
            {
                LightningWrathP1Loaded = true;
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("WrathP2"))
        {
            if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.5f && !animator.IsInTransition(1))
            {
                LightningWrathP2Loaded = true;
                //LightningWrathLoaded = true;
            }
        }
    }

    private void LightningDashAnims()
    {
        if (inputManager.DashPress || inputManager.DashHold)
        {
            animator.SetBool("Dash", true);
        }
        else
        {
            animator.SetBool("Dash", false);
            LightningDashLoaded = true;
            CanDashAnim = false;
        }
    }

    IEnumerator RandomRecoil()
    {
        yield return new WaitForSeconds(1f);
        rnd = Random.Range(0f, 1f);
    }
}
