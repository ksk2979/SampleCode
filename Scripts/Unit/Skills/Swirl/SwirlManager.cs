using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlManager : BaseAttack
{
    [SerializeField] GameObject _swirlPrefab;
    Queue<SwirlObj> _swirlQ;
    PlayerController _playerC;

    Transform _target;

    float _time = 0f;
    float _maxTime = 10f;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
    }
    public void Init()
    {
        _swirlQ = new Queue<SwirlObj>();
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
            CreateSwirl();
        }
    }

    void CreateSwirl()
    {
        if (_swirlQ.Count != 0)
        {
            //_target.position - (Vector3.forward * 2.5f)
            Vector3 spawn = _target.position + (new Vector3(Random.Range(-1, 3), 0, Random.Range(-1, 3)) * 3f);
            _swirlQ.Dequeue().StartAbility(spawn);
        }
        else
        {
            SwirlObj obj = GameObject.Instantiate(_swirlPrefab).GetComponent<SwirlObj>();
            obj.InitData(tdd, this);
            Vector3 spawn = _target.position + (new Vector3(Random.Range(-1, 3), 0, Random.Range(-1, 3)) * 3f);
            obj.StartAbility(spawn);
        }
    }

    public void SwirlObjEnqueue(SwirlObj swirl)
    {
        _swirlQ.Enqueue(swirl);
    }

}
