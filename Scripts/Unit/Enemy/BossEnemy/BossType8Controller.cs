using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 드래곤
/*
기술
전방에 범위형 물폭탄
날개를 힘차게 일으켜 파도를 보낸다
물속에 살짝 들어가서 몸을 움축시켜서 전방으로 고속 이동 공격

스킬
하늘로 날라 튜토리얼처럼 화면에 지나가면서 광역으로 여러개의 용오름을 만들어 낸다

움직이고 있다가 공격을 시작할때는 유저를 향해 돌아준다
돌아준 다음 공격 사거리 만큼 이동한다
공격 전 범위 이펙트 나오고 난 후 공격을 시행한다 
 */
public class BossType8Controller : EnemyController
{
    [SerializeField] BossContinueAttack _attack03; // 돌진 스킬
    bool _isAttack03 = false;
    [SerializeField] Transform[] _skillPoint;

    [SerializeField] float _attack3Speed = 30f;

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
                _animaController.AnimEnventSender("Attack01Effect", Attack01Effect);
                _animaController.AnimEnventSender("Attack03Line", Attack03Line);
                _animaController.AnimEnventSender("Attack03StartMove", Attack03StartMove);
                _animaController.AnimEnventSender("Attack03EndMove", Attack03EndMove);
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
        _attack03.InitData(_enemyStats, _enemyStats.GetTDD3());
        SetState(eCharacterStates.Spawn);
        _movement.angularSpeed = 120; // 회전값 리 초기화
    }

    //private void Update()
    //{
    //    // 임의로 생성
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        // 여기 _temp가 없어 터진다
    //        _tempSkill._state = eCharacterStates.Attack;
    //        _hitPoint = _target.position; _attack = true; _comboIndex = 0;
    //        SetState(eCharacterStates.Trace);
    //    }
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        _tempSkill._state = eCharacterStates.Attack;
    //        _hitPoint = _target.position; _attack = true; _comboIndex = 1;
    //        SetState(eCharacterStates.Trace);
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        _tempSkill._state = eCharacterStates.Attack;
    //        _hitPoint = _target.position; _attack = true; _comboIndex = 2;
    //        MoveChangeStats();
    //    }
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        _tempSkill._state = eCharacterStates.Attack01;
    //        _hitPoint = _target.position; _attack = true; _comboIndex = 0;
    //        MoveChangeStats();
    //    }
    //}

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
        if (_comboIndex == 2)
        {
            _bossAttack = false;
            _movement.NavAgentAction(false);
            _movement.AgentStop(true);
            WatchTarget();
            _animaController.VelocityY = 0f;
            _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // 공격 스타일 중에 어떤 공격을 할지 선택  _comboIndex
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        }
    }
    protected override void AttackUpdate()
    {
        if (_isAttack03)
        {
            _trans.position += _trans.forward * (Time.deltaTime * _attack3Speed);
            return;
        }
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
                CheckAttackFuntion(12f);
            }

            if (!_bossAttack)
            {
                StartCoroutine(DelayStartAttack());
            }
        }
    }

    protected override void Attack01Init()
    {
        _movement.AgentStop(true);
        _movement.NavAgentAction(false);
        _collider.enabled = false;
        _movement.AgentEnabled(false);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
    }
    public override void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        _movement.NavAgentAction(true);
        _collider.enabled = true;
        _movement.AgentEnabled(true);
        SetState(eCharacterStates.Move);
    }

    IEnumerator DelayStartAttack()
    {
        //_isAttack = true;
        _movement.AgentEnabled(false);
        if (_comboIndex == 0) { EffectForwardCreate("Boss/Type8/Boss_Dragon_Attack_01_Area"); }
        else if (_comboIndex == 1) { EffectForwardCreate("Boss/Type8/Boss_Dragon_Attack_02_Area"); }
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        _movement.NavAgentAction(false);
        _movement.AgentStop(true);
        //WatchTarget();
        if (_animaController != null)
        {
            _animaController.VelocityY = 0f;
            _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // 공격 스타일 중에 어떤 공격을 할지 선택  _comboIndex
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        }
    }
    // 간단하게 내 위치에서 앞 방향을 보게 생성
    GameObject EffectForwardCreate(string path)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, path, Vector3.zero, Quaternion.identity);
        obj.transform.position = _trans.position;
        obj.transform.forward = _trans.forward;
        return obj;
    }
    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type8/Boss_Dragon_Attack_01", Vector3.zero, Quaternion.identity);
        obj.transform.position = _skillPoint[0].position; // new Vector3(_skillPoint[0].position.x, 0f, _skillPoint[0].position.z);
        obj.transform.rotation = Quaternion.Euler(12f, this.transform.eulerAngles.y, 0f);
        obj.transform.Find("Collider").GetComponent<BossPushAttack>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type8/Boss_Dragon_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.rotation = Quaternion.Euler(0f, this.transform.eulerAngles.y, 0f);
        obj.transform.Find("Collider").GetComponent<BossPushAttack>().InitData(_enemyStats, _enemyStats.GetTDD2());
    }
    public void AttackEffect3()
    { 
        if (_attack03 != null)
        {
            _attack03.gameObject.SetActive(true);
        }
    }
    public void AttackEffect3End()
    {
        if (_attack03 != null)
        {
            _attack03.gameObject.SetActive(false);
        }
    }
    public void Attack03Line()
    {
        WatchTarget();
        EffectForwardCreate("Boss/Type8/Boss_Dragon_Attack_03_Area");
    }
    public void Attack03StartMove()
    {
        _isAttack03 = true;
        AttackEffect3();
    }
    public void Attack03EndMove()
    {
        _isAttack03 = false;
        AttackEffect3End();
    }
    // 회오리를 만든다
    public void Attack01Effect()
    {
        GameObject obj = EffectForwardCreate("Boss/Type8/Boss_Dragon_Attack01_Air");
        obj.GetComponent<BossContinueAttack>().InitData(_enemyStats, _enemyStats.GetTDD4());
    }

    public override void DoDie()
    {
        base.DoDie();
        SoundManager.GetInstance.PlayAudioEffectSound("BossCrab_Dead");
        //Play("BossCrab_Dead", 0f);
    }
}
