using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 위성 호출 레이저 발사
public class SatelliteLaserCallManager : BaseAttack
{
    [SerializeField] GameObject _satellitePrefab;
    SatelliteLaserCallObj _obj;
    PlayerController _playerC;

    float _maxTime = 10f; // 120f
    float _time;

    bool _on = false;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
        SatelliteLaserCallObj obj = GameObject.Instantiate(_satellitePrefab).GetComponent<SatelliteLaserCallObj>();
        _obj = obj;
        _obj.Init(data, this);
        OnReset();
        _time = _maxTime; // 테스트
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
}
