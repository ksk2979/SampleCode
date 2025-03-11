using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ظ� ��ũ��Ʈ (���� ��Ϲ����� �ٲ㼭 ��ä�� �����ϴµ� ù ���� ���� �ٸ� ��ä�� �ִٸ� �� ��ä�� ƨ���� ������ �̾�� (�ִ� 3��ü))
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
        // ȸ���ϴ� �ִ� �ְ� ���� ����
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
            // �ٸ� ��Ʈ��
            if (_listPlayer.Count == 0) { _speedSub = 7f; return; }

            _target = _listPlayer[0].transform;
            _listPlayer.Remove(_listPlayer[0]);
            StandardFuncUnit.WatchTarget(_trans, _target);

            _speed = _enemyStats.traceMoveSpeed * 2f;
            _speedSub = 5f;
        }
        else if (_count == 2)
        {
            // �ٸ� ��Ʈ��
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