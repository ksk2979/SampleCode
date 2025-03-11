using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기지에서 포격을 날린다
// 각각 들고 있는 포격이 다르다 (독, 불, 전기, 전분)
public class ArtilleryManager : BaseAttack
{
    [SerializeField] GameObject[] _underwaterMinePrefab;
    ArtilleryObj[] _artilleryObj;
    PlayerController _playerC;

    bool[] _artilleryCheck;
    float[] _maxTime;
    float[] _time;

    int[] _initOrder; // 초기화 순서를 저장할 배열
    int _nextOrder; // 다음 초기화 순서를 기록할 변수

    Transform _target;

    float _minRadius = 3f;  // 최소 반경
    float _maxRadius = 10f;  // 최대 반경

    public void InitData(UnitDamageData data, PlayerController playerController, int numArr)
    {
        if (_artilleryCheck == null)
        {
            _artilleryCheck = new bool[_underwaterMinePrefab.Length];
            _maxTime = new float[_underwaterMinePrefab.Length];
            _time = new float[_underwaterMinePrefab.Length];
            _initOrder = new int[_underwaterMinePrefab.Length]; // 초기화 순서를 저장할 배열 초기화
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

        // 초기 발사 시간 설정
        _initOrder[numArr] = _nextOrder; // 초기화 순서 기록
        _time[numArr] = _maxTime[numArr] - 3f * _initOrder[numArr];  // 3초 간격으로 초기 시간 설정
        _nextOrder++; // 다음 초기화 순서를 증가
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
