using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 가오리 스크립트
/*
기본 
꼬리 찍기 - 꼬리로 전방 찍기
물어뜯기 - 전방 물어뜯기
전방 돌진 - 전방으로 몸통박치기

스킬
투명화 - 몇초간 투명화 상태 되어 인지 불가한 상태에서 공격
 */
public class BossType7Controller : EnemyController
{
    [SerializeField] BossContinueAttack _attack03; // 돌진 스킬

    float _attack03Time = 0f;
    bool _attack03B = false;

    bool _invisibility = false; // 투명화 상태인가
    float _invisibilityTime = 0f;
    [SerializeField] float _attack3Speed = 30f;

    protected readonly string SKILLALPHA = "_Alpha";

    float _skillTime = 0f;
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
                _animaController.AnimEnventSender("AttackEffect3End", AttackEffect3End);
                _animaController.AnimEnventSender("Attack1Effect", Attack1Effect);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
            int[] skillH = new int[] { 80, 50, 30 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 4f, 1f, 10f);

            _oneOnStartInit = true;
        }
        _attack03.InitData(_enemyStats, _enemyStats.GetTDD3());
        SetState(eCharacterStates.Spawn);
        _movement.angularSpeed = 120; // 회전값 리 초기화
        _skillTime = 0f;
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

    private void Update()
    {
        if (_invisibility)
        {
            _invisibilityTime += Time.deltaTime;
            if (_invisibilityTime > 10f)
            {
                _invisibilityTime = 0f;
                InvisibilityEnd();
                return;
            }
            return;
        }
        if (_skillOn) { return; }
        _skillTime += Time.deltaTime;
        if (_skillTime > 10f)
        {
            _skillTime = 0;
            _skillOn = true;
            InvisibilityStart();
        }
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

    protected override void AttackFuntion()
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

    protected override void TraceFinish() { base.TraceFinish(); }

    protected override void AttackInit()
    {
        _bossAttack = true;
        _movement.AgentEnabled(false);
        StartCoroutine(DelayAgentEnabled());
        if (_comboIndex == 2) 
        {
            _movement.NavAgentAction(false);
            _movement.AgentStop(true);
            WatchTarget();
            _attack03B = true;
            _attack03Time = 0f;
            _animaController.VelocityY = 0f;
            _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // 공격 스타일 중에 어떤 공격을 할지 선택  _comboIndex
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        }
    }

    protected override void AttackUpdate()
    {
        if (_attack03B)
        {
            if (_attack03.gameObject.activeSelf && _attack03B)
            {
                _attack03Time += Time.deltaTime;
                if (_attack03Time > 1f) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SHOOT); _attack03B = false; }
                _trans.position += _trans.forward * (Time.deltaTime * _attack3Speed);
                //_trans.Translate(_trans.forward * Time.deltaTime * (_movement.speed * 10f), Space.World);
            }
            return;
        }
        if (_bossAttack)
        {
            if (_comboIndex == 0)
            {
                CheckAttackFuntion(10f);
            }
            else if (_comboIndex == 1)
            {
                CheckAttackFuntion(10f);
            }
            else if (_comboIndex == 2)
            {
                CheckAttackFuntion(15f);
            }

            if (!_bossAttack)
            {
                if (_invisibility)
                {
                    if (_target != null) { _trans.position = StandardFuncUnit.NavMeshPositionRandom(_target, 10f); }
                }
                _movement.NavAgentAction(false);
                _movement.AgentStop(true);
                WatchTarget();
                if (_animaController != null)
                {
                    _animaController.VelocityY = 0f;
                    _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // 공격 스타일 중에 어떤 공격을 할지 선택  _comboIndex
                    _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
                }
            }
        }
    }
    public void AttackLineEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Attack_01_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
    }
    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Attack_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackLineEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Attack_02_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        BossAttackEffect ef = obj.transform.Find("Collider").GetComponent<BossAttackEffect>();
        ef.InitData(_enemyStats, _enemyStats.GetTDD2());
        ef.DownSpeed = true;
        ef.DownTime = 1f;
    }
    public void AttackLineEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Attack_03_Area", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
    }
    public void AttackEffect3()
    {
        if (_attack03 != null) { _attack03.gameObject.SetActive(true); }
    }
    public void AttackEffect3End()
    {
        if (_attack03 != null) { _attack03.gameObject.SetActive(false); }
    }
    public void SpeedEffect()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type7/Boss_Stingray_Spd", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
    }

    void InvisibilityStart()
    {
        Attack1Effect();
        _invisibility = true;
        NotHit();
    }
    void InvisibilityEnd()
    {
        Attack1EffectEnd();
        _invisibility = false;
        _skillOn = false;
        OnHit();
    }

    public void Attack1Effect()
    {
        // 투명 스킬 사용
        _meshrender.material.SetFloat(SKILLALPHA, 1f);
        StartCoroutine(DelayColorDown());
    }
    IEnumerator DelayColorDown()
    {
        float a = 1f;
        
        while (a > 0)
        {
            a -= Time.deltaTime * 3f;
            _meshrender.material.SetFloat(SKILLALPHA, a);
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        a = 0;
        _meshrender.material.SetFloat(SKILLALPHA, a);
    }
    public void Attack1EffectEnd()
    {
        // 투명 스킬 해제
        _meshrender.material.SetFloat(SKILLALPHA, 0f);
        StartCoroutine(DelayColorUp());
    }
    IEnumerator DelayColorUp()
    {
        float a = 0f;

        while (a < 1)
        {
            a += Time.deltaTime * 3f;
            _meshrender.material.SetFloat(SKILLALPHA, a);
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        a = 1f;
        _meshrender.material.SetFloat(SKILLALPHA, a);
    }

    protected override void Attack01Init()
    {
        //StartCoroutine(InvisibilityTime());
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
        if (!_invisibility) { OnHit(); _movement.NavAgentAction(true); }
        else { _movement.NavAgentAction(false); }
        SetState(eCharacterStates.Idle);
    }

    public override void DoDie()
    {
        base.DoDie();
        SoundManager.GetInstance.PlayAudioEffectSound("BossCrab_Dead");
        //Play("BossCrab_Dead", 0f);
    }
}
