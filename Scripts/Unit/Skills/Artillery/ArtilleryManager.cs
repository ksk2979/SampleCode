using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ������ ������
// ���� ��� �ִ� ������ �ٸ��� (��, ��, ����, ����)
public class ArtilleryManager : BaseAttack
{
    [SerializeField] GameObject[] _underwaterMinePrefab;
    ArtilleryObj[] _artilleryObj;
    PlayerController _playerC;

    bool[] _artilleryCheck;
    float[] _maxTime;
    float[] _time;

    int[] _initOrder; // �ʱ�ȭ ������ ������ �迭
    int _nextOrder; // ���� �ʱ�ȭ ������ ����� ����

    Transform _target;

    float _minRadius = 3f;  // �ּ� �ݰ�
    float _maxRadius = 10f;  // �ִ� �ݰ�

    public void InitData(UnitDamageData data, PlayerController playerController, int numArr)
    {
        if (_artilleryCheck == null)
        {
            _artilleryCheck = new bool[_underwaterMinePrefab.Length];
            _maxTime = new float[_underwaterMinePrefab.Length];
            _time = new float[_underwaterMinePrefab.Length];
            _initOrder = new int[_underwaterMinePrefab.Length]; // �ʱ�ȭ ������ ������ �迭 �ʱ�ȭ
            _artilleryObj = new ArtilleryObj[_underwaterMinePrefab.Length];
            for (int i = 0; i < _maxTime.Length; ++i)
            {
                _maxTime[i] = 15f;
            }
            _nextOrder = 0;
        }
        tdd = data;
        _playerC = playerController;
        ArtilleryObj obj = GameObject.Instantiate(_underwaterMinePrefab[numArr]).GetComponent<ArtilleryObj>();
        _artilleryObj[numArr] = obj;
        _artilleryObj[numArr].Init(data, this.transform, numArr);
        _artilleryCheck[numArr] = true;

        // �ʱ� �߻� �ð� ����
        _initOrder[numArr] = _nextOrder; // �ʱ�ȭ ���� ���
        _time[numArr] = _maxTime[numArr] - 3f * _initOrder[numArr];  // 3�� �������� �ʱ� �ð� ����
        _nextOrder++; // ���� �ʱ�ȭ ������ ����
    }

    public void Target(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        for (int i = 0; i < _artilleryCheck.Length; ++i)
        {
            if (_artilleryCheck[i])
            {
                _time[i] += Time.deltaTime;
                if (_time[i] > _maxTime[i])
                {
                    _time[i] = 0f;
                    _artilleryObj[i].StartAbility(StandardFuncUnit.GetRandomPointInDonut(_target.position, _minRadius, _maxRadius));
                }
            }
        }
    }

    public bool[] GetArtilleryCheck => _artilleryCheck;
}
