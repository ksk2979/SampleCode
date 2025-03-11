using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaBoundBallBome : ParabolaBoll
{
    public float totalTime = 5;
    private float flowTotalTime = 0;

    public Transform target;
    public float duration =2 ;
    public float height = 5;
    public bool bshot = false;

    private int boundCount = 0;
    private int boundLimit = 5;


    internal override void Shot(CharacterStats cStat, ParabolaData parabolaData, UnitDamageData tdd)
    {
        _flowTime = 0;
        _time = 0;
        flowTotalTime = 0;
        this.stat = cStat;
        this.parabolaData = parabolaData.Clone();
        this.tdd = tdd;
        boundCount = 0;
        bshot = true;
    }
    [ContextMenu("TestShot")]
    public void TestShot()
    {
        this.parabolaData = new ParabolaData()
        {
            attackDist = 10,
            _parabolaBoomDist = 1,
            _parabolaThrowDuration = duration,
            _parabolaThrowHeight = height,
            _targetPos = target.position,
            _origin = _trans.position
        };
        bshot = true;
    }

    void FixedUpdate()
    {
        if (bshot == false)
            return;
        _time += Time.fixedDeltaTime;
        _flowTime += Time.fixedDeltaTime;

        _flowTime = _flowTime % parabolaData._parabolaThrowDuration;
        var destPos = parabolaData._targetPos + (Vector3.down * 1.3f) + (_trans.forward * 0.5f);
        _trans.position = MathParabola.Parabola(parabolaData._origin, destPos, parabolaData._parabolaThrowHeight, _flowTime / parabolaData._parabolaThrowDuration);

        if (boundLimit < boundCount)
        {
            Debug.Log("Bound Over Destroy");
            DestroyThis();
            bshot = false;
        }

        flowTotalTime += Time.fixedDeltaTime;
        if (totalTime < flowTotalTime)
        {
            Debug.Log("Time Up Destroy");
            DestroyThis();
            bshot = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            other.GetComponent<Interactable>().TakeToDamage(tdd);
            DestroyThis();
            bshot = false;
            return;
        }

        if (other.CompareTag(CommonStaticDatas.TAG_FLOOR))
        {
            var dist = Vector3.Distance(parabolaData._origin, parabolaData._targetPos + (Vector3.down * 1.3f) + (_trans.forward * 0.5f));
            var originPos = _trans.position;
            originPos.y = 0;
            parabolaData._targetPos.y = 0;
            parabolaData._targetPos = parabolaData._targetPos + (parabolaData._targetPos - originPos).normalized * (dist * 0.5f);
            parabolaData._origin = _trans.position;
            _flowTime = 0;
            boundCount++;
        }
    }
}
