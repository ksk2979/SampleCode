using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 거북이 (현무)
/*
목 내밀기, 물폭탄 투하, 광역 딜, 순간이동 해서 방어막 생성 해서 방어막 깨지면 그로기/ 방어막 안깨지고 체력이 풀피가 되면 방어막 터지면서 광역딜
*/

public class BossType9Controller : EnemyController
{
    [SerializeField] BossBoxAttackEffect _attack1;

    //float _skillSpeed = 0.01f;
    //float _skillSpeed2 = 0.015f;

    TentacleScript _shieldScript;
    bool _groggy = false;
    [SerializeField] float _hpSpeed = 3f;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();
            if (_animaController != null)
            {
                //_animaController.AnimEnventSender("EndSkill", EndSkill);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("Attack01", Attack01);
                //_animaController.AnimEnventSender("Attack1EffectPos", Attack1EffectPos);
                //_animaController.AnimEnventSender("Attack3Start", Attack3Start);
                //_animaController.AnimEnventSender("Attack3End", Attack3End);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // 최대체력에서 80% 50% 30% 남았을때 스킬 사용하게 하자
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
            int[] skillH = new int[] { 50 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 5f, 1f, 10f);

            _attack1.InitData(_enemyStats, _enemyStats.GetTDD1());

            _oneOnStartInit = true;
        }

        SetState(eCharacterStates.Spawn);
        _movement.angularSpeed = 120; // 회전값 리 초기화
    }

    //private void Update()
    //{
    //    // 임의로 생성
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        StartCoroutine(DelayAttack(0));
    //    }
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        StartCoroutine(DelayAttack(1));
    //    }
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        StartCoroutine(DelayAttack(2));
    //    }
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        StartCoroutine(DelayAttack(3));
    //    }
    //}
    //IEnumerator DelayAttack(int num)
    //{
    //    yield return YieldInstructionCache.WaitForSeconds(1f);
    //    if (num == 0) { AttackEffect1(); }
    //    else if (num == 1) { AttackEffect2(); }
    //    else if (num == 2) { StartCoroutine(AttackDelayFuntion()); }
    //    else { if (_curState != eCharacterStates.Attack01) { SetState(eCharacterStates.Attack01); } }
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
        //_attack3 = false;
        _movement.AgentEnabled(false);
        StartCoroutine(DelayAgentEnabled());
    }

    protected override void AttackUpdate()
    {
        if (_bossAttack)
        {
            if (_comboIndex == 0) // 목 내밀기
            {
                CheckAttackFuntion(5f);
            }
            else if (_comboIndex == 1) // 위로 큰 물덩이를 쏴서 떨어진 거 장판 딜
            {
                CheckAttackFuntion(20f);
            }
            else if (_comboIndex == 2) // 광역 딜
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
        }

        //if (_attack3)
        //{
        //    if (_target != null)
        //    {
        //        _movement.speed = _enemyStats.traceMoveSpeed * 1.2f;
        //        var targetPos = _target.transform.position;
        //        targetPos.y = _trans.position.y;
        //        _movement.SetDestination(targetPos);
        //    }
        //}
    }


    public void AttackEffect1()
    {
        _attack1.gameObject.SetActive(true);
    }
    public void AttackEffect2()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack_02", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.GetComponent<BossType9Attack2Script>().InitData(_enemyStats, _enemyStats.GetTDD2());
        obj.GetComponent<BossType9Attack2Script>().StartAttack();
    }
    public void AttackEffect3()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack_03_A", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.Find("Collider").GetComponent<BossNormalAttack>().InitData(_enemyStats, _enemyStats.GetTDD3());
        obj.gameObject.SetActive(true);
    }
    public void Attack01()
    {
        GameObject main = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack01", Vector3.zero, Quaternion.identity);
        _shieldScript = main.GetComponent<TentacleScript>();
        _shieldScript.transform.position = _trans.position;
        _shieldScript.gameObject.SetActive(true);
        _shieldScript.Init(this);
        _shieldScript.Resetting(StandardFuncUnit.OperatorValue(_enemyStats.maxHp, 20, OperatorCategory.persent));
    }
    public void Attack01BombAttack()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack_03_A", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.Find("Collider").GetComponent<BossNormalAttack>().InitData(_enemyStats, _enemyStats.GetTDD4());
        obj.gameObject.SetActive(true);
    }

    // 그로기 되면 몇로간 가만히 있다가 idle로 돌아간다
    public void Attack01Groggy()
    {
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_GROGGY);
        _groggy = true;
    }

    protected override void Attack01Init()
    {
        _movement.AgentStop(true);
        NotHit();
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
    }

    protected override void Attack01Update()
    {
        if (_groggy) { return; }

        if (_enemyStats.hp < _enemyStats.maxHp)
        {
            _enemyStats.hp += Time.deltaTime * _hpSpeed;
            InGameUIManager.GetInstance.SetBossHp(_enemyStats.HpRate());
        }
        else
        {
            // 터져야한다
            _enemyStats.hp = _enemyStats.maxHp;
            InGameUIManager.GetInstance.SetBossHp(_enemyStats.HpRate());
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ENDSKILL);
            _shieldScript.EndObj();
            Attack01BombAttack();
            AttackFinishCall();
            return;
        }
    }

    public override void AttackFinishCall()
    {
        if (_groggy) { _groggy = false; }
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        OnHit();
        SetState(eCharacterStates.Idle);
    }

    public override void DoDie()
    {
        base.DoDie();
        AudioPlayerFuntion("BossCrab_Dead", 0f);
    }
}
