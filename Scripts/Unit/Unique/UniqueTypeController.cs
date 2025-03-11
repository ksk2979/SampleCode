using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueTypeController : EnemyController
{
    [SerializeField] int _arrNum;
    float _time;
    float _timeMax;

    bool _isHit = false;
    float _prevHp = 0f;
    float _maxHp = 0f;
    float _lifeTime = 0f;
    float _lifeMaxTime = 30f;
    float _persentHp = 0f;

    int[] _persentValue = { 80, 60, 40, 20 };
    int _persentIndex = 0;

    public override void OnStart()
    {
        base.OnStart();
        StateStart();
        SetState(eCharacterStates.Idle);
        // 여기서 리 셋팅 해야 될듯
        if (GetComponent<Enemy>().GetEnemyData != null)
        {
            MyData.EnemyData data = GetComponent<Enemy>().GetEnemyData;
            float maxHp = data.hp;
            for (int j = 0; j < StageManager.GetInstance._stageNumber; j++)
            {
                maxHp = maxHp * 1.155f;
            }

            _enemyStats.maxHp = maxHp;
            _enemyStats.hp = _enemyStats.maxHp;
        }
        _prevHp = _enemyStats.hp;
        _maxHp = _enemyStats.maxHp;
        _timeMax = 5f;
        _persentIndex = 0;
       _persentHp = StandardFuncUnit.OperatorValue(_maxHp, _persentValue[_persentIndex], OperatorCategory.persent);
    }

    IEnumerator CheckDisappear()
    {
        yield return YieldInstructionCache.WaitForSeconds(10f);
    }

    private void Update()
    {
        if(_prevHp == _enemyStats.hp)
        {
            _lifeTime += Time.deltaTime;
            if (_lifeTime > _lifeMaxTime)
            {
                Debug.Log("없어짐");
                SimplePool.Despawn(gameObject);
            }
        }
        else
        {
            _lifeTime = 0f;
            _prevHp = _enemyStats.hp;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ItemBoxCreate();
        }
        if (_persentValue.Length > _persentIndex + 1)
        {
            if (_enemyStats.hp < _persentHp)
            {
                _persentIndex++;
                _persentHp = StandardFuncUnit.OperatorValue(_maxHp, _persentValue[_persentIndex], OperatorCategory.persent);
                ItemBoxCreate();
            }
        }
    }
    void ItemBoxCreate()
    {
        // 아이템 뿌리기
        Debug.Log("아이템 뿌리기");
        UniqueBox box = SimplePool.Spawn(CommonStaticDatas.RES_UNIQUE, "UniqueBox", Vector3.zero, Quaternion.identity).GetComponent<UniqueBox>();
        if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
        box.transform.position = Vector3.zero;
        box.transform.rotation = Quaternion.identity;
        box.Init(_trans.position, _stageManager._chapter[0].GetRandomPlayerDisPos());
    }

    protected override void IdleInit()
    {
        _movement.AgentStop(true);
        if (m_isSpawn == false)
            m_idleTime = _enemyStats._idleTime;
        m_isSpawn = false;
        _idleInitTIme = Time.realtimeSinceStartup;
        SetTargetting();
    }
    protected override void IdleUpdate()
    {
        base.IdleUpdate();
        _movement.SetDestination(_trans.position);
        if (_animaController != null)
            _animaController.VelocityY = _movement._curSpeedRate;
        if (Time.realtimeSinceStartup - _idleInitTIme < m_idleTime)
            return;

        SetMoveState();
    }

    protected override void MoveInit()
    {
        _movement.AgentStop(false);
        if (_movement.enabled == true)
        {
            _movement.SetDestination(_hitPoint);
        }

        _movement._oldFwd = _trans.forward;
    }

    protected override void MoveUpdate()
    {
        _movement.AgentStop(false);

        if (_animaController != null)
            _animaController.VelocityY = _movement._curSpeedRate;

        _time += Time.deltaTime;
        if (_time > _timeMax) { _time = 0f; _timeMax = Random.Range(2.5f, 5f); SetMoveState(); }

        //높이 오차 수정
        var dist = Vector3.Distance(_trans.position, _hitPoint);
        if (dist <= 1.8f)
            SetState(eCharacterStates.Idle);
    }
    public override void SetMoveState()
    {
        if (_curState == eCharacterStates.Die)
            return;

        _hitPoint = _randomPos;
        SetState(eCharacterStates.Move);
    }

    public override void SetSpeed()
    {
        _movement.speed = 5;

        //float ratio = totalSpeedConditioningRatio;
        //if (_curState == eCharacterStates.Move)
        //    _agent.speed = _characterStats.moveSpeed * ratio;
        //else
        //    _agent.speed = _characterStats.traceMoveSpeed * ratio;
        //if (_animator != null)
        //    _animator.speed = defalutAnimatorSpeed * totalSpeedAnimConditioningRatio;
    }

    protected override void MoveFinish()
    {
        base.MoveFinish();
    }

    public override void DoDie()
    {
        SetNoneTargetting();
        SetState(eCharacterStates.Die);
    }

    protected override void DieInit()
    {
        if (_animaController != null)
        {
            _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, true);
            _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_DIE_1);
        }
        DieInitFunc();
    }

    protected override void DieInitFunc()
    {
        if (_movement.enabled)
            _movement.AgentEnabled(false);
        ColliderEnabled(false);
        testDieInitCheck = true;
    }

    public override void DieFinishCall()
    {
        if (UserData.GetInstance.TutorialCheck().Equals(9))
        {
            UserData.GetInstance.SaveTutorial(100);
            UserData.GetInstance.CollectionUnitSave(_arrNum, 7);
        }
        else
        {
            UserData.GetInstance.CollectionUnitSave(_arrNum, UserData.GetInstance.GetCollectionData._collectionList[_arrNum] + 1);
        }

        if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
        _stageManager.UniqueDie();
        if (_animaController != null)
            _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, false);
        SimplePool.Despawn(gameObject);
    }
}