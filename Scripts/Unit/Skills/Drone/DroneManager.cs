using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드론 호출
public class DroneManager : BaseAttack
{
    [SerializeField] GameObject _dronePrefab;
    DroneObj _obj;
    PlayerController _playerC;

    float _maxTime = 10f; // 120f
    float _time;

    bool _on = false;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
        DroneObj obj = GameObject.Instantiate(_dronePrefab).GetComponent<DroneObj>();
        _obj = obj;
        _obj.Init(data, this);
        OnReset();
        _time = _maxTime;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }
        if (_on) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _on = true;
            _time = 0f;
            _obj.StartAbility();
        }
    }

    public void OnReset()
    {
        _on = false;
    }

    public Transform GetPlayerTrans => _playerC.transform;
}
