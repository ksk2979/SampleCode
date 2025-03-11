using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NettingCreateManager : MonoBehaviour
{
    [SerializeField] GameObject _nettingCreatePrefab;

    Queue<NettingCreateObj> _nettingCreateQ;
    PlayerController _playerC;
    PlayerAbility _playerAbility;

    float _maxTime = 8f;
    float _time = 0f;

    Transform _target;

    public void Init(Player player)
    {
        _playerC = player.GetController() as PlayerController;
        _playerAbility = player._playerAbility;
        _nettingCreateQ = new Queue<NettingCreateObj>();
    }

    public void Target(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        if(_playerC._velocity != Vector3.zero)
        {
            _time += Time.deltaTime;
            if (_time > _maxTime)
            {
                _time = 0f;
                CreateNettingCreate();
            }
        }
    }

    void CreateNettingCreate()
    {
        Vector3 targetPos = _playerC._gunFireTop.TargetPos();
        NettingCreateObj obj = null;
        if (_nettingCreateQ.Count != 0)
        {
            obj = _nettingCreateQ.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(_nettingCreatePrefab).GetComponent<NettingCreateObj>();
            obj.Init(_playerAbility, this);
        }
        if(targetPos != Vector3.zero)
        {
            obj.StartAbility(targetPos);
        }
    }

    public void NettingCreateObjEnqueue(NettingCreateObj nettingCreate)
    {
        _nettingCreateQ.Enqueue(nettingCreate);
    }
}
