using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// ũ����
/*
�Թ� �߻� - ���濡 �Թ��� ����, ������ �����鼭 ���ο�� �þ߸� �����Ѵ�(���θ� ����)
�˼� ���� - �˼��� �ٴ� ������ ���� Ÿ�ٿ�(8�� ����) ���� ó�� ����
���� - �˼��� �̿��ؼ� �ֺ��� ������ ���� ���� ����

ũ������ �г� - ������ �������� ���κ�Ʈ�� �߽����� �˼��� ���Ѵ�
�̶� ���� ��Ʈ���� �˼��� �������� �ִ� ���°� �ȴ� (��������)
8���� �ٸ��� �ִ� ���꺸Ʈ���� ���̸鼭 ���κ�Ʈ�� ���� �ȴ�
������ 4���� �˼��ٸ��� ���κ������ؼ� �������鼭 �й��� �Ѵ�
���κ���� �ϳ��� ��ο� �й��ؿ��� �˼��� 1���� ������ �������� �־�� ��� �ִ�
���� �ð��ȿ� �˼� �ϳ��� ������ ��� �����ϰ� �ȴ�
*/
public class BossType10Controller : EnemyController
{
    public GameObject _loopEffectObj;
    public Transform _skillPoint;

    public int _range = 100;

    [SerializeField] Transform[] _catchingTrans;
    [SerializeField] float _playerWanderRadius = 50f;

    Transform[] _supAttackTrans;
    Animator[] _supAnima;
    TentacleScript[] _mainAttackTrans;
    [SerializeField] float _attack01HpValue = 100f;
    [SerializeField] float _sclaeSpeed = 20f;
    [SerializeField] GameObject _attack2LoopEffectObj;

    public override void OnStart()
    {
        base.OnStart();
        _supAttackTrans = new Transform[4];
        _mainAttackTrans = new TentacleScript[4];
        _supAnima = new Animator[_supAttackTrans.Length];
        if (!_oneOnStartInit)
        {
            StateStart();
            SetSkillTdd();
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("OnHit", OnHit);
                _animaController.AnimEnventSender("NotHit", NotHit);
                _animaController.AnimEnventSender("AttackLineEffect01", AttackLineEffect01);
                _animaController.AnimEnventSender("AttackEffect1", AttackEffect1);
                _animaController.AnimEnventSender("AttackEffect2", AttackEffect2);
                _animaController.AnimEnventSender("AttackEffect2End", AttackEffect2End);
                _animaController.AnimEnventSender("AttackEffect3", AttackEffect3);
                _animaController.AnimEnventSender("Attack01Sup", Attack01Sup);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            // �ִ�ü�¿��� 80% 50% 30% �������� ��ų ����ϰ� ����
            _healthThresholds = new List<int>();
            _comboIndexSetting = new int[] { 0, 1, 2 };
            int[] skillH = new int[] { 80, 50, 30 };
            HealthSkillSetting(skillH);
            AttackAndMoveInit(0.5f, 3f, 1f, 10f);

            _oneOnStartInit = true;
        }

        SetState(eCharacterStates.Spawn);
        _movement.angularSpeed = 120; // ȸ���� �� �ʱ�ȭ
        _traceAddSpeed = 3f;
        _attack01HpValue = StandardFuncUnit.OperatorValue(_enemyStats.maxHp, 1, OperatorCategory.persent);
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        _time = 0;
    //        _comboIndex = 0;
    //        _target = StandardFuncUnit.CheckTraceTarget(_trans, _targetTag, 10000f, _unitLayerMask);
    //        SetState(eCharacterStates.Attack);
    //    }
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        _time = 0;
    //        _comboIndex = 1;
    //        SetState(eCharacterStates.Attack);
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        _time = 0;
    //        _comboIndex = 2;
    //        // �������� ��� ���� ���� ����
    //        if (_animaController != null) { _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); }
    //        _down = true;
    //    }
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        _time = 0;
    //        SetState(eCharacterStates.Attack01);
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

        // ���������� �ٶ󺸸鼭 �� ���ִ� �ൿ�� Ž���Ѵ�
        FaceTarget(5f);
        // �Ÿ��� ������� �־����� move�Լ� ȣ��
        if (_target != null) { TargetDistanceSetTrace(_distanceF); }
        // ������ ó�� �Լ�
        if (MoveFuntion()) { return; }
        // ���� ó�� �Լ�
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

            // ���������� ���󰣴�
            if (_target != null)
            {
                _movement.SetDestination(_target.position);
                // ���󰡴µ� �Ÿ��� ����� ���� �����·�
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
            if (_comboIndex == 0)
            {
                CheckAttackFuntion(15f);
            }
            else if (_comboIndex == 1)
            {
                CheckAttackFuntion(25f);
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
                    _animaController.PlayIntegerAnimation(CommonStaticDatas.ANIMPARAM_COMBO, _comboIndex); // ���� ��Ÿ�� �߿� � ������ ���� ����  _comboIndex
                    _animaController.VelocityY = 0f;
                    if (_comboIndex == 2) { StartCoroutine(AttackEffect3IncreaseInScope()); }
                    else { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK); }
                }
            }
            return;
        }
    }

    // �Թ��߻�
    Transform _attack1AreaSkill;
    public void AttackLineEffect01()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack1_Area", Vector3.zero, Quaternion.identity);
        if (_target != null) { obj.transform.position = new Vector3(_target.position.x, 0f, _target.position.z); }
        _attack1AreaSkill = obj.transform;
    }
    public void AttackEffect1()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack_01_Bullet", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        AquaSquirt ast = obj.GetComponent<AquaSquirt>();
        ast.InitData(_enemyStats, _enemyStats.GetTDD1());
        ast.SetData(_trans.position, _attack1AreaSkill.position);
    }
    // �˼� ���
    public void AttackEffect2()
    {
        StartCoroutine(CreateAttackType2());
        _attack2LoopEffectObj.SetActive(true);
    }
    IEnumerator CreateAttackType2()
    {
        int count = 0;
        
        while (count < 8)
        {
            count++;

            GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack2_Area", Vector3.zero, Quaternion.identity);
            //_target = StandardFuncData.GetRandomItem().transform;
            List<Interactable> alivePlayers;
            if (GameManager.GetInstance.TestEditor) { alivePlayers = SpawnTestManager.GetInstance.GetPlayerList().Where(p => !p.GetController().IsDie()).ToList(); }
            else { alivePlayers = _stageManager.GetPlayers().Where(p => !p.GetController().IsDie()).ToList(); }
            // �÷��̾� �߿� ���� �������� üũ�Ѵ�
            if (alivePlayers.Count == 0) { break; }
            else
            {
                int index = Random.Range(0, alivePlayers.Count);
                _target = alivePlayers[index].transform;
            }
            if (_target != null) { obj.transform.position = new Vector3(_target.position.x, 0f, _target.position.z); }
            yield return YieldInstructionCache.WaitForSeconds(0.3f);
            GameObject attack = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack_02", Vector3.zero, Quaternion.identity);
            attack.transform.position = new Vector3(obj.transform.position.x, 0f, obj.transform.position.z);
            attack.transform.forward = obj.transform.forward;
            attack.transform.Find("Collider").GetComponent<BossBoxAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD2());
        }

        if (_animaController != null)
        {
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_SKILL1END);
        }
    }
    public void AttackEffect2End()
    {
        _attack2LoopEffectObj.SetActive(false);
    }

    // ����
    public void AttackEffect3()
    {
        //GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack_03", Vector3.zero, Quaternion.identity);
        //obj.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(_enemyStats, _enemyStats.GetTDD2());
        //obj.transform.position = new Vector3(_attack3Trans[_attack3Count].position.x, 0f, _attack3Trans[_attack3Count].position.z);
        //obj.transform.rotation = _attack3Trans[_attack3Count].transform.rotation;
        //_attack3Count++;
        //StartCoroutine(AttackEffect3IncreaseInScope());
    }
    
    IEnumerator AttackEffect3IncreaseInScope()
    {
        // �� ���� ǥ�� ���ش�
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack_03", Vector3.zero, Quaternion.identity);
        obj.transform.position = _trans.position;
        GameObject mesh = obj.transform.Find("Mesh").gameObject;
        BossContinueAttack collider = obj.transform.Find("Collider").GetComponent<BossContinueAttack>();
        collider.InitData(_enemyStats, _enemyStats.GetTDD3());
        mesh.SetActive(true);
        collider.enabled = false;

        // �������
        float scale = 1f;
        while (scale < 100f)
        {
            mesh.transform.localScale = new Vector3(scale, scale, scale);
            scale += Time.deltaTime * _sclaeSpeed;
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        scale = 100f;
        mesh.transform.localScale = new Vector3(scale, scale, scale);

        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        if (_animaController != null) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK); }

        collider.enabled = true;
        mesh.SetActive(false);
        //GameObject area = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_KingCrab_Attack2_Area", Vector3.zero, Quaternion.identity);
        //area.transform.position = _trans.position;
        yield return YieldInstructionCache.WaitForSeconds(4f);
        collider.enabled = false;
        yield return YieldInstructionCache.WaitForSeconds(1f);
        SimplePool.Despawn(obj);
        //Debug.Log("��ų ��");
    }


    bool _diePlayer = false;
    protected override void Attack01Init()
    {
        WatchTarget();
        _movement.AgentStop(true);
        SetNoneTargetting();
        _movement.AgentEnabled(false);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);

        // �� ����Ʈ�� �����͸� ���ͼ� ���߰� �� ���Ŀ� ���� ����� �����Ѵ�
        if (_supAttackTrans[0] == null)
        {
            for (int i = 0; i < _supAttackTrans.Length; ++i)
            {
                GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack01_Sup", Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                _supAttackTrans[i] = obj.transform;
                _supAnima[i] = obj.transform.Find("Mesh").GetComponent<Animator>();
                GameObject main = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Attack01_Main", Vector3.zero, Quaternion.identity);
                _mainAttackTrans[i] = main.GetComponent<TentacleScript>();
                _mainAttackTrans[i].gameObject.SetActive(false);
                _mainAttackTrans[i].Init(this);
            }
        }
        for (int i = 0; i < _mainAttackTrans.Length; ++i)
        {
            _mainAttackTrans[i].GetComponent<TentacleScript>().Resetting(_attack01HpValue);
        }
        _diePlayer = false;
    }
    public void OKDiePlayer()
    {
        _diePlayer = true;
    }
    protected override void Attack01Update()
    {
        if (_diePlayer)
        {
            _diePlayer = false;
            StartCoroutine(DelayAttack01Finish());
            return;
        }

        for (int i = 0; i < _mainAttackTrans.Length; ++i)
        {
            if (!_mainAttackTrans[i].IsDie) { return; }
        }
        // ���� �� �׾����� ������ �ϱ�
        SetState(eCharacterStates.Spawn);
        List<Interactable> p;
        if (GameManager.GetInstance.TestEditor) { p = SpawnTestManager.GetInstance.GetPlayerList(); }
        else { p = _stageManager.GetPlayers(); }

        for (int i = 0; i < p.Count; ++i)
        {
            if (p[i].GetController().IsDie()) { continue; }
            p[i].GetController().GetMovement.AgentEnabled(true);
        }
        Attack01FinishCall();
        return;
    }
    IEnumerator DelayAttack01Finish()
    {
        // ����Ʈ �����Ǹ鼭 ��� ��Ʈ ���
        List<Interactable> alivePlayers;
        if (GameManager.GetInstance.TestEditor) { alivePlayers = SpawnTestManager.GetInstance.GetPlayerList(); }
        else { alivePlayers = _stageManager.GetPlayers(); }

        for (int i = 0; i < alivePlayers.Count; ++i)
        {
            if (!alivePlayers[i].GetController().IsDie())
            {
                if (alivePlayers[i].GetCharacterCtrl()._main) { SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Skill03_Dead", _mainAttackTrans[0].transform.position, Quaternion.identity); }
                else { SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Skill03_Dead", _supAttackTrans[i - 1].transform.position, Quaternion.identity); _supAnima[i - 1].SetTrigger(CommonStaticDatas.ANIMPARAM_OPEN); }
                alivePlayers[i].GetComponent<Player>().TakeToDamage(float.MaxValue);
                yield return YieldInstructionCache.WaitForSeconds(0.3f);
            }
        }
        //// ��Ʈ�� ����Ʈ ����
        //SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Skill03_Dead", _mainAttackTrans[0].transform.position, Quaternion.identity);
        //for (int i = 0; i < _supAttackTrans.Length; ++i)
        //{
        //    if (_supAttackTrans[i].gameObject.activeSelf)
        //    {
        //        SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type10/Boss_Kraken_Skill03_Dead", _supAttackTrans[i].transform.position, Quaternion.identity);
        //    }
        //}
        yield return YieldInstructionCache.WaitForSeconds(2f);
        SetState(eCharacterStates.Spawn);

        // ��Ȱ�̳� �������� ����� ���κ��忡 ���� ��
        List<Interactable> p;
        if (GameManager.GetInstance.TestEditor) { p = SpawnTestManager.GetInstance.GetPlayerList(); }
        else { p = _stageManager.GetPlayers(); }
        for (int i = 0; i < p.Count; ++i)
        {
            if (p[i].GetController().IsDie()) { continue; }
            p[i].GetController().GetMovement.AgentEnabled(true);
        }

        Attack01FinishCall();
    }

    public void Attack01Sup()
    {
        // ��ü ���κ�Ʈ�� �ű��
        if (GameManager.GetInstance.TestEditor) { _trans.position = SpawnTestManager.GetInstance.GetPlayerList()[0].transform.position; }
        else { _trans.position = _stageManager.GetPlayers()[0].transform.position; }
        
        _trans.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        // �����Ͱ� �ִ� �ڸ��� ���� ���� ��Ʈ�� ���� 4���� �ٸ� ������ �ϱ�
        List<Interactable> p;
        if (GameManager.GetInstance.TestEditor) { p = SpawnTestManager.GetInstance.GetPlayerList(); }
        else { p = _stageManager.GetPlayers(); }
        
        if (p.Count >= 1)
        {
            p[0].GetController().GetMovement.AgentEnabled(false);
            int ro = 0;
            for (int i = 0; i < _mainAttackTrans.Length; ++i)
            {
                _mainAttackTrans[i].transform.position = p[0].transform.position;
                _mainAttackTrans[i].transform.rotation = Quaternion.Euler(0, ro, 0);
                _mainAttackTrans[i].gameObject.SetActive(true);
                ro += 90;
            }
            for (int i = 1; i < p.Count; ++i)
            {
                if (p[i].GetController().IsDie()) { continue; }
                p[i].GetController().GetMovement.AgentEnabled(false);
                _supAttackTrans[i - 1].gameObject.SetActive(true);
                _supAttackTrans[i - 1].position = p[i].transform.position;
            }
        }
    }

    public void Attack01FinishCall()
    {
        for (int i = 0; i < _mainAttackTrans.Length; ++i)
        {
            _mainAttackTrans[i].gameObject.SetActive(false);
            _supAttackTrans[i].gameObject.SetActive(false);
        }

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01);
        SetTargetting();
        _movement.AgentEnabled(true);
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK);
        _canRotateInAttack = false;
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACK01END);
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

    public override void DoDie()
    {
        base.DoDie();
        SoundManager.GetInstance.PlayAudioEffectSound("BossCrab_Dead");
        //Play("BossCrab_Dead", 0f);
    }

    private void ResetAgentMovement(bool stopAgent, float speed)
    {
        _movement.AgentStop(stopAgent);
        if (_movement.enabled)
        {
            _hitPoint = _randomPos;
            _movement.SetDestination(_hitPoint);
            _movement.speed = speed;
        }
    }
}
