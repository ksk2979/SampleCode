using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossType5Controller : EnemyController
{
    // 콤보는 기본 공격을 이행 할 수 있게 해주는 것
    /*
공격1 - 1발짜리 전기공 발사
공격2 - 2발짜리 전기공 발사
공격3 - 1발짜리 터지는 곡사 전기공 발사
스킬 - 광역 전기 장판
     */

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("OnHit", OnHit);
                _animaController.AnimEnventSender("NotHit", NotHit);
                _animaController.AnimEnventSender("EndSkill", EndSkill);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("TeleportSkill", TeleportSkill);
                _animaController.AnimEnventSender("Attack01Skill", Attack01Skill);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
            int[] skillH = new int[] { 80, 50, 30, 10 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 5f, 1f, 10f);
            _distanceF = 20f; // 기본 사거리를 늘린다

            _oneOnStartInit = true;
        }

        SetState(eCharacterStates.Spawn);
        _movement.angularSpeed = 120; // 회전값 리 초기화
    }

    protected override void SpawnInit()
    {
        m_isSpawn = true;
        m_idleTime = 0;
        SetNoneTargetting();
        _movement.AgentStop(true);
        _movement.NavAgentAction(false);
        WatchTarget();
        _animaController.VelocityY = 0f;
    }

    protected override void SpawnFinishCall()
    {
        OnHit();
        SetState(eCharacterStates.Idle);
    }
    // 대기 하고 있다가 어슬렁 거리게 무브 함수 호출 해주어야함
    protected override void IdleInit() { }
    
    protected override void IdleUpdate() 
    {
        if (_curState == eCharacterStates.Die) { return; }
        if (_movement.IsMove)
        {
            if (_animaController.VelocityY > 0f) { _animaController.VelocityY -= Time.deltaTime; }
            if (_movement.speed > 0f) { _movement.speed -= Time.deltaTime * 2f; }
            else { _movement.speed = 0f; _movement.IsMove = false; }
        }

        // 지속적으로 바라보면서 할 수있는 행동을 탐색한다
        FaceTarget(5f);
        // 거리가 어느정도 멀어지면 move함수 호출
        if (_target != null) { TargetDistanceSetTrace(_distanceF); }
        // 움직임 처리 함수
        if (MoveFuntion()) { return; }
        // 공격 처리 함수
        AttackFuntion();
    }

    protected override void IdleFinish() { }

    protected override void MoveInit()
    {
        _movement.AgentStop(false);
        if (_movement.enabled == true)
        {
            SetSpeed();
            _hitPoint = _randomPos * 1.5f;
            _movement.SetDestination(_hitPoint);
            _movement.speed = 0f;
            LastPositionSetting(_trans.position);
        }

        _movement._oldFwd = _trans.forward;
        _movement.IsMove = true;
    }

    
    protected override void MoveUpdate()
    {
        if (_movement.enabled == true)
        {
            if (_curState == eCharacterStates.Die) { return; }
            if (_animaController.VelocityY < 1f) { _animaController.VelocityY += Time.deltaTime; }
            if (_movement.speed < _enemyStats.moveSpeed) { _movement.speed += Time.deltaTime * 1.5f; }

            CheckIdleState();
            HitPointDistanceSetIdle(2f);
        }
    }

    protected override void MoveFinish() { base.MoveFinish(); }
    protected override void TraceInit()
    {
        if (_target == null) { SetState(eCharacterStates.Idle); return; }
        _movement.AgentStop(false);
        if (_movement.enabled == true)
        {
            SetSpeed();
            _hitPoint = _target.position;// _randomPos;
            _movement.SetDestination(_hitPoint);
            _movement.speed = 0f;
        }

        _movement._oldFwd = _trans.forward;

        _animaController.VelocityY = 0f;
        _movement.IsMove = true;
    }
    protected override void TraceUpdate()
    {
        if (_movement.enabled == true)
        {
            if (_curState == eCharacterStates.Die) { return; }
            if (_animaController.VelocityY < 1f) { _animaController.VelocityY += Time.deltaTime; }
            if (_movement.speed < _enemyStats.traceMoveSpeed) { _movement.speed += Time.deltaTime * _traceAddSpeed; }

            // 지속적으로 따라간다
            if (_target != null)
            {
                _movement.SetDestination(_target.position);
                // 따라가는데 거리가 가까워 지면 대기상태로
                TargetDistanceSetIdle(_distanceF);
            }

            AttackFuntion();
        }
    }

    protected override void TraceFinish() { base.TraceFinish(); }

    protected override void AttackInit()
    {
        _bossAttack = true;
        _movement.AgentEnabled(false);
        StartCoroutine(DelayAgentEnabled());
    }

    protected override void AttackUpdate()
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0)
            {
                CheckAttackFuntion(20f);
            }
            else if (_comboIndex == 1)
            {
                CheckAttackFuntion(20f);
            }
            else if (_comboIndex == 2)
            {
                CheckAttackFuntion(20f);
            }

            if (!_bossAttack)
            {
                _movement.NavAgentAction(false);
                _movement.AgentStop(true);
                WatchTarget();
                if (_animaController != null)
                {
                    _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // 공격 스타일 중에 어떤 공격을 할지 선택  _comboIndex
                    _animaController.VelocityY = 0f;
                    _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
                }
            }
        }
    }

    protected override void Attack01Init()
    {
        _movement.AgentStop(true);
        NotHit();
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
    }

    public void TeleportSkill()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            _trans.position = _target.position + new Vector3(Random.Range(1, 7), 0, Random.Range(1, 7));
        }
        else
        {
            _trans.position = _target.position - new Vector3(Random.Range(1, 7), 0, Random.Range(1, 7));
        }
        
    }

    public void Attack01Skill()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type5/Boss_JellyFish_ATTACK_04", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossRadiusUpAttack longAttack = obj.GetComponent<BossRadiusUpAttack>();
        longAttack.InitData(_enemyStats, _enemyStats.GetTDD4());
        longAttack.DownSpeed = true;
        longAttack.DownTime = 0.3f;
    }

    public void EndSkill()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _canRotateInAttack = false;
        _movement.NavAgentAction(true);
        OnHit();
        SetState(eCharacterStates.Idle);
    }

    public override void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        OnHit();
        SetState(eCharacterStates.Idle);
    }

    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type5/Boss_JellyFish_ATTACK_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossLongAttack longAttack = obj.GetComponent<BossLongAttack>();
        longAttack.InitData(_enemyStats, _enemyStats.GetTDD1());
        longAttack.DownSpeed = true;
        longAttack.DownTime = 0.3f;
        longAttack._targetPos = _trans.TransformPoint(Vector3.forward * Random.Range(20, 30));
        AudioPlayerFuntion("BossJellyfish_Attack1", 0f);
        AudioPlayerFuntion("BossJellyfish_Attack_Hit", 2f);
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type5/Boss_JellyFish_ATTACK_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossLongAttack longAttack = obj.GetComponent<BossLongAttack>();
        longAttack.InitData(_enemyStats, _enemyStats.GetTDD2());
        longAttack.DownSpeed = true;
        longAttack.DownTime = 0.3f;
        longAttack._targetPos = _trans.TransformPoint(Vector3.forward * Random.Range(20, 30));
        AudioPlayerFuntion("BossJellyfish_Attack2", 0f);
        AudioPlayerFuntion("BossJellyfish_Attack_Hit", 2f);
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type5/Boss_JellyFish_ATTACK_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossParabolaAttack longAttack = obj.GetComponent<BossParabolaAttack>();
        longAttack.InitData(_enemyStats, _enemyStats.GetTDD3());
        longAttack.SetData(_trans.position, _target.position);
        AudioPlayerFuntion("BossJellyfish_Attack3", 0f);
        AudioPlayerFuntion("BossJellyfish_Attack_Hit", 2f);
    }

    public override void DoDie()
    {
        base.DoDie();
        AudioPlayerFuntion("BossJellyfish_Dead", 0f);
    }
}
