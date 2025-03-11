using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �Ŵ���
// �ð����� Ÿ�� ��Ʈ�� �� �ڿ� ���ڸ� �ɴ´�
public class UMManager : BaseAttack
{
    [SerializeField] GameObject _underwaterMinePrefab;
    Queue<UnderwaterMines> _underwaterMineQ;
    PlayerController _playerC;

    float _maxTime = 3f;
    float _time = 0f;

    Transform _target;

    public void InitData(UnitDamageData data, PlayerController playerController)
    {
        tdd = data;
        _playerC = playerController;
    }
    public void Init()
    {
        _underwaterMineQ = new Queue<UnderwaterMines>();
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
                CreateUnderwaterMine();
            }
        }
    }

    void CreateUnderwaterMine()
    {
        if (_underwaterMineQ.Count != 0)
        {
            _underwaterMineQ.Dequeue().StartAbility(_target);
        }
        else
        {
            UnderwaterMines obj = GameObject.Instantiate(_underwaterMinePrefab).GetComponent<UnderwaterMines>();
            obj.InitData(tdd, this);
            obj.StartAbility(_target);
        }
    }

    public void UnderwaterMineEnqueue(UnderwaterMines underwaterMine)
    {
        _underwaterMineQ.Enqueue(underwaterMine);
    }
}
