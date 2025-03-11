using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 고래 스크립트
/*
    공격패턴
    어떠한 공격을 할지 정한다
    공격을 안하고 움직임만 있으면 그대로 진행
    1번 공격 몸통치기
    2번 공격 옆으로 쳐서 물살 일으키기
    3번 공격 꼬리치기 (가까이 가서 딜을 넣어야한다)

    스킬1 필드 위의 모든 유닛을 흡수 후 뿜어내기
*/

public class BossType1Controller : EnemyController
{
    public Absorption _absorption; // 흡수 스킬을 위한 오브젝트
    public GameObject _loopParticle;

    float _time = 0f;
    bool _attack = false;
    bool _trace = false;
    BossSkill _tempSkill = null;
    public int _range = 100;

    bool _onCheckSkill = false;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();

            if (_animaController != null)
            {
                _animaController.AnimEnventSender("MoveColliderOpen", MoveColliderOpen);
                _animaController.AnimEnventSender("MoveColliderClose", MoveColliderClose);
                _animaController.AnimEnventSender("StartSkillEndAnime", StartSkillEndAnime);
                _animaController.AnimEnventSender("LoopSkillStart", LoopSkillStart);
                _animaController.AnimEnventSender("EndSkill", EndSkill);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("AttackColliderOpen", AttackColliderOpen);
                _animaController.AnimEnventSender("AttackColliderClose", AttackColliderClose);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }

            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 2, 1, 0 };
            int[] skillH = new int[] { 80, 50, 30 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(1f, 5f, 1f, 10f);

            _oneOnStartInit = true;
        }
        
        SetState(eCharacterStates.Spawn);

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
        //if (!_onCheckSkill) { _onCheckSkill = true; SetState(eCharacterStates.Attack01); } // 먼저 스킬을 사용하게
        //else { SetState(eCharacterStates.Move); }
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
    }

    protected override void AttackUpdate() 
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0) // 몸통치기
            {
                CheckAttackFuntion(12f);
            }
            else if (_comboIndex == 1) // 옆으로 치기
            {
                CheckAttackFuntion(12f);
            }
            else if (_comboIndex == 2) // 꼬리치기
            {
                CheckAttackFuntion(12f);
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
        _movement.AgentStop(true);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        //Debug.Log("스킬 1 사용");
        // 대기
        StartCoroutine(DelayStartSkill());
    }
    IEnumerator DelayStartSkill()
    {
        yield return YieldInstructionCache.WaitForSeconds(3f);
        if (_curState == eCharacterStates.Die) { yield break; }
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SKILLSTART);
    }

    public void StartSkillEndAnime()
    {
        Vector3 pos;
        // 들어갔다가 다른곳으로 나와야함
        if (GameManager.GetInstance.TestEditor)
        {
            pos = Vector3.zero;
        }
        else
        {
            StageManager stage = StageManager.GetInstance;
            //+- 45정도씩의 차이 4방향
            pos = stage._chapter[stage._cc].gameObject.transform.position;
        }

        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            _trans.position = new Vector3(pos.x, pos.y, pos.z - 45);
            _trans.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (rand == 1)
        {
            _trans.position = new Vector3(pos.x - 45, pos.y, pos.z);
            _trans.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if (rand == 2)
        {
            _trans.position = new Vector3(pos.x, pos.y, pos.z + 45);
            _trans.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else if (rand == 3)
        {
            _trans.position = new Vector3(pos.x + 45, pos.y, pos.z);
            _trans.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
        }
    }

    public void LoopSkillStart()
    {
        _absorption.gameObject.SetActive(true);
        StartCoroutine(DelayLoopSkill());
        _loopParticle.SetActive(true);
    }
    IEnumerator DelayLoopSkill()
    {
        yield return YieldInstructionCache.WaitForSeconds(10f);
        _absorption._endLoopSkill = true;
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ENDSKILL);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        _loopParticle.SetActive(false);
    }
    public void EndSkill()
    {
        // 들어가면서 포지션 재배치 하고 수초후에 맵의 가운데에 분수 뿜어주면서 몬스터들 재생성 해준다
        StartCoroutine(DelayEndSkill());
    }

    IEnumerator DelayEndSkill()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        //obj.transform.position = stage._chapter[stage._cc].transform.position; // 폭발 이펙트 자리 선정
        yield return YieldInstructionCache.WaitForSeconds(1f);
        _absorption.gameObject.SetActive(false); // 여기서 재배치가 일어남
        yield return YieldInstructionCache.WaitForSeconds(4f);
        if (GameManager.GetInstance.TestEditor) { this.transform.position = SpawnTestManager.GetInstance.GetBossPos(); }
        else
        {
            if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
            this.transform.position = _stageManager._chapter[_stageManager._cc].GetBossPos();
        }
        
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SPAWN);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_SKILLSTART);
    }
    
    public override void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        SetState(eCharacterStates.Move);
        if (_movement != null) { _movement.AgentEnabled(true); }
    }
    public void MoveColliderOpen()
    {
        _movement.NavAgentAction(true);
        AttackColliderOpen();
    }
    public void MoveColliderClose()
    {
        _movement.NavAgentAction(false);
        AttackColliderClose();
    }
    public void AttackColliderOpen()
    {
        _collider.enabled = true;
    }
    public void AttackColliderClose()
    {
        _collider.enabled = false;
    }
    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type1/Boss_Whale_Wave_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type1/Boss_Whale_Wave_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossPushAttack>().InitData(_enemyStats, _enemyStats.GetTDD2());
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type1/Boss_Whale_Wave_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD3());
    }
}