using MyData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyInfoType
{
    ENEMYCOUNT,
    ADDCOUNT,
    ADD_ATK,
    ADD_HP,
}

public class StageSetData : MonoBehaviour
{
    MyData.StageData _stageData;

    [Header("맵 정보 설정")]
    public int[] _bossId; // 보스의 ID
    public TimeSpan _limitTime; // 스테이지 시간
    public float _exp;
    public int[] _materialSetting;
    float _bossMultiplier = 1f;
    List<int[]> _waveInfoList = new List<int[]>();
    List<List<float>> _enemyInfoList = new List<List<float>>();
    Dictionary<int, float> _enemyDamageDic = new Dictionary<int, float>();
    Dictionary<int, float> _enemyHPDic = new Dictionary<int, float>();

    public void Init(MyData.StageData data)
    {
        _stageData = data;
        string time = string.Format("0:{0}", data.limitTime);
        _limitTime = TimeSpan.Parse(time);

        RegisterWaveInfo();
        RegisterEnemyInfo();

        _bossMultiplier = data.bossMutiplier;
        _bossId = data.bossId.ToArray();
        _materialSetting = data.materialSetting.ToArray();
        _exp = data.stageExp;
    }
    // 튜토리얼
    public void Init()
    {
        _waveInfoList.Add(new int[] { 1, 0, 0, 0, 0, 0 });

        _enemyInfoList.Add(new List<float>() { 10, 0, 1, 1 });
        _enemyInfoList.Add(new List<float>() { 0, 0, 0, 0 });
        _enemyInfoList.Add(new List<float>() { 0, 0, 0, 0 });
        _enemyInfoList.Add(new List<float>() { 0, 0, 0, 0 });
        _enemyInfoList.Add(new List<float>() { 0, 0, 0, 0 });
        _enemyInfoList.Add(new List<float>() { 0, 0, 0, 0 });

        _bossId = new int[] { 0 };
        _materialSetting = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        _exp = 0f;
        _limitTime = new TimeSpan(0, 2, 0);
    }

    #region Wave
    /// <summary>
    /// 웨이브 정보 등록(등장하는 몬스터 ID 배열)
    /// </summary>
    void RegisterWaveInfo()
    {
        _waveInfoList.Add(_stageData.wave1.ToArray());
        _waveInfoList.Add(_stageData.wave2.ToArray());
        _waveInfoList.Add(_stageData.wave3.ToArray());
    }

    /// <summary>
    /// 웨이브 전환시 몬스터 숫자 늘리는 메서드
    /// </summary>
    /// <param name="waveNumber">웨이브</param>
    public void AddEnemyCount(int waveNumber)
    {
        // 웨이브 넘버 체크 필요
        var waveInfo = _waveInfoList[waveNumber - 1];
        for (int i = 0; i < waveInfo.Length; i++)
        {
            // wave Info에 몬스터 ID가 등록되어 있는 경우
            if (waveInfo[i] > 0)
            {
                for (int j = 0; j < _enemyInfoList.Count; j++)
                {
                    if (_enemyInfoList[j][(int)EnemyInfoType.ENEMYCOUNT] > 0)
                    {
                        _enemyInfoList[j][(int)EnemyInfoType.ENEMYCOUNT] += _enemyInfoList[j][(int)EnemyInfoType.ADDCOUNT];
                    }
                }
            }
        }
    }

    /// <summary>
    /// 웨이브에 등록된 몬스터 아이디 반환
    /// </summary>
    public int GetWaveEnemyID(int waveNumber, int idx) => _waveInfoList[waveNumber - 1][idx];
    public int MaxWave => _waveInfoList.Count;
    #endregion Wave

    #region Enemy
    /// <summary>
    /// 몬스터 정보 등록
    /// </summary>
    /// <param name="data"></param>
    void RegisterEnemyInfo()
    {
        _enemyInfoList.Add(new List<float>(_stageData.enemy1));
        _enemyInfoList.Add(new List<float>(_stageData.enemy2));
        _enemyInfoList.Add(new List<float>(_stageData.enemy3));
        _enemyInfoList.Add(new List<float>(_stageData.enemy4));
        _enemyInfoList.Add(new List<float>(_stageData.enemy5));
        _enemyInfoList.Add(new List<float>(_stageData.enemy6));

        var wave = _stageData.wave1;

        for (int i = 0; i < _stageData.wave1.Count; i++)
        {
            if (wave[i] > 0)
            {
                var data = DataManager.GetInstance.FindData(DataManager.KEY_ENEMY, wave[i]) as EnemyData;
                float atk = data.attackDamge;
                float hp = data.hp;
                float atkMultiple = GetATKMultiple(wave[i]);
                float hpMultiple = GetHPMultiple(wave[i]);
                //Debug.Log("Stage Number : " + StageManager.GetInstance._stageNumber);
                for (int j = 0; j < StageManager.GetInstance._stageNumber; j++)
                {
                    atk = atk * atkMultiple;
                    hp = hp * hpMultiple;
                }
                //Debug.Log("id : " + wave[i]);
                //Debug.Log("DMG : " + atk);
                //Debug.Log("HP : " + hp);
                _enemyDamageDic.Add(wave[i], atk);
                _enemyHPDic.Add(wave[i], hp);

            }
        }
    }

    public float GetATK(int id)
    {
        if(_enemyDamageDic.ContainsKey(id))
        {
            return _enemyDamageDic[id];
        }
        else
        {
            //Debug.LogError("해당 ID의 몬스터 정보가 없습니다.");
            return 0;
        }
    }

    public float GetHP(int id)
    {
        if(_enemyHPDic.ContainsKey(id))
        {
            return _enemyHPDic[id];
        }
        else
        {
            //Debug.LogError("해당 ID의 몬스터 정보가 없습니다.");
            return 0;
        }
    }

    /// <summary>
    /// 웨이브 최대 카운트 반환
    /// </summary>
    /// <returns></returns>
    public int GetEnemyMaxCount()
    {
        double count = 0;
        for (int i = 0; i < _enemyInfoList.Count; i++)
        {
            count += _enemyInfoList[i][(int)EnemyInfoType.ENEMYCOUNT];
        }
        return (int)count;
    }

    /// <summary>
    /// 몹 카운트 반환 
    /// </summary>
    public int GetEnemyCount(int idx) => (int)_enemyInfoList[idx][(int)EnemyInfoType.ENEMYCOUNT];

    /// <summary>
    /// 몹 공/체 배율 반환
    /// </summary>
    /// <param name="id">몬스터 ID</param>
    /// <returns></returns>
    public float GetATKMultiple(int id)
    {
        float[] multiple = new float[20] 
        { 
            1.102f,    
            1.102f,   
            1.107f,   
            1.1f,
            1.109f,
            1.109f,
            1.098f,
            1.091f,
            1.104f,
            1.098f,
            1.1f,
            1.102f,
            1.098f,
            1.101f,
            1.102f,
            1.1f,
            1.051f,
            1.102f,
            1.102f,
            1.102f
        };
        
        if(id < 100)
        {
            return multiple[id - 1];
        }
        else
        {
            int idx = ((id % 100) - 1) + 10;
            return multiple[idx];
        }

/*        for (int i = 0; i < _waveInfoList.Count; i++)
        {
            for (int j = 0; j < _waveInfoList[i].Length; j++)
            {
                if (_waveInfoList[i][j] == id)
                {
                    return _enemyInfoList[j][(int)EnemyInfoType.ADD_ATK];
                }
            }
        }
        return 1.0f;*/
    }

    public float GetHPMultiple(int id)
    {
        float[] multiple = new float[20]
        {
            1.153f,
            1.152f,   
            1.153f,
            1.149f,
            1.154f,   
            1.154f,   
            1.153f,
            1.152f,
            1.155f,
            1.115f,
            1.153f,
            1.154f,
            1.154f,
            1.154f,
            1.154f,
            1.154f,
            1.127f,
            1.154f,
            1.154f,
            1.154f,
        };

        if (id < 100)
        {
            return multiple[id - 1];
        }
        else
        {
            int idx = ((id % 100) - 1) + 10;
            return multiple[idx];
        }

/*        for (int i = 0; i < _waveInfoList.Count; i++)
        {
            for (int j = 0; j < _waveInfoList[i].Length; j++)
            {
                if (_waveInfoList[i][j] == id)
                {
                    return _enemyInfoList[j][(int)EnemyInfoType.ADD_HP];
                }
            }
        }
        return 1.0f;*/
    }

    #endregion Enemy

    #region Boss
    public float BossMultiplier => _bossMultiplier;
    public int BossCountMax => _bossId.Length;
    #endregion Boss
}
