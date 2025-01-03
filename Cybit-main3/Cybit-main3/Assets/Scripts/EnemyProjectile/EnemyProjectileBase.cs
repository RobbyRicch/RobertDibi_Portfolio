using UnityEditor.Rendering;
using UnityEngine;

public class EnemyProjectileBase : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] protected float _maxTravelDistance;
    [SerializeField] protected float _knockBackPower;
    protected bool _isDeflected = false;

    [Header("OverlapBox")]
    [SerializeField] protected Vector2 _boxPositionOffset;
    [SerializeField] protected Vector2 _boxSize;
    [SerializeField] protected bool _isUsingOverlap;
    protected float _boxAngle = 0.0f;

    protected const string _targetTag = "Player", _wallsTag = "Walls", _roofsTag = "Roofs";
    protected const string _enemyTag = "Enemy";
    protected const string _deflectionTag = "Deflect";

    protected Vector2 _originPos;
    protected bool _canHit = true;
    protected int _damage = 0;
    protected float _speed = 0;

    protected Vector2 _direction;
    public Vector2 Direction => _direction;

    [Header("Optional Components")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected ImpactVFX _impactVFX;

    protected virtual void Start()
    {
        OnStart();
    }
    private void LateUpdate()
    {
        if (_isUsingOverlap)
            CheckOverlapBox();
    }
    protected virtual void FixedUpdate()
    {
        Move();
        HandleTravelDistance();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isUsingOverlap)
            return;

        OnHit(collision);
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isUsingOverlap)
            return;

        OnHitObstacle(collision);
    }
    protected virtual void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
            return;

        if (_impactVFX)
            CreateImpact(null);
    }

    protected virtual void CheckOverlapBox()
    {
        Vector2 boxPosition = (Vector2)transform.position + _boxPositionOffset;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(boxPosition, _boxSize, _boxAngle);

        for (int i = 0; i < collisions.Length; i++)
        {
            Collider2D collision = collisions[i];
            if (collision.CompareTag("Untagged"))
            {
                continue;
            }
            else if (_isDeflected && collision.CompareTag("Enemy"))
            {
                OnHitEnemy(collision);
            }
            else if (collision.CompareTag(_targetTag) || collision.CompareTag(_roofsTag) || collision.CompareTag(_wallsTag))
            {
                if (collision.isTrigger)
                    OnHit(collision);
                else
                    OnHitObstacle(collision);

                break;
            }
        }
    }
    protected virtual void OnStart()
    {
        _originPos = transform.position;
        _boxAngle = transform.eulerAngles.z;
    }
    protected virtual void Move()
    {
        _rb.velocity = _speed * Time.fixedDeltaTime * _direction;
    }
    protected virtual void HandleTravelDistance() // fixedUpdate
    {
        float distanceTraveled = Vector2.Distance(_originPos, _rb.position);
        if (distanceTraveled >= _maxTravelDistance)
            Destroy(gameObject);
    }
    protected virtual void OnHit(Collider2D collision)
    {
        if (!_canHit)
            return;

        if (collision.gameObject.CompareTag(_wallsTag) || collision.gameObject.CompareTag(_roofsTag))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag(_targetTag))
        {
            EventManager.InvokePlayerHit(_direction, _damage, _knockBackPower);
            Destroy(gameObject);
        }
    }
    protected virtual void OnHitEnemy(Collider2D collision)
    {
        if (!_canHit)
            return;

        collision.GetComponent<EnemyBase>().TakeDamage(_direction, _damage, _knockBackPower);
        Debug.Log("Enemy bullet hit" + collision.name);
        Destroy(gameObject);
    }
    protected virtual void OnHitObstacle(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Walls") || collider.gameObject.CompareTag("Roofs"))
            Destroy(gameObject);
    }
    protected virtual void OnHitObstacle(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Roofs"))
            Destroy(gameObject);
    }
    protected virtual void CreateImpact(Collider2D collision)
    {
        if (_impactVFX)
        {
            // Match the VFX rotation with the projectile's current rotation
            Quaternion projectileRotation = transform.rotation;
            Instantiate(_impactVFX, transform.position, projectileRotation);
        }
    }

    public virtual void Deflect()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = -newScale.x;

        transform.localScale = newScale;
        _speed = -_speed;
        _isDeflected = true;
    }

    public virtual void SetStats(int dmg, float speed, Vector2 dir)
    {
        _damage = dmg;
        _speed = speed;
        _direction = dir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; // Set the gizmo color
        Vector2 boxPosition = (Vector2)transform.position + _boxPositionOffset;

        // Calculate the rotation
        Quaternion rotation = Quaternion.Euler(0, 0, _boxAngle);

        // Draw the wireframe box
        Gizmos.matrix = Matrix4x4.TRS(boxPosition, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector2.zero, _boxSize);
    }
}
