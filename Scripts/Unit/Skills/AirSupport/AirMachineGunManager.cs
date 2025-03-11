using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����
// ���߿��� �÷��̾� �ֺ����� ����� ����
public class AirMachineGunManager : BaseAttack
{
    [SerializeField] GameObject _underwaterMinePrefab;
    AirMachineGunScript _airSupportObj;
    PlayerController _playerC;

    float _maxTime = 7f;
    float _time = 0f;

    Transform _target;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
        AirMachineGunScript obj = GameObject.Instantiate(_underwaterMinePrefab).GetComponent<AirMachineGunScript>();
        _airSupportObj = obj;
        _airSupportObj.InitData(data);
    }
    public void Target(Transform target)
    {
        _target = target;
        //_time = _maxTime;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _time = 0f;
            _airSupportObj.StartAbility(_target.position);
        }
    }
}