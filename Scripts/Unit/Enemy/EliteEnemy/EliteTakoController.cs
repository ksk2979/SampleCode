using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 오징어
/// </summary>

// 배에 달라붙어 이속감소와 지속대미지 / 피통이 많이 큼
public class EliteTakoController : EnemyController
{
    PlayerController _playerController;
    Player _player;
    [SerializeField] float _distancePos = 1f;

    float _attackDamageTime = 0f;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        _movement.AgentEnabled(true);
        SetState(eCharacterStates.Spawn);
    }

    protected override void IdleUpdate()
    {
        _movement.SetDestination(_trans.position);
        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;
        if (Time.realtimeSinceStartup - _idleInitTIme < m_idleTime)
            return;
        if (StandardFuncUnit.CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
        {
            SetState(eCharacterStates.Hide);
            return;
        }
        SetMoveState();
    }
    protected override void MoveUpdate()
    {
        if (_movement.enabled == true)
            SetSpeed();

        _movement.AgentStop(false);

        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;

        if (TargetCheckFuntion())
        {
            if (StandardFuncUnit.CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
                SetState(eCharacterStates.Hide);
            else
                SetMoveState();
        }

        //높이 오차 수정
        var dist = Vector3.Distance(_trans.position, _hitPoint);
        if (dist <= 1.8f)
            SetState(eCharacterStates.Idle);
    }

    protected override void TraceUpdate()
    {
        if (_movement.enabled == true)
            SetSpeed();
        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;

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
            SetState(eCharacterStates.Hide);
    }

    //public override void BasicAttack()
    //{
    //    // 공격시 달라붙는다
    //    SetState(eCharacterStates.Hide);
    //}

    protected override void HideInit()
    {
        _movement.AgentEnabled(false);
        if (_animaController != null) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK); }
        // 타겟에 달라붙는다
        if (_target == null) { SetState(eCharacterStates.Trace); return; }
        _playerController = _target.GetComponent<PlayerController>();
        _player = _playerController.GetComponent<Player>();
        _attackDamageTime = 0f;
    }
    protected override void HideUpdate()
    {
        if (_playerController == null) { SetState(eCharacterStates.Trace); }

        if (!_playerController.IsDie())
        {
            if (!_playerController.GetStatusEffects.GetSlowOn) { _playerController.GetStatusEffects.SlowOn(); }
            else { _playerController.GetStatusEffects.SlowReAttack(); }

            _attackDamageTime += Time.deltaTime;
            if (_attackDamageTime > 0.5f) { _attackDamageTime = 0f; _player.TakeToDamage(_enemyStats.GetTDD1()); }

            // 적이 플레이어와의 거리를 두도록 위치 설정
            Vector3 direction = (_trans.position - _target.position).normalized; // 현재 위치에서 타겟 위치까지의 방향
            _trans.position = _target.position + direction * _distancePos; // 타겟 위치에서 일정 거리만큼 떨어진 위치
        }
        else
        {
            // 만약 적이 붙이 있는데 죽어 버렸으면 (어떤 다른 공격으로 인해서) 그냥 대기 상태로 돌아간다
            _movement.AgentEnabled(true);
            if (_animaController != null) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACKPUSH); }
            SetState(eCharacterStates.Trace);
        }
    }

    public override void AttackFinishCall() { }
}
