using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected CharacterController cc;
    protected CharacterStats cs;
    public UNIT _unit = UNIT.Boat;
    public Type _type;
    bool _delayHit;

    public CharacterController GetController()
    {
        return cc;
    }
    public CharacterStats GetStats()
    {
        return cs;
    }
    public virtual void Init(MyData.SingleKeyData_Int data)
    {
        cc.IsBossOrEliteZombie = IsBossOrElite;
    }

    public virtual void Init(MyData.DoubleKeyData_Int data)
    {

    }
    public virtual void Init(MyData.DoubleKeyData_Int data, MyData.DoubleKeyData_Int data2, Info.PlayerInfo info, MyData.DoubleKeyData_Int data3 = null, MyData.DoubleKeyData_Int data4 = null, MyData.DoubleKeyData_Int data5 = null, MyData.DoubleKeyData_Int data6 = null)
    {

    }

    public virtual void Init(Info.PlayerInfo info, Dictionary<EItemList, int> dataIdDictionary)
    {

    }

    private readonly string[] enemyTag=  new string[] { CommonStaticDatas.TAG_ENEMY};
    private readonly string[] playerTag = new string[] { CommonStaticDatas.TAG_PLAYER };
    public virtual void TakeToDamage(UnitDamageData ttd, bool useAffect = true)
    {
        if (string.IsNullOrEmpty(ttd._hitEffectRes) == false)
        {// 히트 이펙트 명이 있을때 이펙트 생성
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, ttd._hitEffectRes, cc._trans.position, Quaternion.identity);
        }
        if(useAffect == false)
            return;
        //Debug.Log("TakeToDamage: 반사 데미지 처리에 대한 부분");
        //반사 데미지 치리
        //if (cc.ContainAffect(Affects.eAffects.ReflectionDamage))
        //{
        //    var reflectionAffect = cc.GetAffect(Affects.eAffects.ReflectionDamage);
        //    if (reflectionAffect != null)
        //    {
        //        Debug.Log("TakeToDamage: 반사 데미지 처리에 대한 부분");
        //        //float reflectionDmg = (ttd.damage * (reflectionAffect as ReflectionDamage).reflectionDamageValue);
        //        //var reflectionTdd = new UnitDamageData()
        //        //{
        //        //    attacker = transform,
        //        //    layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT),
        //        //    //targetTag = (ttd.targetTag == CommonStaticDatas.TAG_PLAYER ? CommonStaticDatas.TAG_ENEMY : CommonStaticDatas.TAG_PLAYER),
        //        //    targetTag = (tag == CommonStaticDatas.TAG_PLAYER ? enemyTag : playerTag),
        //        //    damage = reflectionDmg,
        //        //};
        //        //ttd.attacker.GetComponent<Interactable>().TakeToDamage(reflectionTdd);
        //    }
        //}
    }


    // 연출에서의 죽음
    public void ProductionDie()
    {
        if (cc._curState == eCharacterStates.Die) { return; }
        cc.SetState(eCharacterStates.Die);
    }

    public virtual void StayToDamage(UnitDamageData tdd, bool useAffect = true) { }

    public virtual bool IsBossOrElite()
    {
        return _unit == UNIT.Boss || _unit == UNIT.EliteEnemy;
    }

    internal bool IsBoss()
    {
        return _unit == UNIT.Boss;
    }

    public virtual bool IsSummonEnemy()
    {
        return _unit == UNIT.SummonEnemy;
    }
    public abstract void Ready();

    protected bool IsPhysiceDamageImmune()
    {
        return cc.GetStatusEffects._physicDamageImmune;
    }

    public CharacterController GetCharacterCtrl()
    {
        return cc;
    }

    private void OnParticleCollision(GameObject other)
    {
        //other.GetComponent<ColleagueFireEffect>().UnitDamageData;
        var bullet = other.GetComponent<Shot>();
        if (bullet == null)
            return;

        bool isSameTag = false;
        var tags = bullet.GetTdd().targetTag;
        for (int i = 0; i < tags.Length; i++)
        {
            if(tags[i] ==tag)
            {
                isSameTag = true;
                break;
            }
        }

        if (isSameTag == false)
            return;
        bullet.CreateHitObject(GetCharacterCtrl()._trans.position);
        TakeToDamage( bullet.GetTdd());
        // 샷것류때문에 여기서 삭제하면 안됨
        //bullet.DestroyThis();
    }

    internal bool IsTarget(UNIT[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == _unit)
                return true;
        }
        return false;
    }

    public bool GetDelayHit { get { return _delayHit; } set { _delayHit = value; } }
    public void DelayCheckTime(float delayTime)
    {
        StartCoroutine(DelayOnCheckTime(delayTime));
    }
    public IEnumerator DelayOnCheckTime(float delayTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(delayTime);
        //Debug.Log(this.name + "무적풀림!");
        _delayHit = false;
    }
}
