using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스가 있을때 포격을 요청해 보스의 체력을 까준다 (쿨타임이 있으며 제한적 탄환이 존재한다)
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
        OnReset(); // 바로 쏠수 있게 한다
    }
    // 보스를 가져와야한다
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
            // 보스가 죽지 않았다면 쏜다
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