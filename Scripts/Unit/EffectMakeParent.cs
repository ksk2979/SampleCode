using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMakeParent : BaseAttack
{
    public string _bulletName = string.Empty;
    public Transform _fireReadyTrans = null;

    public bool IsWorld = false;
    public bool isForwardChange = true;


    public override void DoFire()
    {
        //var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, tdd.bullet, _fireReadyTrans.position, Quaternion.identity);
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _bulletName, _fireReadyTrans.position, Quaternion.identity);
        var goTrans = goShot.transform;
        if (IsWorld == false)
        {
            goTrans.SetParent(_fireReadyTrans);
            goTrans.localPosition = Vector3.zero;
            goTrans.localRotation = Quaternion.identity;
        }
        else
        {
            goTrans.SetParent(_fireReadyTrans);
            goTrans.localPosition = Vector3.zero;
            if (isForwardChange)
                goTrans.forward = _fireReadyTrans.forward;
            goTrans.SetParent(null);
        }

        var skill = goShot.GetComponent<Skill>();
        if (skill != null)
            skill.InitData(stats, tdd);
    }

    internal void DoFire(EnemyStats enemyStats, UnitDamageData UnitDamageData)
    {
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _bulletName, _fireReadyTrans.position, Quaternion.identity);
        var goTrans = goShot.transform;
        if (IsWorld == false)
        {
            goTrans.SetParent(_fireReadyTrans);
            goTrans.localPosition = Vector3.zero;
            goTrans.localRotation = Quaternion.identity;
        }
        else
        {
            goTrans.SetParent(_fireReadyTrans);
            goTrans.localPosition = Vector3.zero;
            if (isForwardChange)
                goTrans.forward = _fireReadyTrans.forward;
            goTrans.SetParent(null);
        }

        var skill = goShot.GetComponent<Skill>();
        if (skill != null)
        {
            skill.InitData(enemyStats, UnitDamageData);
            return;
        }
        var triggerStay = goShot.GetComponent<TriggerStayEnemyAttack>();
        if (triggerStay != null)
            triggerStay.InitData(enemyStats, UnitDamageData);
    }
}
