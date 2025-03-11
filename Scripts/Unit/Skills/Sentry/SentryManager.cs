using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ʈ������ ������
public class SentryManager : BaseAttack
{
    [SerializeField] GameObject _sentryPrefab;
    SentryObj _sentryObj;
    PlayerController _playerC;

    float _maxTime = 10f;
    float _time = 0f;

    Transform _target;

    float _minRadius = 3f;  // �ּ� �ݰ�
    float _maxRadius = 6f;  // �ִ� �ݰ�

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
        SentryObj obj = GameObject.Instantiate(_sentryPrefab).GetComponent<SentryObj>();
        _sentryObj = obj;
        _sentryObj.Init(_playerC);
    }
    public void Target(Transform target)
    {
        _target = target;
        _time = _maxTime;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _time = 0f;

            _sentryObj.OnObj(StandardFuncUnit.GetRandomPointInDonut(_target.position, _minRadius, _maxRadius));
        }
    }
}
