using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemeyAI : MonoBehaviour
{
    [Header("Stats // General HP/CD")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;
    [SerializeField] private float attackCooldown;
    private bool _isHaltingAgent = false;
    [SerializeField] float _knockBackTime = 0.15f;
    private bool _isKnockedBack = false;

    [Header("Ranged Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float bulletProjectileSpeed;
    private bool canShoot = true;

    [Header("Charged Attack")]
    [SerializeField] private float chargedShot;
    [SerializeField] private float chargedProjectileSpeed;
    [SerializeField] private float chargedCooldown;
    [SerializeField] private float timeForChargedRelease;
    [SerializeField] private bool shouldFlash;
    [SerializeField] private bool isCharged;
    [SerializeField] private bool canCharge;
    [SerializeField] private Color chargeFlashColor = Color.white;
    private float chargedShotCounter;
    private bool hasfiredCharged = false;

    [Header("States")]
    [SerializeField] private bool isChasing;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isRanged;
    [SerializeField] private bool isRetreating;
    private bool isCooldown = false;
    private bool isCharging = false;

    [Header("Attacks/Bullets")]
    [SerializeField] GameObject meleeAttack;
    [SerializeField] EnemyProjectileBullet rangedAttackGO;
    [SerializeField] GameObject chargedAttackGO;

    [Header("Refrences")]
    [SerializeField] GameObject playerObject;
    [SerializeField] Animator animatorRef;
    [SerializeField] Animator weaponAnimRef;
    [SerializeField] Transform armPivot;
    [SerializeField] Transform firePoint;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] private TaskBase _task;
    public TaskBase Task { get => _task; set => _task = value; }


    [Header("Drops")]
    [SerializeField] GameObject hpDrop;
    [SerializeField] float focusAmountDrop;


    [Header("DMG Flash")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private bool isFlashing = false;

    [Header("Flip")]
    [SerializeField] private bool isFacingLeft;
    [SerializeField] private bool isFacingRight;
    private Color originalColor;
    private NavMeshAgent navMeshAgent;

    private bool _isAlive = true;

    private void Start()
    {
        currentHealth = maxHealth;
        originalColor = spriteRenderers[0].color;
        isChasing = true;
        isAttacking = false;
        playerObject = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();

        animatorRef.SetBool("IsFacingRight", isFacingRight);
        animatorRef.SetBool("IsFacingLeft", isFacingLeft);
        // Constrain rotation
        transform.rotation = Quaternion.identity;

        if (isRanged)
        {
            Flip();
        }
    }

    private void Update()
    {
        if (playerObject != null)
        {
            if (!isRetreating)
            {
                ChaseState();

            }
            if (!isRanged)
            {
                AttackState();

            }
            if (isRanged)
            {
                AimAtPlayer();
                float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
                if (distanceToPlayer <= navMeshAgent.stoppingDistance)
                {
                    isAttacking = true;

                }
                AimWeapon();
                if (isAttacking && !isCharged)
                {
                    RangedAttack();
                }
                if (isCharged && !hasfiredCharged)
                {
                    StartChargedRangedAttack();
                }
                if (hasfiredCharged)
                {
                    isCharged = false;
                    StartCoroutine(ResetChargeShot());
                }
            }

            FlipIfNeeded();
        }
        else
        {
            navMeshAgent.enabled = false;
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void AimAtPlayer()
    {
        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        if (directionToPlayer.x < 0 && !isFacingLeft)
        {
            Flip();
            animatorRef.SetBool("IsFacingLeft", true);
            animatorRef.SetBool("IsFacingRight", false);

        }
        else if (directionToPlayer.x > 0 && !isFacingRight)
        {
            Flip();
            animatorRef.SetBool("IsFacingRight", true);
            animatorRef.SetBool("IsFacingLeft", false);

        }
    }

    private void ChaseState()
    {


        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > navMeshAgent.stoppingDistance)
        {
            isChasing = true;
            navMeshAgent.SetDestination(playerObject.transform.position);

            if (!isRanged)
            {
                animatorRef.SetBool("IsMoving", true);
            }
        }
        else
        {
            isChasing = false;
            navMeshAgent.SetDestination(transform.position);

            if (!isRanged)
            {
                animatorRef.SetBool("IsMoving", false);
            }
        }

        // Fix rotation to prevent it from being forced to -90 on the X-axis
        transform.rotation = Quaternion.Euler(Vector3.zero);

    }

    private void AttackState()
    {
        if (!isCooldown)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

            if (distanceToPlayer <= navMeshAgent.stoppingDistance)
            {
                isAttacking = true;
                animatorRef.SetTrigger("Attack");
                StartCoroutine(MeleeAttack());
                StartCoroutine(AttackCooldown(attackCooldown));
            }
            else
            {
                isAttacking = false;
            }
        }
    }
    private void RangedAttack()
    {
        if (!isCooldown && !isCharged && canShoot)
        {

            weaponAnimRef.SetTrigger("Attack");
            Vector2 direction = firePoint.right;

            EnemyProjectileBullet firedBullet = Instantiate(rangedAttackGO, firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            firedBullet.SetStats(damage, bulletProjectileSpeed, direction);

            StartCoroutine(AttackCooldown(attackCooldown));
            if (canCharge)
                chargedShotCounter++;
        }
        if (chargedShotCounter == chargedShot)
        {
            isCharged = true;
            canShoot = false;
        }
        else
        {
            isCharged = false;
        }
    }

    private void StartChargedRangedAttack()
    {
        if (!isCharging)
        {
            isCharging = true;
            StartCoroutine(DelayedChargedRangedAttack());
        }
    }

    private IEnumerator DelayedChargedRangedAttack()
    {
        if (!hasfiredCharged)
        {
            isCharged = false;
            weaponAnimRef.SetTrigger("ChargedAttack");
            yield return new WaitForSeconds(timeForChargedRelease);
            GameObject firedBullet = Instantiate(chargedAttackGO, firePoint.position, Quaternion.identity);
            Vector2 direction = firePoint.right;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Rigidbody2D bulletRb = firedBullet.GetComponent<Rigidbody2D>();
            float bulletSpeed = chargedProjectileSpeed;
            bulletRb.velocity = direction * bulletSpeed;
            Destroy(firedBullet, 1.25f);
            chargedShotCounter = 0;
            canCharge = true;
            hasfiredCharged = true;
            canShoot = true;
        }


    }

    private IEnumerator ResetChargeShot()
    {
        hasfiredCharged = true;
        yield return new WaitForSeconds(1);
        hasfiredCharged = false;
        isCharging = false;
    }
    private void AimWeapon()
    {
        Vector3 armPosition = playerObject.transform.position;
        Vector3 direction = armPosition - armPivot.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the rotation only on the Z-axis
        armPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private IEnumerator AttackCooldown(float cd)
    {
        isCooldown = true;
        yield return new WaitForSeconds(cd);
        isCooldown = false;
    }

    private void FlipIfNeeded()
    {
        if (isChasing)
        {
            Vector3 directionToPlayer = playerObject.transform.position - transform.position;

            if (directionToPlayer.x < 0 && !isFacingLeft)
            {
                Flip();
                animatorRef.SetBool("IsFacingLeft", true);
                animatorRef.SetBool("IsFacingRight", false);

            }
            else if (directionToPlayer.x > 0 && !isFacingRight)
            {
                Flip();
                animatorRef.SetBool("IsFacingRight", true);
                animatorRef.SetBool("IsFacingLeft", false);

            }
        }
    }

    private void Flip()
    {
        // Toggle the facing flags
        isFacingLeft = !isFacingLeft;
        isFacingRight = !isFacingRight;

        // Flip the character's scale
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;

        // Flip the weapon's scale based on the character's orientation if isRanged is true
        if (isRanged)
        {
            Vector3 weaponScale = armPivot.localScale;

            // Flip along the X-axis
            weaponScale.x = isFacingLeft ? Mathf.Abs(weaponScale.x) * -1 : Mathf.Abs(weaponScale.x);

            // Flip along the Y-axis
            weaponScale.y = isFacingLeft ? Mathf.Abs(weaponScale.y) * -1 : Mathf.Abs(weaponScale.y);

            armPivot.localScale = weaponScale;
        }
    }

    public void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        if (!_isAlive)
            return;

        currentHealth -= damage;

        if (!_isKnockedBack)
            StartCoroutine(HandleKnockback(normalizedAttackDirection, knockBackPower));

        if (!isFlashing) // Check if a flash is not already in progress
        {
            StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            _isAlive = false;
            Die();
        }
    }

    public void Die()
    {
        boxCollider2D.enabled = false;
        if (hpDrop != null)
        {
            hpDrop.transform.SetParent(null);
            hpDrop.SetActive(true);
        }

        EventManager.InvokeGainFocus(focusAmountDrop);
        //EventManager.InvokeEnemyDeath(this);

        if (_task != null && _task is TaskKill)
            (_task as TaskKill).AddToCurrentKills();

        animatorRef.SetTrigger("Death");
        playerObject = null;
        isCooldown = true;

        if (isRanged && spriteRenderers[1] != null)
            spriteRenderers[1].enabled = false;

        if (isFacingLeft)
        {
            Vector3 characterScale = transform.localScale;
            characterScale.x = 3;
            transform.localScale = characterScale;
        }
        else if (isFacingRight)
        {
            Vector3 characterScale = transform.localScale;
            characterScale.x = -3;
            transform.localScale = characterScale;
        }

        Color newColor;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            newColor = spriteRenderers[i].color;
            newColor.a /= 2;
            spriteRenderers[i].color = newColor;
        }

        boxCollider2D.enabled = false;
        rb.simulated = false;
        //animatorRef.enabled = false;
        navMeshAgent.enabled = false;
        enabled = false;
    }
    private IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(0.45f);
        meleeAttack.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        meleeAttack.SetActive(false);
    }
    private IEnumerator DamageFlash()
    {
        isFlashing = true; // Set the flag to indicate that damage flash is in progress

        // Store original colors
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            originalColors.Add(renderer.color);
            renderer.color = damageColor;
        }

        yield return new WaitForSeconds(0.1f);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * 5f; // Adjust the speed of color lerp here
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                spriteRenderers[i].color = Color.Lerp(damageColor, originalColors[i], elapsedTime);
            }
            yield return null;
        }

        // Ensure we set the original color explicitly
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }

        isFlashing = false; // Reset the flag after damage flash is complete    
    }

    

    private IEnumerator DoHaltAgentForTime(float seconds)
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(seconds);
        navMeshAgent.isStopped = false;
    }
    public IEnumerator HandleKnockback(Vector2 normalizedAttackDirection, float knockBackPower)
    {
        if (_isKnockedBack)
            yield break;

        float elapsedTime = 0f;
        Vector3 originalPos = transform.position;
        Vector3 targetPos = originalPos + (Vector3)normalizedAttackDirection * knockBackPower;

        _isKnockedBack = true;
        while (elapsedTime < _knockBackTime)
        {
            float t = elapsedTime / _knockBackTime;
            float easeOutT = 1 - Mathf.Pow(1 - t, 3);  // Cubic ease-out
            transform.position = Vector3.Lerp(originalPos, targetPos, easeOutT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        _isKnockedBack = false;
    }
    public void HaltAgentForTime(float seconds)
    {
        if (_isHaltingAgent)
            return;

        StartCoroutine(DoHaltAgentForTime(seconds));
    }
}
