using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RootAction : MonoBehaviour
{
    [SerializeField] protected float _actionTime;
    [SerializeField] protected bool _flagSmooth;
    [SerializeField] protected float _frontDelayTime;
    [SerializeField] protected float _backDelayTime;
    

    protected float ChangeSmoothTime(float t)
    {
        return _flagSmooth == true ? Mathf.SmoothStep(0.0f, 1.0f, t) : t;
    }
    protected float GetActionTime()
    {
        return _actionTime + _frontDelayTime;
    }

    public float SetFrontDelayTime
    {
        set => _frontDelayTime = value;
    }

}
