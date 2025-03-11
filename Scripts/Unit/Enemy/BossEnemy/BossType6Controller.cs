using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 뱀장어 스크립트
/*
기본 공격
원거리 독 공격 - 입에서 나오는 독샘을 전방에 뿌린다
직선으로 몸 내려치기 - 큰 몸을 내려쳐서 파도를 일으킨다
꼬리 치기 - 가볍게 꼬리를 쳐서 전방 데미지

스킬
위를 보며 독샘을 부리고 시간이 지나면 랜덤으로 하늘에서 독이 떨여진다
 */
public class BossType6Controller : EnemyController
{
    float _attackSkillWanderRadius = 10f;
    bool _skillOn = false;

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
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("Attack1Effect", Attack1Effect);
                _animaController.AnimEnventSender("AttackPositionSetting", AttackPositionSetting);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
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

    public void AttackPositionSetting()
    {
        OnHit();
        if (_skillOn) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01); _skillOn = false; }
        else { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK); }

        if (_target == null) { return; }

        _trans.position = StandardFuncUnit.NavMeshPositionRandom(_target, _attackSkillWanderRadius);
        _trans.LookAt(_target);
        //_down = false;
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

        _animaController.VelocityY = 0f;
        _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex);
        if (_comboIndex != 0) { NotHit(); _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_CASTING);}
    }

    protected override void AttackUpdate()
    {
        if (_comboIndex != 0) { return; }
        if (_bossAttack)
        {
            if (_comboIndex == 0)
            {
                CheckAttackFuntion(14f);
            }

            if (!_bossAttack)
            {
                _movement.NavAgentAction(false);
                _movement.AgentStop(true);
                WatchTarget();
                if (_animaController != null)
                {
                    _animaController.VelocityY = 0f;
                    _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
                }
            }
        }
    }
    protected override void AttackFuntion()
    {
        // 일정 체력이 까졌는가 스킬 체크
        if (CheckHealthAndAttack())
        {
            //Debug.Log("스킬 사용");
            _healthThresholds.RemoveAt(0);
            _skillOn = true;
            NotHit();
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_CASTING);
            //_animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
            //SetState(eCharacterStates.Attack01);
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
    public void Attack_01_EffectPos(Vector3 pos)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_01_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(pos.x, 0f, pos.z);
        obj.transform.forward = _trans.forward;
    }
    public void Attack_02_EffectPos(Vector3 pos)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_02_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(pos.x, 0f, pos.z);
        obj.transform.forward = _trans.forward;
    }
    public void Attack_03_EffectPos(Vector3 pos)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_03_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(pos.x, 0f, pos.z);
        obj.transform.forward = _trans.forward;
    }

    
    protected override void Attack01Init()
    {
        _movement.AgentStop(true);
        NotHit();
        WatchTarget();
        //if (_animaController != null)
        //{
        //    _animaController.VelocityY = _movement.AnimeVelocityYSetting;
        //    _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        //}
    }

    public override void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_CASTING);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _canRotateInAttack = false;
        OnHit();
        SetState(eCharacterStates.Idle);
    }

    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossPushAttack>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossBoxAttackEffect ef = obj.transform.Find("Collider").GetComponent<BossBoxAttackEffect>();
        ef.InitData(_enemyStats, _enemyStats.GetTDD2());
        ef.DownSpeed = true;
        ef.DownTime = 1f;
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD3());
    }
    public void Attack1Effect()
    {
        // 스킬을 사용하면 랜덤으로 비가 내리듯 오브젝트가 떨어져야한다
        // 이걸 하기 위해서는 일단 메인이 되는 것을 위에 처럼 생성만 해두고 생성된 오브젝트가 없어졌다고 알게 되면 이 생성된 오브젝트로 없어지는 걸로
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack1", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.GetComponent<BossType6AttackSkill>().InitData(_enemyStats, _enemyStats.GetTDD4());
        obj.GetComponent<BossType6AttackSkill>().StartAttack();
    }

    public override void DoDie()
    {
        base.DoDie();
        SoundManager.GetInstance.PlayAudioEffectSound("BossCrab_Dead");
        //Play("BossCrab_Dead", 0f);
    }
}