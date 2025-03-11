using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// 상어
public class BossType4Controller : EnemyController
{
    public GameObject _skillAttack;

    // 회전 회오리! 스킬 이펙트 켜주기
    public GameObject _shotSkillEffect;

    bool _targetShot = false;
    public float _skillTime = 0.07f;
    public float _skillRange = 27f;

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
                _animaController.AnimEnventSender("ShotSkillAttack", ShotSkillAttack);
                _animaController.AnimEnventSender("EndShot", EndShot);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
            int[] skillH = new int[] { 80, 50, 30 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 3f, 1f, 10f);

            _oneOnStartInit = true;
        }
        
        SetState(eCharacterStates.Spawn);
        if (_skillAttack != null) { _skillAttack.GetComponent<BossContinueAttack>().InitData(_enemyStats, _enemyStats.GetTDD4()); }
        _movement.angularSpeed = 120; // 회전값 리 초기화
        _traceAddSpeed = 3f;
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
        _attackDestinationTime = 0f;
    }

    protected override void AttackUpdate()
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0)
            {
                CheckAttackFuntion(8f);
            }
            else if (_comboIndex == 1)
            {
                CheckAttackFuntion(8f);
            }
            else if (_comboIndex == 2)
            {
                CheckAttackFuntion(10f);
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
            return;
        }
    }

    protected override void Attack01Init()
    {
        WatchTarget();
        _movement.AgentStop(true);
        _movement.NavAgentAction(false);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
    }

    public void EndSkill()
    {
        _targetShot = false;
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _canRotateInAttack = false;
        _movement.NavAgentAction(true);
        SetState(eCharacterStates.Move);
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

    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type4/Boss_Shark_Attack_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type4/Boss_Shark_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossAttackEffect ef = obj.transform.Find("Collider").GetComponent<BossAttackEffect>();
        ef.InitData(_enemyStats, _enemyStats.GetTDD2());
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type4/Boss_Shark_Attack_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD3());
    }
    // 앞으로 돌진!
    Vector3 _skillLastPos;
    public void ShotSkillAttack()
    {
        if (_skillAttack != null) { _skillAttack.SetActive(true); }
        if (_shotSkillEffect != null) { _shotSkillEffect.SetActive(true); }
        _skillLastPos = _trans.TransformPoint(Vector3.forward * _skillRange);
        StartCoroutine(TargetShotAttack());
    }
    
    IEnumerator TargetShotAttack()
    {
        _targetShot = true;
        while (_targetShot)
        {
            if (_target != null) { WatchTarget(); _trans.position = Vector3.MoveTowards(_trans.position, _target.position, _skillTime * Time.deltaTime); } // _skillLastPos _trans.position = Vector3.Lerp(_trans.position, _target.position, _skillTime);
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
    }

    public void EndShot()
    {
        if (_skillAttack != null) { _skillAttack.SetActive(false); }
        if (_shotSkillEffect != null) { _shotSkillEffect.SetActive(false); }
        OnHit();
    }

    public override void DoDie()
    {
        base.DoDie();
        AudioPlayerFuntion("BossShark_Dead", 0f);
        if (_skillAttack != null) { _skillAttack.SetActive(false); }
        if (_shotSkillEffect != null) { _shotSkillEffect.SetActive(false); }
        _targetShot = false;
    }
}