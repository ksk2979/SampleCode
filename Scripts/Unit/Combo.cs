using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour
{
    public int _maxCount = 3;
    public float _resetTime = 2f;
    private int _combo = 0;
    private bool _resetFlag = false;
    private float _flowTime = 0;
    
    void Update()
    {
        if (_resetFlag == false)
            return;

        _flowTime += Time.deltaTime;
        if(_resetTime < _flowTime)
        {
            _combo = 0;
            _flowTime = 0;
            _resetFlag = false;
        }
    }

    public int GetCombo()
    {
        _resetFlag = true;
        _flowTime = 0;
        if (_maxCount <= _combo)
            _combo = 0;
        return _combo++;
    }

    public int GetRandom()
    {
        return StandardFuncData.RandomRange(0, _maxCount - 1); // Random.Range(0, _maxCount);
    }



}
