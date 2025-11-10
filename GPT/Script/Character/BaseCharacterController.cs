using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public delegate void NormalDel();

public abstract class BaseCharacterController : MonoBehaviour
{
    public enum eCharacterStates
    {
        Spawn = 0,
        Idle,
        Move,
        Attack,
        Trace,
        Die,
        KnockBack,
        KnockDown,
        Attack01,
        Max
    }

    protected virtual void SpawnInit() { }
    protected virtual void SpawnUpdate() { }
    protected virtual void SpawnFinish() { }

    protected virtual void IdleInit() { }
    protected virtual void IdleUpdate() { }
    protected virtual void IdleFinish() { }

    protected virtual void MoveInit() { }
    protected virtual void MoveUpdate() { }
    protected virtual void MoveFinish() { }

    protected virtual void AttackInit() { }
    protected virtual void AttackUpdate() { }
    protected virtual void AttackFinish() { }

    protected virtual void TraceInit() { }
    protected virtual void TraceUpdate() { }
    protected virtual void TraceFinish() { }

    protected virtual void DieInit() { }
    protected virtual void DieUpdate() { }
    protected virtual void DieFinish() { }

    protected virtual void KnockBackInit() { }
    protected virtual void KnockBackUpdate() { }
    protected virtual void KnockBackFinish() { }

    protected virtual void KnockDownInit() { }
    protected virtual void KnockDownUpdate() { }
    protected virtual void KnockDownFinish() { }

    public NormalDel[] InitDels = new NormalDel[(int)eCharacterStates.Max];
    public NormalDel[] UpdateDels = new NormalDel[(int)eCharacterStates.Max];
    public NormalDel[] FinishDels = new NormalDel[(int)eCharacterStates.Max];

    public eCharacterStates _curState = eCharacterStates.Spawn;
    public int _curStateToInt { get { return (int)_curState; } }

    #region 컴포넌트 변수들
    public Animator _animator;
    protected Collider _collider;
    [HideInInspector]
    public Transform _trans;
    protected AnimEventSender _animEventSender;
    protected Rigidbody _rigidbody;
    protected NavMeshAgent _agent;

    [SerializeField] protected float _moveSpeed;
    protected float _velocity_y = 0;

    protected void Init()
    {
        _trans = transform;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        if (_animator != null)
            _animEventSender = _animator.GetComponent<AnimEventSender>();
    }
    #endregion

    public abstract void OnStart();

    protected virtual void FixedUpdate()
    {
        StateUpdate();
    }

    protected void StateStart()
    {
        InitDels[(int)eCharacterStates.Spawn] = SpawnInit;
        InitDels[(int)eCharacterStates.Idle] = IdleInit;
        InitDels[(int)eCharacterStates.Move] = MoveInit;
        InitDels[(int)eCharacterStates.Attack] = AttackInit;
        InitDels[(int)eCharacterStates.Trace] = TraceInit;
        InitDels[(int)eCharacterStates.Die] = DieInit;
        InitDels[(int)eCharacterStates.KnockBack] = KnockBackInit;
        InitDels[(int)eCharacterStates.KnockDown] = KnockDownInit;

        UpdateDels[(int)eCharacterStates.Spawn] = SpawnUpdate;
        UpdateDels[(int)eCharacterStates.Idle] = IdleUpdate;
        UpdateDels[(int)eCharacterStates.Move] = MoveUpdate;
        UpdateDels[(int)eCharacterStates.Attack] = AttackUpdate;
        UpdateDels[(int)eCharacterStates.Trace] = TraceUpdate;
        UpdateDels[(int)eCharacterStates.Die] = DieUpdate;
        UpdateDels[(int)eCharacterStates.KnockBack] = KnockBackUpdate;
        UpdateDels[(int)eCharacterStates.KnockDown] = KnockDownUpdate;

        FinishDels[(int)eCharacterStates.Spawn] = SpawnFinish;
        FinishDels[(int)eCharacterStates.Idle] = IdleFinish;
        FinishDels[(int)eCharacterStates.Move] = MoveFinish;
        FinishDels[(int)eCharacterStates.Attack] = AttackFinish;
        FinishDels[(int)eCharacterStates.Trace] = TraceFinish;
        FinishDels[(int)eCharacterStates.Die] = DieFinish;
        FinishDels[(int)eCharacterStates.KnockBack] = KnockBackFinish;
        FinishDels[(int)eCharacterStates.KnockDown] = KnockDownFinish;
    }

    protected void SetState(eCharacterStates statas)
    {
        if (FinishDels[_curStateToInt] != null)
            FinishDels[_curStateToInt]();
        _curState = statas;
        if (InitDels[_curStateToInt] != null)
            InitDels[_curStateToInt]();
    }

    protected void StateUpdate()
    {
        if (UpdateDels == null || UpdateDels[_curStateToInt] == null) { return; }
        UpdateDels[_curStateToInt]();
    }

    internal bool IsDie()
    {
        return _curState == eCharacterStates.Die;
    }

    protected void VelocitySetting(float value)
    {
        if (value == 0f) { _rigidbody.constraints = RigidbodyConstraints.FreezeAll; }
        else { _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; }
        _velocity_y = value;
        if (_animator != null) { _animator.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, _velocity_y); }
    }
}
