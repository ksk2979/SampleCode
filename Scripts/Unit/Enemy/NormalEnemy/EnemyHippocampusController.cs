using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 해마 스크립트 (몸을 톱니바퀴로 바꿔서 선채를 가격하는데 첫 가격 이후 다른 선채가 있다면 그 선채가 튕겨져 공격을 이어간다 (최대 3개체))
public class EnemyHippocampusController : EnemyController
{
    [SerializeField] EnemyHippocampusAttack _attack;
    int _count = 0;
    float _speed = 0f;
    float _speedSub = 0f;

    List<PlayerController> _listPlayer;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _listPlayer = new List<PlayerController>();
            _attack.Init(this, _enemyStats);
            _oneOnStartInit = true;
        }
        SetState(eCharacterStates.Spawn);
    }

    public override void BasicAttack() { }
    protected override void AttackInit()
    {
        //base.AttackInit();
        _listPlayer.Clear();
        if (GameManager.GetInstance.TestEditor) { for (int i = 0; i < SpawnTestManager.GetInstance.GetPlayerList().Count; ++i) { _listPlayer.Add(SpawnTestManager.GetInstance.GetPlayerList()[i].GetComponent<PlayerController>()); } }
        else
        {
            if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
            for (int i = 0; i < _stageManager.GetPlayers().Count; ++i) { _listPlayer.Add(_stageManager.GetPlayers()[i].GetComponent<PlayerController>()); }
        }
        WatchTarget();
        if (_target == null) { SetState(eCharacterStates.Trace); return; }
        else { _listPlayer.Remove(_target.GetComponent<PlayerController>()); }
        _attack.ActiveOn();
        _movement.AgentStop(true);
        _movement.NavAgentAction(false);
        // 회전하는 애니 넣고 돌진 시작
        _count = 0;
        _speed = _enemyStats.traceMoveSpeed * 1.5f;
        _speedSub = 3f;
    }
    protected override void AttackUpdate()
    {
        _speed -= Time.deltaTime * _speedSub;
        if (_speed < 0f) { SetState(eCharacterStates.Trace); return; }
        _trans.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
    public void AttackOn()
    {
        _count++;
        if (_count == 1)
        {
            // 다른 보트로
            if (_listPlayer.Count == 0) { _speedSub = 7f; return; }

            _target = _listPlayer[0].transform;
            _listPlayer.Remove(_listPlayer[0]);
            StandardFuncUnit.WatchTarget(_trans, _target);

            _speed = _enemyStats.traceMoveSpeed * 2f;
            _speedSub = 5f;
        }
        else if (_count == 2)
        {
            // 다른 보트로
            if (_listPlayer.Count == 0) { _speedSub = 9f; return; }

            _target = _listPlayer[0].transform;
            _listPlayer.Remove(_listPlayer[0]);
            StandardFuncUnit.WatchTarget(_trans, _target);

            _speed = _enemyStats.traceMoveSpeed * 3f;
            _speedSub = 7f;
        }
        else if (_count == 3)
        {
            _attack.ActiveOff();
            _speedSub = 9f;
        }
    }
    public override void AttackFinishCall() { }
}