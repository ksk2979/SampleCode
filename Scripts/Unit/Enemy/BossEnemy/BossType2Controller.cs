using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// 아귀 스크립트
/*
물에 들어가있는 동안 엘리트 몬스터를 소환하는 유인 스킬을 사용한다
공격은 3개를 사용
 */
public class BossType2Controller : EnemyController
{
    public GameObject _loopEffectObj;

    [SerializeField] float _playerWanderRadius = 50f;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();

            if (_animaController != null)
            {
                _animaController.AnimEnventSender("MovementOnEnabled", MovementOnEnabled);
                _animaController.AnimEnventSender("EndSkill", EndSkill);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 2, 0, 1 }; // 머리치기/기습/몸덮치기
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
    }

    protected override void AttackUpdate()
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0) // 기습
            {
                CheckAttackFuntion(50f);
            }
            else if (_comboIndex == 1) // 몸통치기
            {
                CheckAttackFuntion(8f);
            }
            else if (_comboIndex == 2) // 머리치기
            {
                CheckAttackFuntion(10f);
            }

            if (!_bossAttack)
            {
                //_bossSkills.GetSkill(eCharacterStates.Attack).ResetTime();
                // 아귀의 공격0은 숨어있다가 팡 치는 거기 때문에 딜레이를 준다
                if (_comboIndex == 0) { StartCoroutine(AttackDelayFuntion()); }
                // 공격속도가 빠르고 범위도 넓어서 잠시 공격전 딜레이를 준다
                else if (_comboIndex == 1) { StartCoroutine(AttackIndex1Funtion()); return; }

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

    public void MovementOnEnabled() { _movement.AgentEnabled(true); }
    IEnumerator AttackDelayFuntion()
    {
        // 숨어있다가
        _movement.AgentEnabled(false);
        NotHit();
        yield return YieldInstructionCache.WaitForSeconds(1f);
        // 메인 보트에 포지션 지정하고 범위 이펙트 보여준다
        Vector3 pos;
        if (GameManager.GetInstance.TestEditor) { pos = SpawnTestManager.GetInstance.GetPlayerList()[0].transform.position; }
        else { pos = _stageManager.PlayerStat.transform.position; }
        Attack1EffectPos(pos);
        this.transform.position = pos;
        yield return YieldInstructionCache.WaitForSeconds(1f);
        // 애니 돌리고 종료
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACKCOMBO0);
    }
    IEnumerator AttackIndex1Funtion()
    {
        _movement.AgentStop(true);
        WatchTarget();
        _animator.GetComponent<PawnAddEffect>().OnPawnAddEffect(PawnAddEffect.EffectState.casting);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        if (_animaController != null)
        {
            _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex);
            _animaController.VelocityY = 0f;
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        }
    }

    protected override void Attack01Init()
    {
        _movement.AgentStop(true);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        StartCoroutine(DelaySkillAction());
    }
    
    IEnumerator DelaySkillAction()
    {
        // 여기서는 이제 데미지를 줘서 나오게하거나 아니면 시간이 지나면 나오게 설정
        AudioPlayerFuntion("BossAnglerfish_skill01", 0f);
        _loopEffectObj.SetActive(true);
        bool endSkill = false;
        float time = 0f;
        float soundTime = 0f;
        float mobTime = 0f;
        int mobCount = 0; // 몹 카운터 50이상이되면 나오기
        Transform player;
        if (GameManager.GetInstance.TestEditor) { player = SpawnTestManager.GetInstance.GetPlayerList()[0].transform; }
        else { player = _stageManager.PlayerStat.transform; }
        while (!endSkill)
        {
            if (_curState == eCharacterStates.Die) { endSkill = true; }
            time += Time.deltaTime;
            soundTime += Time.deltaTime;
            mobTime += Time.deltaTime;
            yield return YieldInstructionCache.WaitForFixedUpdate;
            if (mobTime > 1f) {
                for (int i = 0; i < 5; ++i) {
                    if (GameManager.GetInstance.TestEditor) { SpawnTestManager.GetInstance.BossType2CreateEnemy(RandomID(), player); }
                    else { _stageManager._chapter[_stageManager._cc].CreateForcedSpawnEnemy(RandomID(), player, _enemyStats); }
                    mobCount++;
                }
                mobTime = 0f;
            }
            if (soundTime > 2f) { soundTime = 0f; AudioPlayerFuntion("BossAnglerfish_skill01", 0f); }
            if (mobCount > 50) { endSkill = true; }
            if (time > 9f) { endSkill = true; }
        }

        if (_curState == eCharacterStates.Die)
        {
            _loopEffectObj.SetActive(false);
            yield break;
        }
        // 스킬을 끝낸다
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ENDSKILL);
        _loopEffectObj.SetActive(false);
    }

    int RandomID()
    {
        int id = 10001;
        return id;
    }
    void EndSkill()
    {
        _movement.AgentStop(false);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _canRotateInAttack = false;
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
        SetState(eCharacterStates.Move);
    }

    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type2/Boss_Blackmouth_Angler_ATTACK1", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type2/Boss_Blackmouth_Angler_ATTACK2", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD2());
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type2/Boss_Blackmouth_Angler_ATTACK3", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD3());
    }
    public void Attack1EffectPos(Vector3 pos)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type2/Boss_Whale_Wave_03_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(pos.x, 0f, pos.z);
    }

    public override void DoDie()
    {
        base.DoDie();
        AudioPlayerFuntion("BossAnglerfish_death01", 0f);
    }
}
