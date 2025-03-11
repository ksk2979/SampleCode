using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ������ ������ ��û�� ������ ü���� ���ش� (��Ÿ���� ������ ������ źȯ�� �����Ѵ�)
public class BombardmentRequestManager : BaseAttack
{
    [SerializeField] GameObject _bombardmentPrefab;
    BombardmentRequestObj _bombardObj;
    PlayerController _playerC;

    float _maxTime = 300f;
    float _time;

    Transform _target;

    bool _on = false;

    EnemyController _enemyController;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
        BombardmentRequestObj obj = GameObject.Instantiate(_bombardmentPrefab).GetComponent<BombardmentRequestObj>();
        _bombardObj = obj;
        _bombardObj.Init(data, this.transform);
        OnReset(); // �ٷ� ��� �ְ� �Ѵ�
    }
    // ������ �����;��Ѵ�
    public void Target(EnemyController target)
    {
        _enemyController = target;
    }
    public void PlayerTarget(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }
        if (_enemyController != null) { BossTargetOn(); }
        if (_on) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _on = true;
            _time = 0f;
        }
    }

    public void BossTargetOn()
    {
        if (_on)
        {
            _on = false;
            // ������ ���� �ʾҴٸ� ���
            if (!_enemyController.IsDie())
            {
                _bombardObj.StartAbility(_target.position, _enemyController._trans);
            }
        }
    }

    public void OnReset()
    {
        _on = false;
        _time = _maxTime;
    }
}