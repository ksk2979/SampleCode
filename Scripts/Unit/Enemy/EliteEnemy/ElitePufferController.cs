using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 원거리 복어
/// </summary>

// 먼거리에서 얼굴만 나와 반구형태로 가시 발사(5개 발사 되면 될듯) / 두더지 잡기 형태라고 하는데 롤에 조이 생각하면 될듯
// 수정 : 두더지 형태로 지속적으로 돌아다니면서 공격이 안되다가 공격할때만 공격을 받을수 있는 형태로 수정
// 공격 하고 조금 있다가 다시 들어가는 형태로 구현
public class ElitePufferController : EnemyController
{
    float _animScale = 0f;
    bool _move = false;
    bool _attackStart = false;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        SetState(eCharacterStates.Spawn);
    }
    protected override void IdleInit()
    {
        //if (!_onSpawn) { _onSpawn = true; _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SPAWN); SetState(eCharacterStates.Spawn); return; }
        _movement.AgentStop(true);
        if (m_isSpawn == false)
            m_idleTime = _enemyStats._idleTime;
        m_isSpawn = false;
        _idleInitTIme = Time.realtimeSinceStartup;
        SetTargetting();
    }
    protected override void IdleUpdate()
    {
        _movement.SetDestination(_trans.position);
        if (_animaController != null)
        {
            if (_move)
            {
                _animScale -= Time.deltaTime;
                if (_animScale < 0f) { _animScale = 0f; _move = false; }
                _animaController.VelocityY = _animScale;
            }
        }
        if (Time.realtimeSinceStartup - _idleInitTIme < m_idleTime)
            return;
        if (StandardFuncUnit.CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
        {
            SetState(eCharacterStates.Attack);
            return;
        }
        SetMoveState();
    }

    protected override void MoveInit()
    {
        base.MoveInit();
        SetNoneTargetting();
    }
    protected override void MoveUpdate()
    {
        if (_movement.enabled == true)
            SetSpeed();

        _movement.AgentStop(false);

        if (_animaController != null)
        {
            if (!_move)
            {
                _animScale += Time.deltaTime;
                if (_animScale > 1f) { _animScale = 1f; _move = true; }
                _animaController.VelocityY = _animScale;
            }
        }

        if (TargetCheckFuntion())
        {
            if (StandardFuncUnit.CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
                SetState(eCharacterStates.Attack);
            else
                SetMoveState();
        }

        //높이 오차 수정
        var dist = Vector3.Distance(_trans.position, _hitPoint);
        if (dist <= 1.8f)
            SetState(eCharacterStates.Idle);
        
    }

    protected override void TraceInit()
    {
        base.TraceInit();
        SetNoneTargetting();
    }
    protected override void TraceUpdate()
    {
        if (_movement.enabled == true)
            SetSpeed();

        if (_animaController != null)
        {
            if (!_move)
            {
                _animScale += Time.deltaTime;
                if (_animScale > 1f) { _animScale = 1f; _move = true; }
                _animaController.VelocityY = _animScale;
            }
        }

        flowTraceTime += Time.deltaTime;
        if (0.5f < flowTraceTime)
        {
            TargetCheckFuntion(_enemyStats._TraceRange);
            bool beWhachMyTeam = StandardFuncUnit.CheckRoundAndAngle(_trans, tag, 0.52f, 3, 10, _mylayer);
            _movement.obstacleAvoidanceType = beWhachMyTeam ? ObstacleAvoidanceType.LowQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;

            if (_movement.enabled == true)
            {
                if (_target != null)
                {
                    var targetPos = _target.transform.position;
                    targetPos.y = _trans.position.y;
                    _movement.SetDestination(targetPos);
                }
            }
            flowTraceTime = 0;
        }

        if (_target == null || _target.GetComponent<CharacterController>().IsDie())
        {
            SetMoveState();
            return;
        }

        if (StandardFuncUnit.CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
            SetState(eCharacterStates.Attack);
    }

    protected override void TraceFinish() { }

    public override void BasicAttack()
    {
        // 투~ 하고 전방 반구로 가시5개 발사 -90 -> 90까지
        //EnemyPufferAttack
        // 물거품을 앞으로 내보내기만 하면 됨 투~ 침뱉듯이
        for (int i = 0; i < 5; ++i)
        {
            GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyPufferAttack", Vector3.zero, Quaternion.identity);
            obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
            //obj.transform.forward = this.transform.forward;

            float angle = -90f + i * 45f;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
            obj.transform.forward = direction;

            ElitePufferAttack ef = obj.GetComponent<ElitePufferAttack>();
            ef.InitData(_enemyStats, _enemyStats.GetTDD1());
            ef.OnStart();
        }
    }

    protected override void AttackInit()
    {
        _movement.AgentStop(true);
        WatchTarget();
        if (_animaController != null)
        {
            _animScale = 1f;
            _animaController.VelocityY = _animScale;
        }
        _attackStart = false;
    }
    protected override void AttackUpdate()
    {
        if (!_attackStart)
        {
            if (_animScale > 0f) { _animScale -= Time.deltaTime * 2f; }
            else
            {
                _move = false;
                _animScale = 0f;
                _attackStart = true;
                _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
                SetTargetting();
            }
            _animaController.VelocityY = _animScale;
        }
    }
}