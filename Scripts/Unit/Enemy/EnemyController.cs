using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static StandardFuncUnit;

public class EnemyController : CharacterController
{
    public BaseAttack _baseAttack;
    public AnimationCurve knockBackBrakeCurve;

    protected EnemyStats _enemyStats = null;
    protected Vector3 _hitPoint = Vector3.zero;
    protected float _idleInitTIme = 0.0f;
    protected Transform _target = null;
    private Action _offTargetDieCall;

    public bool testDieInitCheck = false;
    public bool testDieFinishCheck = false;
    public bool testDieAffterHitAnimCheck = false;
    protected float m_idleTime = 0.0f;
    protected int _mylayer = 0;

    protected GameObject _detecter; // 탐지기

    protected List<int> _healthThresholds;

    //public float _finalAttackDist
    //{
    //    get { return _enemyStats._AttackDist + (_collider as CapsuleCollider).radius; }
    //}

    protected StageManager _stageManager;

    protected bool m_isSpawn = false;
    //protected bool _onSpawn = false;

    // 회전 관련
    [SerializeField] protected float _rotationAngle = 10f; // 적을 설정된 각에 맞춰 바라봄
    protected float _maxTurnSpeed = 720f;
    protected float _turnAcceleration = 120f; // 회전 가속도
    protected float _currentTurnSpeed; // 현재 회전 속도

    protected float _attackTime = 0f;
    protected float _attackTimeMax = 3f;
    protected float[] _randomAttackArr = { 1f, 5f }; // 최소, 최대
    protected float[] _randomMoveArr = { 1f, 10f }; // 최소, 최대
    protected int _comboIndex;

    protected float _moveTime = 0f;
    protected float _moveTimeMax = 3f;

    protected int[] _comboIndexSetting;

    protected float flowTraceTime = 0.6f;

    protected bool _canRotateInAttack = false;
    int defaultLayerMask;

    // 움직임 끼임 현상 해결 픽스
    protected float _distanceF = 10f; // 일정 거리 멀어지면 따라가기로 변형
    protected Vector3 _lastPosition;
    protected float _distanceThreshold = 0.1f; // 최소 이동 거리 기준
    protected float _idleTimeThreshold = 3f; // 일정 시간 동안 움직이지 않으면 전환
    protected float _idleTimer = 0f;

    // 공격 관련
    protected bool _bossAttack = false;
    // 사운드 관련
    protected bool _onSound = false;

    // 움직임 속도 추가 관련
    protected float _traceAddSpeed = 2f;
    protected float _traceMoveAddSpeed = 1.5f;
    protected float _attackAddSpeed = 1.5f;

    protected float _attackDestinationTime = 0f;

    protected float _traceTime = 0f;
    protected float _traceMaxTime = 10f;

    // 이펙트
    GameObject _sternEffect;
    /// <summary>
    /// 적이 죽을떄 타겟 포인트 반환
    /// </summary>
    /// <param name="offTarget"></param>
    /// <returns></returns>
    internal Transform SetDieCall(Action offTarget)
    {
        _offTargetDieCall = offTarget;
        return _animaController.AnimatorTrans;
    }

    public EnemyStats GetEnemyStat()
    {
        return _enemyStats;
    }
    public override void OnStart()
    {
        testDieInitCheck = false;
        testDieFinishCheck = false;
        testDieAffterHitAnimCheck = false;
        _collider.enabled = true;
        _movement.AgentEnabled(true);

        if (!_oneOnStartInit)
        {
            _mylayer = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT);
            _enemyStats = _characterStats as EnemyStats;
            _targetTag = _enemyStats.GetTDD1().targetTag;
            _unitLayerMask = _enemyStats.GetTDD1().layerMask;

            if (_baseAttack != null)
            {
                var eStat = (_characterStats as EnemyStats);

                _baseAttack.InitData(_enemyStats, _enemyStats.GetTDD1());
            }

            if (_animaController != null)
            {
                _animaController.AnimEnventSender("DieFinishCall", DieFinishCall);
                _animaController.AnimEnventSender("SpawnFinishCall", SpawnFinishCall);
                _animaController.AnimEnventSender("AttackFinishCall", AttackFinishCall);
                _animaController.AnimEnventSender("AttackRotateStartCall", AttackRotateStartCall);
                _animaController.AnimEnventSender("BasicAttack", BasicAttack);
                _animaController.AnimEnventSender("FinishKnockBack", FinishKnockBack);
                _animaController.AnimEnventSender("FinishKnockDown", FinishKnockDown);
                _animaController.AnimEnventSender("NotHit", NotHit);
                _animaController.AnimEnventSender("NotHitPriority", NotHitPriority);
                _animaController.AnimEnventSender("OnHit", OnHit);
                _animaController.AnimEnventSender("OnHitPriority", OnHitPriority);
                _movement.defalutAnimatorSpeed = _animaController.Speed;
            }
        }
        defaultLayerMask = _unitLayerMask;
        InitColor();
        _movement.angularSpeed = 1000;
        if (GetComponent<Enemy>().IsBossOrElite())
            InGameUIManager.GetInstance.SetBossHp(_enemyStats.HpRate());
    }

    protected override void SpawnInit()
    {
        base.SpawnInit();
        m_isSpawn = true;
        m_idleTime = 0;
        SetNoneTargetting();
        _movement.AgentStop(true);
        _animator.gameObject.SetActive(true);
    }

    protected virtual void SpawnFinishCall()
    {
        SetTargetting();
        SetState(eCharacterStates.Idle);
    }

    internal void Ready() { }

    public override void SetMoveState()
    {
        if (_curState == eCharacterStates.Die)
            return;

        //모든 행동이 끝난후 타겟이 있었다면 그타겟만 좇아라
        if (_target != null && _target.GetComponent<CharacterController>().IsDie() == false)
        {
            SetState(eCharacterStates.Trace);
            return;
        }
        var traceTaret = CheckTraceTarget(_trans, _targetTag, _enemyStats._TraceRange, _unitLayerMask);
        if (traceTaret != null)
        {
            _target = traceTaret;
            SetState(eCharacterStates.Trace);
        }
        else
        {
            _hitPoint = _randomPos;
            SetState(eCharacterStates.Move);
        }
    }

    protected override void IdleInit()
    {
        //if (!_onSpawn) { _onSpawn = true; _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SPAWN); SetState(eCharacterStates.Spawn); return; }
        _movement.AgentStop(true);
        if (m_isSpawn == false)
            m_idleTime = _enemyStats._idleTime;
        m_isSpawn = false;
        _idleInitTIme = Time.realtimeSinceStartup;
        base.IdleInit();
    }

    protected override void IdleUpdate()
    {
        base.IdleUpdate();
        _movement.SetDestination(_trans.position);
        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;
        if (Time.realtimeSinceStartup - _idleInitTIme < m_idleTime)
            return;
        if (CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
        {
            SetState(eCharacterStates.Attack);
            return;
        }
        SetMoveState();
    }
    
    protected override void IdleFinish()
    {
        base.IdleFinish();
    }

    protected override void MoveInit()
    {
        base.MoveInit();
        _movement.AgentStop(false);
        if (_movement.enabled == true)
        {
            SetSpeed();
            _movement.SetDestination(_hitPoint);
        }

        _movement._oldFwd = _trans.forward;
    }

    protected override void MoveUpdate()
    {
        base.MoveUpdate();

        if (_movement.enabled == true)
            SetSpeed();

        _movement.AgentStop(false);

        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;

        if (TargetCheckFuntion())
        {
            if (CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
                SetState(eCharacterStates.Attack);
            else
                SetMoveState();
        }

        //높이 오차 수정
        var dist = Vector3.Distance(_trans.position, _hitPoint);
        if (dist <= 1.8f)
            SetState(eCharacterStates.Idle);
    }

    protected override void MoveFinish()
    {
        base.MoveFinish();
    }

    protected override void AttackInit()
    {
        _movement.AgentStop(true);
        WatchTarget();
        if (_animaController != null)
        {
            _animaController.VelocityY = 0f;
            if (IsInDist(_target, _trans, _enemyStats._AttackRange + 1))
                _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
            else
                SetState(eCharacterStates.Trace);
        }
    }

    protected override void AttackUpdate()
    {
        base.AttackUpdate();
        if (_canRotateInAttack) { FaceTarget(); }
    }

    public void AttackRotateStartCall()
    {
        _canRotateInAttack = true;
    }

    public virtual void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;

        _target = null;
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        SetMoveState();
    }

    public virtual void BasicAttack()
    {
        _baseAttack.DoAttackTarget(_target);
    }

    public override void DoDie()
    {
        base.DoDie();
        if (_sternEffect != null) { _sternEffect.SetActive(false); }
        //_onSpawn = false;
        SetNoneTargetting();
        SetState(eCharacterStates.Die);
    }

    protected override void DieInit()
    {
        base.DieInit();
        if (_animaController != null)
        {
            _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, true);
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_DIE_1);
        }
        DieInitFunc();
    }

    protected virtual void DieInitFunc()
    {
        if (_movement.enabled)
            _movement.AgentEnabled(false);
        ColliderEnabled(false);
        if (_offTargetDieCall != null)
            _offTargetDieCall();
        testDieInitCheck = true;

        if (GameManager.GetInstance.TestEditor) { if (!GameManager.GetInstance.Production) { SpawnTestManager.GetInstance.GetEnemyRemove(this); } }
        else
        {
            if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
            _stageManager._chapter[_stageManager._cc].GetEnemyRemove(this);
        }
    }

    public virtual void DieFinishCall()
    {
        testDieFinishCheck = true;
        if (_animaController != null)
            _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, false);
        SimplePool.Despawn(gameObject);
    }

    public override void DoHit(UnitDamageData _data)
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_animaController != null)
            if (_animaController.ShortNameHash_0 == CommonStaticDatas.ANIMPARAM_DIE_1)
                return;

        if (_data.attacker != null && IsInDist(_data.attacker, _trans, _enemyStats._AttackDist))
            _target = _data.attacker;

        if (_curState == eCharacterStates.Die)
            testDieAffterHitAnimCheck = true;
    }

    #region KnockBack ==================================================================================
    //protected bool push = false;
    protected override void KnockBackInit()
    {
        if (_animaController != null)
        {
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_KNOCKBACK);
            _statusEffectsController._knockBackOn = true;
            _statusEffectsController._knockBackTime = 0.2f;
        }
    }
    protected override void KnockBackUpdate()
    {
        base.KnockBackUpdate();
        _statusEffectsController._knockBackTime -= Time.deltaTime;
        if (_statusEffectsController._knockBackTime < 0f)
        {
            SetState(eCharacterStates.Idle);
        }
    }
    protected override void KnockBackFinish()
    {
        base.KnockBackFinish();
        _statusEffectsController._knockBackOn = false;
    }

    public void FinishKnockBack()
    {
        _movement.AgentStop(false);
        SetMoveState();
    }

    protected override void KnockDownInit()
    {
        _movement.angularSpeed = 0;
        if (_animaController != null)
        {
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SHOCK);
            _statusEffectsController._shockOn = true;
            _statusEffectsController._shockTime = 2.0f;
        }
    }
    protected override void KnockDownUpdate()
    {
        base.KnockDownUpdate();
        _statusEffectsController._shockTime -= Time.deltaTime;
        if (_statusEffectsController._shockTime < 0f)
        {
            SetState(eCharacterStates.Idle);
        }
    }

    protected override void KnockDownFinish()
    {
        if (_movement.enabled == true)
            _movement.angularSpeed = 1000;
        _statusEffectsController._shockOn = false;
    }

    public void FinishKnockDown()
    {
        _movement.AgentStop(false);
        if (_movement.enabled == true)
        {
            _movement.angularSpeed = 1000;
            _movement.updateRotation = true;
        }
        SetMoveState();
    }


    #endregion ============================================================================================

    #region Shock ==================================================================================
    protected override void ShockInit()
    {
        base.ShockInit();
        _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_SHOCK, true);
        _statusEffectsController._shockOn = true;
        _statusEffectsController._shockTime = 0.2f;
    }
    protected override void ShockUpdate()
    {
        base.ShockUpdate();
        _statusEffectsController._shockTime -= Time.deltaTime;
        if (_statusEffectsController._shockTime < 0f)
        {
            SetState(eCharacterStates.Idle);
        }
    }

    protected override void ShockFinish()
    {
        base.ShockFinish();
        _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_SHOCK, false);
        _statusEffectsController._shockOn = false;
    }
    #endregion Shock ==================================================================================

    #region Stern ==================================================================================
    protected override void SternInit()
    {
        base.SternInit();
        _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_STUN, true);
        _statusEffectsController._sternOn = true;
        _statusEffectsController._sternTime = 0.2f;
        if (_sternEffect == null) { _sternEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_07", _trans, Vector3.zero, Quaternion.identity); }
        if (_sternEffect != null) { _sternEffect.SetActive(true); }
    }
    protected override void SternUpdate()
    {
        base.SternUpdate();
        _statusEffectsController._sternTime -= Time.deltaTime;
        if (_statusEffectsController._sternTime < 0f)
        {
            if (_sternEffect != null) { _sternEffect.SetActive(false); }
            SetState(eCharacterStates.Idle);
        }
    }
    protected override void SternFinish()
    {
        base.SternFinish();
        _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_STUN, false);
        _statusEffectsController._sternOn = false;
    }
    #endregion ============================================================================================

    #region Trace
    protected override void TraceInit()
    {
        _movement.AgentStop(false);
        if (_movement.enabled == true)
            SetSpeed();
        if (_target == null)
        {
            SetState(eCharacterStates.Idle);
            return;
        }
        var targetPos = _target.transform.position;
        targetPos.y = _trans.position.y;
        _movement.SetDestination(targetPos);
        _traceTime = 0f;
    }

    protected override void TraceUpdate()
    {
        if (_movement.enabled == true)
            SetSpeed();
        if (_animaController != null)
            _animaController.VelocityY = _movement.AnimeVelocityYSetting;

        // 최대 체력일때만 재 리스폰
        if (_enemyStats.hp == _enemyStats.maxHp)
        {
            _traceTime += Time.deltaTime;
            if (_traceTime > _traceMaxTime)
            {
                _traceTime = 0;
                // 멈추면서 콜라이더도 꺼준다
                SetNoneTargetting();
                _animator.gameObject.SetActive(false);
                if (GameManager.GetInstance.TestEditor) { }
                else { _trans.position = StageManager._chapter[0].GetRandomPlayerDisPos(); }
                SetState(eCharacterStates.Spawn);
                return;
            }
        }

        flowTraceTime += Time.deltaTime;
        if (0.5f < flowTraceTime)
        {
            TargetCheckFuntion(_enemyStats._TraceRange);
            bool beWhachMyTeam = CheckRoundAndAngle(_trans, tag, 0.52f, 3, 10, _mylayer);
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

        if (CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
            SetState(eCharacterStates.Attack);
    }

    protected override void TraceFinish()
    {
        base.TraceFinish();
        _movement.NavAgentAction(true);
    }
    #endregion

    public void OrderAttack()
    {
        if (gameObject.activeSelf == false)
            return;
        _enemyStats._TraceRange = 500;
    }

    // 도발자를 보게 하기 위해
    float _tempTraceRange = 0;
    public void TargetSetting(Transform target)
    {
        _target = target;
        _tempTraceRange = _enemyStats._TraceRange;
        _enemyStats._TraceRange = 0;
        _statusEffectsController._provocation = true;
        _statusEffectsController._provocationTime = 0f;
        _statusEffectsController._provocationMaxTime = 3f;
    }
    public override void TargetNotting()
    {
        _target = null;
        _enemyStats._TraceRange = _tempTraceRange;
        _tempTraceRange = 0;
    }

    protected bool IsFacingPlayer(float angle)
    {
        if (_target == null) { return false; }
        if (Vector3.Angle(transform.forward, _target.position - transform.position) < angle) { return true; } // facingThreshold
        else { return false; }
    }

    /// <summary>
    /// 빠르게 달리면서 점차적으로 회전하는 함수
    /// </summary>
    protected void TurnAndMoveTowardsTarget()
    {
        if (_target == null) { _target = CheckTraceTarget(_trans, _targetTag, 10000f, _unitLayerMask); }
        Vector3 targetDirection = _target.position - _trans.position;
        targetDirection.y = 0;
        if (_currentTurnSpeed < _maxTurnSpeed)
        {
            _currentTurnSpeed += _turnAcceleration * Time.deltaTime;
            _currentTurnSpeed = Mathf.Min(_currentTurnSpeed, _maxTurnSpeed);
        }
        float step = _currentTurnSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(_trans.forward, targetDirection, step, 0.0f);
        Quaternion newRotation = Quaternion.LookRotation(newDirection);

        _trans.rotation = Quaternion.Slerp(_trans.rotation, newRotation, step / Quaternion.Angle(_trans.rotation, newRotation));
        _trans.position += _trans.forward * (_movement.speed * 2) * Time.deltaTime;
    }
    protected void ToggleAgentControl(bool active)
    {
        _movement.ToggleAgentControl(active);
    }

    /// <summary>
    ///  타겟이 있으면 true 없거나 죽어있으면 false
    /// </summary>
    /// <returns></returns>
    protected bool TargetCheckFuntion(float range = 10000f)
    {
        _target = CheckTraceTarget(_trans, _targetTag, range, _unitLayerMask);
        if (_target == null) { return false; }
        else { if (_target.GetComponent<CharacterController>().IsDie()) { return false; } }
        return true;
    }

    /// <summary>
    ///  range한 거리 만큼 타겟이 있으면 바라본다
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    protected bool FaceTarget(float trunSpeed = 10f, float range = 10000f)
    {
        if (!TargetCheckFuntion(range)) { return false; }

        var targetPos = _target.position;
        targetPos.y = _trans.position.y;

        FaceRotation(_trans, _target, Time.deltaTime * trunSpeed);
        return true;
    }
    /// <summary>
    /// 타겟을 즉시 바라보게 하는 함수
    /// </summary>
    public void WatchTarget()
    {
        if (!TargetCheckFuntion()) { return; }
        StandardFuncUnit.WatchTarget(_trans, _target);
    }

    /// <summary>
    /// 타겟과 어느정도 멀어지면 따라가는 함수 호출
    /// </summary>
    protected void TargetDistanceSetTrace(float value)
    {
        if (!TargetCheckFuntion()) { return; }
        float distance = Vector3.Distance(_trans.position, _target.position);
        if (distance > value) { SetState(eCharacterStates.Trace); }
    }

    /// <summary>
    /// 타겟과 가까워지면 대기 상태
    /// </summary>
    /// <param name="value"></param>
    protected void TargetDistanceSetIdle(float value)
    {
        if (!TargetCheckFuntion()) { return; }
        float distance = Vector3.Distance(_trans.position, _target.position);
        if (distance < value) { SetState(eCharacterStates.Idle); }
    }
    protected void HitPointDistanceSetIdle(float value)
    {
        float distance = Vector3.Distance(_trans.position, _hitPoint);
        if (distance < value) { SetState(eCharacterStates.Idle); }
    }

    /// <summary>
    /// 일정 체력이 빠질시 스킬을 사용 셋팅
    /// </summary>
    protected void HealthSkillSetting(int[] value)
    {
        for (int i = 0; i < value.Length; ++i)
        {
            // 확률로 일정 체력 변환
            _healthThresholds.Add((int)OperatorValue(_enemyStats.maxHp, value[i], OperatorCategory.persent));
        }
    }
    /// <summary>
    /// 전투 중에 스킬을 다시 회복하는 몬스터가 사용 (현무)
    /// </summary>
    /// <param name="value"></param>
    protected void AddSkillSetting(int value)
    {
        if (_healthThresholds.Count == 0)
        {
            _healthThresholds.Add((int)OperatorValue(_enemyStats.maxHp, value, OperatorCategory.persent));
        }
    }
    protected bool CheckHealthAndAttack()
    {
        if (_healthThresholds.Count != 0)
        {
            if (_enemyStats.hp < _healthThresholds[0])
            {
                return true;
            }
        }
        return false;
    }
    protected void AttackAndMoveInit(float attackMin, float attackMax, float moveMin, float moveMax)
    {
        _randomAttackArr[0] = attackMin;
        _randomAttackArr[1] = attackMax;
        _attackTimeMax = UnityEngine.Random.Range(_randomAttackArr[0], _randomAttackArr[1]);
        _randomMoveArr[0] = moveMin;
        _randomMoveArr[1] = moveMax;
        _moveTimeMax = UnityEngine.Random.Range(_randomMoveArr[0], _randomMoveArr[1]);
    }
    /// <summary>
    /// 움직임 체크
    /// </summary>
    protected bool MoveFuntion()
    {
        _moveTime += Time.deltaTime;
        if (_moveTimeMax < _moveTime)
        {
            _moveTime = 0f;
            _moveTimeMax = UnityEngine.Random.Range(_randomMoveArr[0], _randomMoveArr[1]);
            SetState(eCharacterStates.Move);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 공격 체크
    /// </summary>
    protected virtual void AttackFuntion()
    {
        // 일정 체력이 까졌는가 스킬 체크
        if (CheckHealthAndAttack())
        {
            //Debug.Log("스킬 사용");
            _healthThresholds.RemoveAt(0);
            SetState(eCharacterStates.Attack01);
            return;
        }
        else
        {
            _attackTime += Time.deltaTime;
            if (_attackTimeMax < _attackTime)
            {
                _attackTime = 0f;
                _attackTimeMax = UnityEngine.Random.Range(_randomAttackArr[0], _randomAttackArr[1]);

                int randomValue = UnityEngine.Random.Range(0, 100);
                if (randomValue < 40) { _comboIndex = _comboIndexSetting[0]; } // 40
                else if (randomValue < 75) { _comboIndex = _comboIndexSetting[1]; } // 35
                else { _comboIndex = _comboIndexSetting[2]; } // 25

                SetState(eCharacterStates.Attack);
            }
        }
    }

    /// <summary>
    /// 공격거리 체크
    /// </summary>
    /// <param name="range"></param>
    protected void CheckAttackFuntion(float range)
    {
        // 공격 거리 체크
        if (CheckCanAttackAngle(_trans, _targetTag, range, _enemyStats._FindEnemyAngle, _unitLayerMask))
        {
            _bossAttack = false;
        }
        // 공격 거리가 안되면 지속 이동
        else
        {
            if (_animaController.VelocityY < 1f) { _animaController.VelocityY += Time.deltaTime; }
            if (_movement.speed < _enemyStats.traceMoveSpeed * _traceMoveAddSpeed) { _movement.speed += Time.deltaTime * _attackAddSpeed; }
            UnitTraceFuntion();
        }
    }
    void UnitTraceFuntion()
    {
        TargetCheckFuntion();

        if (_movement.enabled == true)
        {
            if (_target != null)
            {
                var targetPos = _target.transform.position;
                targetPos.y = _trans.position.y;

                _movement.SetDestination(targetPos);

                _attackDestinationTime += Time.deltaTime;
                if (_attackDestinationTime >= 2f)
                {
                    _attackDestinationTime = 0f;
                    StartCoroutine(DelayMesh());
                }
            }
            else { SetState(eCharacterStates.Idle); }
        }
    }
    IEnumerator DelayMesh()
    {
        _movement.AgentEnabled(false);
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        _movement.AgentEnabled(true);
    }

    protected void ColliderEnabled(bool enabled)
    {
        _collider.enabled = enabled;
    }
    protected void NotHit()
    {
        _movement.NavAgentAction(false);
        ColliderEnabled(false);
    }
    protected void OnHit()
    {
        _movement.NavAgentAction(true);
        ColliderEnabled(true);
    }
    protected void NotHitPriority()
    {
        _movement.NavAgentAction(false, true);
        ColliderEnabled(false);
    }
    protected void OnHitPriority()
    {
        _movement.NavAgentAction(true, true);
        ColliderEnabled(true);
    }
    protected void LastPositionSetting(Vector3 pos) 
    {
        _idleTimer = 0f;
        _lastPosition = pos; 
    }
    /// <summary>
    /// 움직이는 중 가만히 있게 되면(끼임 버그) 대기형태로 돌아감
    /// </summary>
    protected void CheckIdleState()
    {
        float distanceMoved = Vector3.Distance(_trans.position, _lastPosition);

        if (distanceMoved < _distanceThreshold)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleTimeThreshold)
            {
                SetState(eCharacterStates.Trace); // idle로 해도 안되서 따라가는걸로 변경
                _idleTimer = 0f;
                //Debug.Log("가만히 있어서 변환");
            }
        }
        else { _idleTimer = 0f; }
        _lastPosition = _trans.position;
    }

    /// <summary>
    /// 사운드 플레이
    /// </summary>
    /// <param name="audioID"></param>
    /// <param name="delayTime"></param>
    /// <param name="isFx"></param>
    /// <param name="addChild"></param>
    public void AudioPlayerFuntion(string audioID, float delayTime, bool isFx = true, bool addChild = false)
    {
        if (!string.IsNullOrEmpty(audioID))
        {
            Play(audioID, delayTime, isFx, addChild);
        }
        else { Debug.Log(string.Format("{0} Null", audioID)); }
    }
    private void Play(string audioID, float delayTime, bool isFx = true, bool addChild = false)
    {
        StartCoroutine(PlayCo(audioID, delayTime, isFx, addChild));
    }
    private IEnumerator PlayCo(string audioID, float delayTime, bool isFx = true, bool addChild = false)
    {
        yield return YieldInstructionCache.WaitForSeconds(delayTime);
        if (isFx == false)
        {
            var obj = AudioController.PlayMusic(audioID);
            if (addChild)
            {
                obj.transform.SetParent(transform);
            }
        }
        else
        {
            var obj = AudioController.Play(audioID);
            if (addChild)
            {
                obj.transform.SetParent(transform);
            }
        }
    }

    // 보스 관련 
    public void SetSkillTdd()
    {
        var tdd2 = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_PLAYER),
            targetTag = new string[] { CommonStaticDatas.TAG_PLAYER },
            damage = _enemyStats._enemyData.skill01[0],
            _hitEffectRes = "FX_Gun_Hit"
        };
        _enemyStats.InitUnitDamageData2(tdd2);

        var tdd3 = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_PLAYER),
            targetTag = new string[] { CommonStaticDatas.TAG_PLAYER },
            damage = _enemyStats._enemyData.skill01[1],
            _hitEffectRes = "FX_Gun_Hit"
        };
        _enemyStats.InitUnitDamageData3(tdd3);

        var tdd4 = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_PLAYER),
            targetTag = new string[] { CommonStaticDatas.TAG_PLAYER },
            damage = _enemyStats._enemyData.skill02[0],
            _hitEffectRes = "FX_Gun_Hit"
        };
        _enemyStats.InitUnitDamageData4(tdd4);
    }

    protected IEnumerator DelayAgentEnabled()
    {
        yield return YieldInstructionCache.WaitForEndOfFrame;
        _movement.AgentEnabled(true);
    }

    public GameObject Detecter { get { return _detecter; } }
    StageManager StageManager { get { if (_stageManager == null) { _stageManager = StageManager.GetInstance; } return _stageManager; } }
}


