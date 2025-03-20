using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SceneStaticObj<StageManager>
{
    int _lifePlayer = 4;
    int _clearCount = 0;
    [SerializeField] RoadWaveScript[] _roadWaveArr;

    private void Start()
    {
        for (int i = 0; i < _roadWaveArr.Length; ++i)
        {
            _roadWaveArr[i].Init(this);
        }
    }
    public void SpellCreate()
    {
        _roadWaveArr[0].AddSpellBtn();
    }
    public void BountyCreate(int index)
    {
        _roadWaveArr[0].EnemyCreate(index);
    }
    public void ShinusCreate(int index)
    {
        _roadWaveArr[0].ShinusCreate(index);
    }

    public Player GetPlayer { get { return _roadWaveArr[0].GetPlayer; } }
    public int LifePlayer { get { return _lifePlayer; } set { _lifePlayer = value; } }
    public int ClearCount {  get { return _clearCount; } set { _clearCount = value; } }
}
