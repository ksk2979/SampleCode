using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 꽃게 스크립트
/*
원거리 공격가능한 물뿜기
콤포 3종류가 있다
 */
public class BossType3Controller : EnemyController
{
    public Transform _skillPoint;

    bool _attack3 = false;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("EndSkill", EndSkill);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("Attack1EffectPos", Attack1EffectPos);
                _animaController.AnimEnventSender("Attack3Start", Attack3Start);
                _animaController.AnimEnventSender("Attack3End", Attack3End);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 1, 0, 2 };
            int[] skillH = new int[] { 80, 50, 30 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 5f, 1f, 10f);

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
        _attack3 = false;
        _movement.AgentEnabled(false);
        StartCoroutine(DelayAgentEnabled());
    }

    protected override void AttackUpdate()
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0) // 범위 내려치기
            {
                CheckAttackFuntion(10f);
            }
            else if (_comboIndex == 1) // 앞 내려치기
            {
                CheckAttackFuntion(8f);
            }
            else if (_comboIndex == 2) // 돌기
            {
                CheckAttackFuntion(10f);
            }

            if (!_bossAttack)
            {
                //_bossSkills.GetSkill(eCharacterStates.Attack).ResetTime();
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
            return;
        }

        if (_attack3)
        {
            if (_target != null)
            {
                _movement.speed = _enemyStats.traceMoveSpeed * 1.2f;
                var targetPos = _target.transform.position;
                targetPos.y = _trans.position.y;
                _movement.SetDestination(targetPos);
            }
        }
    }

    protected override void Attack01Init()
    {
        //_bossSkills.GetSkill(eCharacterStates.Attack01).ResetTime();
        WatchTarget();
        _movement.AgentStop(true);
        _movement.NavAgentAction(false);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
    }

    public void EndSkill()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _onSound = false;
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _canRotateInAttack = false;
        _movement.NavAgentAction(true);
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
        _movement.NavAgentAction(true);
        SetState(eCharacterStates.Idle);
    }

    // 범위딜
    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type3/Boss_Crap_Attack_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    // 일자딜
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type3/Boss_Crap_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossBoxAttackEffect ef = obj.transform.Find("Collider").GetComponent<BossBoxAttackEffect>();
        ef.InitData(_enemyStats, _enemyStats.GetTDD2());
        ef.DownSpeed = true;
        ef.DownTime = 1f;
    }
    // 회전 돌기
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type3/Boss_Crap_Attack_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossContinueAttack>().InitData(_enemyStats, _enemyStats.GetTDD3());
    }
    // 물뿌리기
    public void Attack1EffectPos()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type3/Boss_Crap_WaterCannon", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(_skillPoint.position.x, 0f, _skillPoint.position.z);
        obj.transform.rotation = Quaternion.Euler(0f, _skillPoint.eulerAngles.y, 0f);
        obj.transform.Find("Collider").GetComponent<BossPushAttack>().InitData(_enemyStats, _enemyStats.GetTDD4());
        if (!_onSound) { _onSound = true; AudioPlayerFuntion("BossCrab_Skill", 0f); }
    }

    // 추적한다!
    public void Attack3Start()
    {
        _movement.NavAgentAction(true);
        _movement.AgentStop(false);
        _attack3 = true;
    }
    // 끝난다!
    public void Attack3End()
    {
        _movement.speed = 0f;
        _attack3 = false;
    }

    public override void DoDie()
    {
        base.DoDie();
        if (_attack3) { _movement.speed = 0f; }
        _attack3 = false;
        AudioPlayerFuntion("BossCrab_Dead", 0f);
    }
}
