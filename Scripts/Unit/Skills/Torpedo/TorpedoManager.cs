using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoManager : BaseAttack
{
    [SerializeField] GameObject _torpedoPrefab;
    Queue<TorpedoObj> _torpedoQ;
    PlayerController _playerC;

    float _maxTime = 5f;
    float _time = 0f;

    Transform _target;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
    }
    public void Init()
    {
        _torpedoQ = new Queue<TorpedoObj>();
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
            CreateTorpedo();
        }
    }

    void CreateTorpedo()
    {
        if (_torpedoQ.Count != 0)
        {
            _torpedoQ.Dequeue().StartAbility(_target.position);
        }
        else
        {
            TorpedoObj obj = GameObject.Instantiate(_torpedoPrefab).GetComponent<TorpedoObj>();
            obj.InitData(tdd, this);
            obj.StartAbility(_target.position);
        }
    }

    public void TorpedoObjEnqueue(TorpedoObj torpedo)
    {
        _torpedoQ.Enqueue(torpedo);
    }
}
