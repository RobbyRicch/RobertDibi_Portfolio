using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuckStatusEffects
{
    None,
    EnlargePuck,
    Impulse,
    Unblockable,
    Ghost
}
[System.Serializable]
public struct PuckStageInfo
{
    public int StageSpeed;
    public int PushForce;
}
[RequireComponent(typeof(Rigidbody))]
public class Puck : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private LayerMask WallLayer;
    private PuckStatusEffects _currentStatusEffect;
    public PuckStatusEffects CurrentStatusEffect { get { return _currentStatusEffect; } set { _currentStatusEffect = value; } }

    [Header("Status Effects")]
    [SerializeField] private PuckEffectsManager _EffectManager;
    [SerializeField] private Renderer _puckRenderer;
    [SerializeField] private Material _puckOGMat;
    [SerializeField] private Material[] PuckStagesMats;
    [SerializeField] private Renderer rendererRef;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private float _timerSE = 0;
    private bool _statusEffect = false;
    private bool _unblockable = false;
    private bool _impulse = false;
    private bool _ghost = false;
    [SerializeField] private float _radius = 5f, _force = 1500f;
    [SerializeField] private GameObject _impulseOBJ;
    public bool EffectChanged = false;

    [Header("Puck Stages")]
    [SerializeField] private List<PuckStageInfo> _puckStages;
    [SerializeField] private int _onWallHitAddSpeed;
    [SerializeField] private int _onPlayerHitAddSpeed;
    private PuckStageInfo _currentPuckStage;
    [SerializeField] private int _puckStage = 1;
    private float _maxSpeed;
    private int _momentum;

    [Header("Timer")]
    [SerializeField] private float _timeToWearOffSE = 5f;

    private void Start()
    {
        _currentStatusEffect = PuckStatusEffects.None;
        _momentum = _puckStages[0].StageSpeed;
        _maxSpeed = _puckStages[4].StageSpeed;
        _currentPuckStage = _puckStages[0];
        rendererRef.material = PuckStagesMats[0];
        //body.AddForce(new Vector3(10, 0, 30), ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionNormal;
        switch (_currentStatusEffect)
        {
            case PuckStatusEffects.None:
                TurnOffStatusEffects();
                _statusEffect = false;
                break;
            case PuckStatusEffects.EnlargePuck:
                TurnOffStatusEffects();
                for (int i = 0; i < _EffectManager.effects.Count; i++)
                {
                    if (_EffectManager.effects[i].Status == PuckStatusEffects.EnlargePuck)
                    {
                        _puckRenderer.material = _EffectManager.effects[i].material;
                        _EffectManager.effects[i].Effect.SetActive(true);
                        EnableEnLarge(true);
                        _statusEffect = true;
                    }
                }
                break;
            case PuckStatusEffects.Impulse:
                TurnOffStatusEffects();
                for (int i = 0; i < _EffectManager.effects.Count; i++)
                {
                    if (_EffectManager.effects[i].Status == PuckStatusEffects.Impulse)
                    {
                        _puckRenderer.material = _EffectManager.effects[i].material;
                        _EffectManager.effects[i].Effect.SetActive(true);
                        ImpulseEnable(true);
                        _statusEffect = true;
                    }
                }
                break;
            case PuckStatusEffects.Unblockable:
                TurnOffStatusEffects();
                for (int i = 0; i < _EffectManager.effects.Count; i++)
                {
                    if (_EffectManager.effects[i].Status == PuckStatusEffects.Unblockable)
                    {
                        _puckRenderer.material = _EffectManager.effects[i].material;
                        _EffectManager.effects[i].Effect.SetActive(true);
                        UnblockableEnable(true);
                        _statusEffect = true;
                    }
                }
                break;
            case PuckStatusEffects.Ghost:
                TurnOffStatusEffects();
                for (int i = 0; i < _EffectManager.effects.Count; i++)
                {
                    if (_EffectManager.effects[i].Status == PuckStatusEffects.Ghost)
                    {
                        _puckRenderer.material = _EffectManager.effects[i].material;
                        //_EffectManager.effects[i].Effect.SetActive(true);
                        GhostEnable(true);
                        _statusEffect = true;
                    }
                }
                break;
        }
        //player
        if (((1 << collision.gameObject.layer) & PlayerLayer) != 0)
        {
            PlayerInputHandler player = collision.gameObject.GetComponent<PlayerInputHandler>();
            collisionNormal = collision.gameObject.transform.position - collision.contacts[0].point;
            collisionNormal.Normalize();
            Vector3 dir = new Vector3(collisionNormal.x, 0, collisionNormal.z);
            player.Controller.Rb.AddForce(dir * _currentPuckStage.PushForce, ForceMode.Impulse);

            if (_momentum >= _puckStages[0].StageSpeed)
            {
                _momentum += _onPlayerHitAddSpeed;
            }
            if (_unblockable)
            {
                //Puck layer ignore collision to Deflector layer

            }
            if (_impulse)
            {
                collisionNormal = collision.contacts[0].point;
                //collisionNormal.Normalize();
                Explode(collisionNormal);
            }

        }
        //walls
        if (((1 << collision.gameObject.layer) & WallLayer) != 0 && body.velocity.magnitude <= _maxSpeed)
        {
            if (_momentum < _maxSpeed)
                _momentum += _onWallHitAddSpeed;
            if (_impulse)
            {
                collisionNormal = collision.contacts[0].point;
                //collisionNormal.Normalize();
                Explode(collisionNormal);
            }
        }
        //Clamp Speed
        if (body.velocity.magnitude <= _maxSpeed)
        {
            switch (_puckStage)
            {
                case 1:
                    if (_momentum >= _puckStages[_puckStage].StageSpeed)
                    {
                        _puckStage++;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }

                    body.velocity *= (10f);
                    body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
                    //body.velocity = body.velocity.normalized * body.velocity.magnitude * 4;
                    //body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed - (_stageSpeed * 4));
                    break;
                case 2:
                    if (_momentum >= _puckStages[_puckStage].StageSpeed)
                    {
                        _puckStage++;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }
                    if (_momentum < _currentPuckStage.StageSpeed)
                    {
                        _puckStage--;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }

                    body.velocity *= (10f);
                    body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
                    //body.velocity = body.velocity.normalized * (body.velocity.magnitude * 8);
                    //body.velocity *= (12f);
                    //body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed - (_stageSpeed * 3));
                    break;
                case 3:
                    if (_momentum >= _puckStages[_puckStage].StageSpeed)
                    {
                        _puckStage++;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }
                    if (_momentum < _currentPuckStage.StageSpeed)
                    {
                        _puckStage--;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }

                    body.velocity *= (10f);
                    body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
                    //body.velocity = body.velocity.normalized * body.velocity.magnitude * 5;
                    //body.velocity *= (12f);
                    //body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed - (_stageSpeed * 2));
                    break;
                case 4:
                    if (_momentum >= _puckStages[_puckStage].StageSpeed)
                    {
                        _puckStage++;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }
                    if (_momentum < _currentPuckStage.StageSpeed)
                    {
                        _puckStage--;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }

                    body.velocity *= (10f);
                    body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
                    //body.velocity = body.velocity.normalized * body.velocity.magnitude * 3;
                    //body.velocity *= (12f);
                    //body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed - _stageSpeed);
                    break;
                case 5:
                    /*if (_momentum >= _puckStages[_puckStage].StageSpeed)
                    {
                        _puckStage++;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }*/
                    if (_momentum < _currentPuckStage.StageSpeed)
                    {
                        _puckStage--;
                        _currentPuckStage = _puckStages[_puckStage - 1];
                    }

                    body.velocity *= (10f);
                    body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
                    //body.velocity = body.velocity.normalized * body.velocity.magnitude * 4;
                    //body.velocity *= (12f);
                    //body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed);
                    break;
            }
        }
        else
            body.velocity = Vector3.ClampMagnitude(body.velocity, _currentPuckStage.StageSpeed);
    }
    private void Update()
    {
        if (!_statusEffect)
            rendererRef.material = PuckStagesMats[_puckStage - 1];
        else
        {
            if (EffectChanged)
            {
                _timerSE = 0;
                EffectChanged = false;
            }

            if (_timerSE < _timeToWearOffSE)
            {
                _timerSE += Time.deltaTime;
            }
            else
            {
                _currentStatusEffect = PuckStatusEffects.None;
                _timerSE = 0;
            }
        }
    }

    private void TurnOffStatusEffects()
    {
        _puckRenderer.material = _puckOGMat;
        foreach (var effect in _EffectManager.effects)
        {
            effect.Effect.SetActive(false);
        }
        EnableEnLarge(false);
        ImpulseEnable(false);
        UnblockableEnable(false);
    }

    private void EnableEnLarge(bool enable)
    {
        if (enable)
            capsuleCollider.radius = 1f;
        else
            capsuleCollider.radius = 0.5f;
    }
    private void ImpulseEnable(bool enable)
    {
        if (enable)
            _impulse = true;
        else
            _impulse = false;
    }
    private void UnblockableEnable(bool enable)
    {
        if (enable)
        {
            _unblockable = true;
            Physics.IgnoreLayerCollision(20, 21, true);
        }
        else
        {
            _unblockable = false;
            Physics.IgnoreLayerCollision(20, 21, false);
        }
    }
    private void GhostEnable(bool enable)
    {
        if (enable)
            _ghost = true;
        else
            _ghost = false;
    }

    private void Explode(Vector3 pos)
    {
        GameObject impulse = Instantiate(_impulseOBJ, pos, Quaternion.identity, null);
        Destroy(impulse, 2);

        /* Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
         foreach (Collider nearByObject in colliders)
         {
             if (nearByObject.gameObject == this.gameObject)
                 continue;
             Rigidbody _rb = nearByObject.GetComponent<Rigidbody>();
             if (_rb != null)
             {
                 //_rb.AddExplosionForce(_force, transform.position, _radius);

                 Vector3 dir = transform.position - nearByObject.transform.position;
                 Vector3 dirNoY = new Vector3(dir.x, 0, dir.z);
                 _rb.AddForce(-dirNoY.normalized * _force, ForceMode.Impulse);
             }
         }*/
    }
}
