using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JackalWardenAI : MonoBehaviour
{

    [Header("Stats // General HP/CD")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    [Header("Attack Bank")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletProjectileSpeed;
    [SerializeField] private GameObject hounds;
    [SerializeField] private int houndAmount;
    [SerializeField] private float fireCooldown;
    [SerializeField] private int shotsFired;
    [SerializeField] private int maxShots;

    [Header("States")]
    [SerializeField] private bool IsAlive;
    [SerializeField] private bool IsFiring;
    [SerializeField] private bool BarrierActive;
    [SerializeField] private bool ReleaseHounds;
    [SerializeField] private bool canFire;
    //[SerializeField] private bool isWidningUp;

    [Header("Pattern")]
    [SerializeField] private bool NextAttackBarrage;
    [SerializeField] private bool NextAttackHounds;
    [SerializeField] private bool currentAttackBarrage;
    [SerializeField] private bool currentAttackHounds;


    [Header("Attack Pattern Timing")]
    [SerializeField] private float timeforWindup;
    [SerializeField] private float timeforShooting;
    [SerializeField] private float timeforBarrierReturn;

    [Header("Refrences")]
    [SerializeField] GameObject playerObject;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    [SerializeField] Transform armPivot;
    [SerializeField] Transform firePoint;
    [SerializeField] Animator animatorRef;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip taunt, damage, death;

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


    private NavMeshAgent navMeshAgent;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        originalColor = spriteRenderers[0].color;
        IsAlive = true;
        playerObject = GameObject.FindWithTag("Player");
        NextAttackBarrage = true;
        // Constrain rotation
        transform.rotation = Quaternion.identity;

        Flip();

    }
    private void Update()
    {

        if (IsAlive)
        {
            AimAtPlayer();
            AimWeapon();
            
        }

        if (NextAttackBarrage)
        {
            StartCoroutine(ActiveBarrage());
            //NextAttackHounds = true;
            NextAttackBarrage = false;
        }

        if (true)
        {

        }

        if (IsFiring && currentAttackBarrage)
        {
            Barrage();

        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

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
    private void AimAtPlayer()
    {
        Vector3 directionToPlayer = playerObject.transform.position - transform.position;
        if (directionToPlayer.x < 0 && !isFacingLeft)
        {
            Flip();


        }
        else if (directionToPlayer.x > 0 && !isFacingRight)
        {
            Flip();


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

        Vector3 weaponScale = armPivot.localScale;

        // Flip along the X-axis
        weaponScale.x = isFacingLeft ? Mathf.Abs(weaponScale.x) * -1 : Mathf.Abs(weaponScale.x);

        // Flip along the Y-axis
        weaponScale.y = isFacingLeft ? Mathf.Abs(weaponScale.y) * -1 : Mathf.Abs(weaponScale.y);

        armPivot.localScale = weaponScale;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

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
        // should be event related to player
        EventManager.InvokeGainFocus(focusAmountDrop);
        IsAlive = false;
        playerObject = null;
        boxCollider2D.enabled = false;

        Destroy(gameObject, 2);
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

    private IEnumerator ActiveBarrage()
    {
        currentAttackBarrage = true;
        //isWidningUp = true;
        yield return new WaitForSeconds(timeforWindup);
        //isWidningUp = false;
        IsFiring = true;
        canFire = true;
    }

    private void Barrage()
    {
        if (canFire)
        {
            Vector2 direction = firePoint.right;


            GameObject firedBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            Rigidbody2D bulletRb = firedBullet.GetComponent<Rigidbody2D>();


            float bulletSpeed = bulletProjectileSpeed;
            bulletRb.velocity = direction * bulletSpeed;
            canFire = false;
            StartCoroutine(ShootCooldown());
            Destroy(firedBullet, 1.25f);
            shotsFired++;

            if (shotsFired == maxShots)
            {
                canFire = false;
                IsFiring = false;
                currentAttackBarrage=false;
            }

        }
    }
    IEnumerator ShootCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}
