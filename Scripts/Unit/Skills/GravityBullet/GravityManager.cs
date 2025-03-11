using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : BaseAttack
{
    [SerializeField] GameObject _gravityBulletPrefab;
    Queue<GravityBulletObj> _gravityQ;
    PlayerController _playerC;

    Transform _target;

    float _time = 0f;
    float _maxTime = 5f;

    float _minRadius = 3f;  // 최소 반경
    float _maxRadius = 8f;  // 최대 반경

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
    }
    public void Init()
    {
        _gravityQ = new Queue<GravityBulletObj>();
    }
    public void Target(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _time = 0f;
            CreateGravity();
        }
    }

    void CreateGravity()
    {
        if (_gravityQ.Count != 0)
        {
            Vector3 spawn = StandardFuncUnit.GetRandomPointInDonut(_target.position, _minRadius, _maxRadius);
            GravityBulletObj obj = _gravityQ.Dequeue();
            obj.Init(_target.position, spawn);
        }
        else
        {
            GravityBulletObj obj = GameObject.Instantiate(_gravityBulletPrefab).GetComponent<GravityBulletObj>();
            obj.InitData(tdd, this);
            Vector3 spawn = StandardFuncUnit.GetRandomPointInDonut(_target.position, _minRadius, _maxRadius);
            obj.Init(_target.position, spawn);
        }
    }

    public void GravityObjEnqueue(GravityBulletObj gravity)
    {
        _gravityQ.Enqueue(gravity);
    }
}
