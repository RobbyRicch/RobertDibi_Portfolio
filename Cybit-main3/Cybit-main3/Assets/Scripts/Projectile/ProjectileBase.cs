using UnityEditor.Rendering;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] protected float _maxTravelDistance;
    [SerializeField] protected float _knockBackPower;

    [Header("OverlapBox")]
    [SerializeField] protected Vector2 _boxPositionOffset;
    [SerializeField] protected Vector2 _boxSize;
    [SerializeField] protected bool _isUsingOverlap;
    protected float _boxAngle = 0.0f;

    protected const string _targetTag = "Enemy", _wallsTag = "Walls", _roofsTag = "Roofs", _canisterTag = "Canister";

    protected Vector2 _originPos;
    protected Vector2 _direction;
    protected bool _canHit = true;
    protected int _damage = 0;
    protected float _speed = 0;

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
            if (collision.CompareTag(_targetTag) || collision.CompareTag(_roofsTag) || collision.CompareTag(_wallsTag) || collision.CompareTag(_canisterTag))
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
        if (Time.timeScale != 1)
            _rb.velocity = _speed / 5 * Time.fixedUnscaledDeltaTime * _direction;
        else
            _rb.velocity = _speed * Time.fixedUnscaledDeltaTime * _direction;
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

        if (collision.TryGetComponent(out EnemyBase enemy))
        {
            if (enemy is JackalWarden_AI warden)
            {
                warden.TakeDamage(_direction, _damage, 0);
                Destroy(gameObject);
                return;
            }

            enemy.TakeDamage(_direction, _damage, _knockBackPower);
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out Barrier_System barrier) && barrier._isActive && barrier._canBeDamaged)
        {
            barrier.TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out Dummy dummy))
        {
            dummy.TakeDamage();
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out HPCapsule_Interactable capsule) && !capsule._hasBeenShot && !capsule._isBroken)
        {
            capsule._hasBeenShot = true;
            Destroy(gameObject);
        }

    }
    protected virtual void OnHitObstacle(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(_wallsTag) || collider.gameObject.CompareTag(_roofsTag))
            Destroy(gameObject);
    }
    protected virtual void OnHitObstacle(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_wallsTag) || collision.gameObject.CompareTag(_roofsTag))
            Destroy(gameObject);
    }
    protected virtual void CreateImpact(Collider2D collision)
    {
        if (_impactVFX)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Instantiate(_impactVFX, transform.position, rotation);
        }
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
