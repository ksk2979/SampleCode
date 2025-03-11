using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaBoll : MonoBehaviour
{
    protected float _flowTime = 0;
    protected float _time = 0;
    protected Transform _trans = null;
    protected UnitDamageData tdd;
    protected CharacterStats stat;
    protected ParabolaData parabolaData;
    public string FX_Enemy_Projectile_01_HIt = "FX_Enemy_Projectile_01_HIt";

    private void Start()
    {
        _trans = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;
        _flowTime += Time.fixedDeltaTime;
        _flowTime = _flowTime % parabolaData._parabolaThrowDuration;
        var destPos = parabolaData._targetPos + (Vector3.down * 2f) + (_trans.forward * 0.5f);
        _trans.position = MathParabola.Parabola(parabolaData._origin, destPos, parabolaData._parabolaThrowHeight, _flowTime / parabolaData._parabolaThrowDuration);

        if (parabolaData._parabolaThrowDuration + 1 < _time) 
        {
            //Debug.Log("Time Up Destroy");
            DestroyThis();
            //Debug.Log("_time");
        }

        if(Vector3.Distance(_trans.position , destPos) < 0.01f)
        {
            //Debug.Log("Distance Destroy");
            DestroyThis();
            //Debug.Log("0.01f");
        }

        //if(_UnitDamageData._shotSpeed * 0.5f < _flowTime)
        //    _transform.LookAt(_UnitDamageData._targetPos);
        //else
        //    _transform.LookAt(_UnitDamageData._targetPos + (Vector3.up * 2));
    }

    internal virtual void Shot(CharacterStats cStat, ParabolaData parabolaData, UnitDamageData tdd)
    {
        _flowTime = 0;
        _time = 0;
        this.stat = cStat;
        this.parabolaData = parabolaData;
        this.tdd = tdd;
    }

    //internal void Shot(UnitDamageData UnitDamageData)
    //{
    //    _flowTime = 0;
    //    _time = 0;
    //    _UnitDamageData = UnitDamageData;
    //}

    private void OnTriggerEnter(Collider other)
    {
        // 데미지 주기
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            other.GetComponent<Interactable>().TakeToDamage(tdd);
            DestroyThis();
            return;
        }

        if (other.CompareTag(CommonStaticDatas.TAG_FLOOR))
        {
            DestroyThis();
        }
    }

    public virtual void DestroyThis()
    {
        if (gameObject.activeSelf == false)
            return;
        if (string.IsNullOrEmpty(FX_Enemy_Projectile_01_HIt) == false)
        {
            var pos = _trans.position;
            //pos.y = 0;
            var explosionObj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Enemy_Projectile_01_HIt, pos, Quaternion.identity);
            var bomExplosion = explosionObj.GetComponent<BomExplosionTiggerStay>();
            if(bomExplosion!= null)
                bomExplosion.InitData(stat, tdd);
        }
        //SimplePool.Spawn(_hitEffect, _transform.position, Quaternion.identity);
        SimplePool.Despawn(gameObject);
    }
}
