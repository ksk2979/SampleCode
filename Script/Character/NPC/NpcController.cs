using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NpcController : BaseCharacterController, IDayResettable
{
    [Header("Move(NPC)")]
    [SerializeField] protected float _stoppingDistance = 0.2f;  // 아주 근접 판정
    [SerializeField] protected float _repathInterval = 0.5f;    // 재지정 간격(선택)
    [SerializeField] protected float _agentSpeed = 3.5f;        // NavMeshAgent 속도
    [SerializeField] protected float _agentAngularSpeed = 720f; // 회전 속도
    [SerializeField] protected float _agentAccel = 12f;

    protected Vector3 _initPosition;
    protected Quaternion _initRotation;
    protected bool _hasInitTransform;
    protected DayCycleManager _dayCycleManager;

    public override void OnStart()
    {
        if (_trans == null)
        {
            Init();
            StateStart();
            _animator.applyRootMotion = false;
            //_animEventSender.AddEvent("DieFinishCall", DieFinishCall);
            //_animEventSender.AddEvent("AttackFuntion", AttackFuntion);
            //_animEventSender.AddEvent("SkillFuntion", SkillFuntion);
            //_animEventSender.AddEvent("AttackFinishCall", AttackFinishCall);
        }

        if (_agent != null)
        {
            _agent.updatePosition = true;
            _agent.updateRotation = true;
            _agent.isStopped = true;
            _agent.speed = _agentSpeed;
            _agent.angularSpeed = _agentAngularSpeed;
            _agent.acceleration = _agentAccel;
            _agent.stoppingDistance = _stoppingDistance;
        }

        _moveSpeed = _agentSpeed;
        SetState(eCharacterStates.Idle);

        CacheInitTransform();
        EnsureRegisteredWithDayCycle();
    }
    
    protected virtual void OnDisable()
    {
        if (_dayCycleManager != null)
        {
            _dayCycleManager.UnregisterResettable(this);
        }
    }

    void StartMove()
    {
        SetState(eCharacterStates.Move);
    }

    protected override void SpawnUpdate()
    {
        SetState(eCharacterStates.Idle);
    }

    protected override void IdleInit()
    {
        VelocitySetting(0f);
        if (_rigidbody != null) _rigidbody.isKinematic = true;
        if (_agent != null) _agent.isStopped = true;
    }

    protected override void MoveInit()
    {
        // 이동 시작
        VelocitySetting(0.1f);
        if (_rigidbody != null) { _rigidbody.isKinematic = true; }
    }
    protected override void MoveUpdate()
    {
        if (_agent == null)
        {
            SetState(eCharacterStates.Idle);
            return;
        }

        // 이동하는거 구현 해야함

        // 애니메이터 속도 파라미터(0~1)
        //if (_animator != null)
        //{
        //    float norm = Mathf.Clamp01(_agent.velocity.magnitude / Mathf.Max(0.0001f, _moveSpeed));
        //    _animator.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, norm, 0.05f, Time.fixedDeltaTime);
        //}
    }
    protected override void MoveFinish()
    {
        if (_agent != null) _agent.isStopped = true;
    }

    protected override void AttackInit()
    {
        VelocitySetting(0f);
        if (_animator != null) { _animator.SetTrigger(CommonStaticKey.ANIMPARAM_ATTACK); }
    }

    protected override void DieInit()
    {
        base.DieInit();
        if (_animator != null)
        {
            _animator.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 0f);
            _animator.SetTrigger(CommonStaticKey.ANIMPARAM_DEAD);
            _animator.SetBool(CommonStaticKey.ANIMPARAM_ISDEAD, true);
        }
    }

    void CacheInitTransform()
    {
        if (_hasInitTransform) { return; }

        _initPosition = _trans.position;
        _initRotation = _trans.rotation;
        _hasInitTransform = true;
    }

    void EnsureRegisteredWithDayCycle()
    {
        var uiManager = UIManager.GetInstance;
        if (uiManager != null)
        {
            var dayCycle = uiManager.GetDayCycleManager;
            if (dayCycle != null)
            {
                dayCycle.RegisterResettable(this);
                _dayCycleManager = dayCycle;
            }
        }
    }

    public virtual void ResetForNewDay()
    {
        if (_hasInitTransform) { return; }

        if (_agent != null)
        {
            _agent.ResetPath();
            _agent.Warp(_initPosition);
            _agent.isStopped = true;
        }

        if (_rigidbody != null)
        {
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.position = _initPosition;
            _rigidbody.rotation = _initRotation;
        }

        _trans.SetPositionAndRotation(_initPosition, _initRotation);

        if (_curState != eCharacterStates.Idle)
        {
            SetState(eCharacterStates.Idle);
        }
    }
}