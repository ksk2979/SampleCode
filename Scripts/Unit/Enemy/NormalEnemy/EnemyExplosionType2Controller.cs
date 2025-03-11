using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static StandardFuncUnit;

// 보스 유형의 날치 (폭파)
public class EnemyExplosionType2Controller : EnemyController
{
    public string effectName = string.Empty;
    public bool addChild = false;
    public Transform perent;
    private GameObject obj;
    bool _explosion = false; // 한번 폭파하면 끝

    float _mobTime = 0f;
    bool _attackSpawn = false;

    float _dieTime = 2f;
    [SerializeField] Transform _spawnPos;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("OnEffectDream", OnEffectDream);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            _oneOnStartInit = true;
        }

        _explosion = false;
        _mobTime = 0f;
        SetState(eCharacterStates.Spawn);
        if (_spawnPos == null) { _spawnPos = _stageManager.GetPlayers()[0].transform; }
    }

    // 죽으면 터지게
    public void OnEffectDream()
    {
        _explosion = true;
        obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, effectName, Vector3.zero, Quaternion.identity);
        obj.transform.position = perent.position;
        obj.transform.forward = perent.forward;
        if (addChild)
        {
            obj.transform.SetParent(perent);
        }
        obj.GetComponent<Explosion>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }

    private void Update()
    {
        if (_attackSpawn)
        {
            _mobTime += Time.deltaTime;
            if (_mobTime > 1f)
            {
                for (int i = 0; i < 1; ++i)
                {
                    if (GameManager.GetInstance.TestEditor) { SpawnTestManager.GetInstance.BossType2CreateEnemy(10001, _trans); }
                    else { if (_spawnPos != null) { _stageManager._chapter[_stageManager._cc].CreateForcedSpawnEnemy(10001, _spawnPos, _enemyStats); } else { _stageManager._chapter[_stageManager._cc].CreateForcedSpawnEnemy(10001, _trans, _enemyStats); } }
                }
                _mobTime = 0f;
            }
        }
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
            SetState(eCharacterStates.Trace);
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
            if (CheckCanAttackAngle(_trans, _targetTag, _enemyStats._AttackRange, _enemyStats._FindEnemyAngle, _unitLayerMask))
                SetState(eCharacterStates.Trace);
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
        { _attackSpawn = true; }
        else { _attackSpawn = false; }
    }

    protected override void DieInit()
    {
        //if (_animaController != null)
        //{
        //    _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, true);
        //    _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_DIE_1);
        //}
        _dieTime = 2f;
        DieInitFunc();
        OnEffectArea();
    }

    public void OnEffectArea()
    {
        obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/NormalExplosionArea", Vector3.zero, Quaternion.identity);
        obj.transform.position = perent.position;
        obj.transform.forward = perent.forward;
    }

    // 2초후 폭파
    protected override void DieUpdate()
    {
        _dieTime -= Time.deltaTime;
        if (_dieTime < 0f)
        {
            OnEffectDream();

            testDieFinishCheck = true;
            if (GameManager.GetInstance.TestEditor) { if (!GameManager.GetInstance.Production) { SpawnTestManager.GetInstance.GetEnemyRemove(this); } }
            else
            {
                if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
                _stageManager._chapter[_stageManager._cc].GetEnemyRemove(this);
            }
            if (_animaController != null)
                _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, false);
            SimplePool.Despawn(gameObject);
        }
    }
}