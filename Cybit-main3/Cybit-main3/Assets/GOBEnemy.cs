using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GOBEnemy : MonoBehaviour
{
    [Header("Stats // General HP/CD")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;
    [SerializeField] float _knockBackTime = 0.15f;
    private bool _isKnockedBack = false;

    [Header("States")]
    //[SerializeField] private bool isChasing;
    [SerializeField] private bool canHook;
    [SerializeField] private bool isHooking;
    [SerializeField] private bool hasWeapon;
    [SerializeField] private bool canMove;
    //[SerializeField] private bool isRunning;
    //[SerializeField] private bool InStealthMode;
    [SerializeField] private bool IsAlive;


    [Header("Refrences")]
    [SerializeField] Player_Controller _playerController;
    [SerializeField] private BoxCollider2D hookCollider;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    [SerializeField] Transform armPivot;
    [SerializeField] Transform carryingTransform;
    [SerializeField] Animator animatorRef;
    [SerializeField] Animator hookAnimator;
    public Rigidbody2D Rb;


    [Header("Drops")]
    [SerializeField] GameObject hpDrop;
    [SerializeField] float focusAmountDrop;
    [SerializeField] GunBase carriedWeapon;
    private Vector3 _dropGunScale = new(2.5f, 2.5f, 1.0f);

    [Header("DMG Flash")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private bool isFlashing = false;

    [Header("Flip")]
    [SerializeField] private bool isFacingLeft;
    [SerializeField] private bool isFacingRight;

    [Header("Gizmo")]
    private Color gizmoColor = Color.red;
    private float detectionRadius = 5f;

    private NavMeshAgent navMeshAgent;
    private Color originalColor;
    private SpriteRenderer carriedSprite;
    

    private void Start()
    {
        currentHealth = maxHealth;
        originalColor = spriteRenderers[0].color;
        //isChasing = true;
        IsAlive = true;
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Constrain rotation
        transform.rotation = Quaternion.identity;

        Flip();

    }
    private void Update()
    {
        if (canMove && !hasWeapon)
        {
            ChaseState();
        }
        if (!hasWeapon)
        {
            AimAtPlayer();
            AimHook();
        }

        if (hasWeapon && IsAlive)
        {
            hookAnimator.SetBool("CanHook", false);
            StartCoroutine(StealthMode(0.35f, 1));
            RunAway();
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hookCollider.enabled && isHooking)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GunBase playerCurrentWeapon = _playerController.CurrentlyEquippedGun;
                _playerController.SwapGuns();

                if (playerCurrentWeapon is GunPrimary)
                    _playerController.PrimaryGun = null;
                else if (playerCurrentWeapon is GunSideArm)
                    _playerController.SideArm = null;

                if (playerCurrentWeapon)
                {
                    carriedWeapon = playerCurrentWeapon;
                    Transform weaponTransform = carriedWeapon.transform;
                    weaponTransform.position = carryingTransform.position;
                    weaponTransform.rotation = Quaternion.identity;
                    weaponTransform.SetParent(carryingTransform);
                    hasWeapon = true;
                    playerCurrentWeapon.CanFire = false;
                    playerCurrentWeapon.enabled = false;
                    spriteRenderers.Add(playerCurrentWeapon.SR);
                    carriedSprite = playerCurrentWeapon.SR;
                    carriedSprite.enabled = true;
                }
            }
        }
    }
    private void ChaseState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerController.transform.position);

        if (distanceToPlayer > navMeshAgent.stoppingDistance)
        {
            //isChasing = true;
            navMeshAgent.SetDestination(_playerController.transform.position);
            animatorRef.SetBool("IsMoving", true);
            hookAnimator.SetBool("CanHook", false);
        }
        else
        {
            //isChasing = false;
            navMeshAgent.SetDestination(transform.position);
            StartCoroutine(AttemptHook());
            animatorRef.SetBool("IsMoving", false);
            hookAnimator.SetBool("CanHook", true);
        }

        // Fix rotation to prevent it from being forced to -90 on the X-axis
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void AimHook()
    {
        Vector3 armPosition = _playerController.transform.position;
        Vector3 direction = armPosition - armPivot.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the rotation only on the Z-axis
        armPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void AimAtPlayer()
    {
        Vector3 directionToPlayer = _playerController.transform.position - transform.position;
        if (directionToPlayer.x < 0 && !isFacingLeft)
            Flip();
        else if (directionToPlayer.x > 0 && !isFacingRight)
            Flip();
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
        Vector3 weaponScale = armPivot.localScale;

        // Flip along the X-axis
        weaponScale.x = isFacingLeft ? Mathf.Abs(weaponScale.x) * -1 : Mathf.Abs(weaponScale.x);

        // Flip along the Y-axis
        weaponScale.y = isFacingLeft ? Mathf.Abs(weaponScale.y) * -1 : Mathf.Abs(weaponScale.y);

        armPivot.localScale = weaponScale;
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
    public void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        currentHealth -= damage;

        if (!_isKnockedBack)
            StartCoroutine(HandleKnockback(normalizedAttackDirection, knockBackPower));

        if (!isFlashing) // Check if a flash is not already in progress
        {
            StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        EventManager.InvokeGainFocus(focusAmountDrop);
        IsAlive = false;
        canMove = false;
        boxCollider2D.enabled = false;

        if (carriedWeapon)
        {
            carriedWeapon.transform.SetParent(null);
            carriedWeapon.Pickup.Collider.enabled = true;
            carriedWeapon.transform.rotation = Quaternion.identity;
            carriedWeapon.transform.localScale = _dropGunScale;
            carriedWeapon.SR.enabled = true;
            carriedWeapon.IsEquipped = false;
            carriedWeapon.IsFiring = false;
            carriedWeapon.IsFull = true;
            carriedWeapon.CanFire = false;
            carriedWeapon.Pickup.gameObject.SetActive(true);
            carriedSprite.color = originalColor;
            carriedWeapon.enabled = true;
            carriedWeapon.HighlightVFX.SetActive(true);
            hasWeapon = false;
            spriteRenderers.Remove(carriedWeapon.SR);
            carriedSprite = null;
        }

        _playerController = null;
        Destroy(gameObject);
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

        yield return new WaitForSeconds(flashDuration);

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

    private IEnumerator AttemptHook()
    {
        isHooking = true;
        yield return new WaitForSeconds(0.5f);
        isHooking = false;
    }

    private IEnumerator StealthMode(float targetOpacity, float duration)
    {
        // Store the original colors of the sprite renderers
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            originalColors.Add(renderer.color);
        }

        // Gradually adjust the color for each SpriteRenderer
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Interpolate the color gradually over the specified duration
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                // Calculate the current color with the desired opacity
                Color currentColor = Color.Lerp(originalColors[i], new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, targetOpacity), elapsedTime / duration);

                // Update the renderer's color
                spriteRenderers[i].color = currentColor;
            }

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the renderer's color is set to the target color
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, targetOpacity);
        }

        //InStealthMode = true;
    }

    private void RunAway()
    {
        float detectionRadius = 5f; // Set your desired detection radius here
        //isRunning = true;
        // Check if the player is within the detection radius
        if (Vector3.Distance(transform.position, _playerController.transform.position) <= detectionRadius)
        {
            // Calculate a new target position far away from the player
            Vector3 directionToPlayer = transform.position - _playerController.transform.position;
            Vector3 runAwayPosition = transform.position + directionToPlayer.normalized * 10f; // Adjust the distance as needed

            // Set the new target position for the NavMeshAgent
            navMeshAgent.SetDestination(runAwayPosition);
        }
        else
        {
            //isRunning = false; // Player is not within the detection radius, stop running
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to represent the detection radius
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
