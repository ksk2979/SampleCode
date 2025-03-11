using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
/// <summary>
/// 상태 이상은 1티어 - 넉백 넉다운 
///             2티어- 스턴 쇼크 프리즈 슬로우 카오스
/// 
/// 같은 티어는 교체 되고 
/// 하위티어는 상위 티어를 교체 할수없다
/// </summary>
public abstract class Affects
{
    public enum eAffects
    {
        None = 0,
        Slow = 1,
        Chaos = 2,
        Stun = 3,
        Shock = 4,
        Push = 5,
        DocterHeal = 6,
        ReflectionDamage = 7,
        DamageDecrease = 8, //데미지 감소
        PhysiceDamageImmune = 9,
        Splash = 10,
        SpeedIncrease = 11,
        DamageIncrease = 12,
        NoneTargeting = 13,
        Death = 14,
        Sommon = 15,
        Agrro = 16,
        TickDamage = 17,
    }
    public float duration = 0;
    /// <summary>
    /// 효과가 흐르는 시간 
    /// </summary>
    public float flowTime = 0;
    public int index = 0;
    public eAffects affectCate = Affects.eAffects.None;

    protected CharacterController c;

    public void SetController(CharacterController c)
    {
        this.c = c;
    }

    public abstract void Init();
    public virtual void AUpdate()
    {
        if (duration == -1) // 제한 시간 없이 적용
            return;
        flowTime += Time.deltaTime;
        if (duration < flowTime)
        {
            Finish();
            flowTime = 0;
            return;
        }
    }
    public abstract void Finish();
}

/// <summary>
/// 물리데미지 무시
/// </summary>
//public class PhysiceDamageImmune : Affects
//{
//    public Transform physiceDamageImunne;
//
//    /// <summary>
//    /// 물리데미지 무시 
//    /// </summary>
//    /// <param name="c"></param>
//    /// <param name="duration"></param>
//    public PhysiceDamageImmune(CharacterController c, float duration)
//    {
//        SetController(c);
//        this.duration = duration;
//        affectCate = eAffects.PhysiceDamageImmune;
//        physiceDamageImunne = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Reflection").transform;
//        physiceDamageImunne.SetParent(c._trans);
//        physiceDamageImunne.localPosition = Vector3.up;
//    }
//
//    public override void Init()
//    {
//    }
//
//    public override void AUpdate()
//    {
//        base.AUpdate();
//    }
//
//    public override void Finish()
//    {
//        physiceDamageImunne.SetParent(null);
//        SimplePool.Despawn(physiceDamageImunne.gameObject);
//        c.RemoveAtAffect(index);
//    }
//}

/// <summary>
/// 타겟팅 무시
/// </summary>
//public class NoneTargeting : Affects
//{
//    public Transform physiceDamageImunne;
//
//    /// <summary>
//    /// 물리데미지 무시
//    /// </summary>
//    /// <param name="c"></param>
//    /// <param name="duration"></param>
//    public NoneTargeting(CharacterController c, float duration)
//    {
//        SetController(c);
//        this.duration = duration;
//        affectCate = eAffects.NoneTargeting;
//        c.SetNoneTargetting();
//        physiceDamageImunne = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Reflection").transform;
//        physiceDamageImunne.SetParent(c._trans);
//        physiceDamageImunne.localPosition = Vector3.up;
//    }
//
//    public override void Init()
//    {
//    }
//
//    public override void AUpdate()
//    {
//        base.AUpdate();
//    }
//
//    public override void Finish()
//    {
//        c.SetTargetting();
//        physiceDamageImunne.SetParent(null);
//        SimplePool.Despawn(physiceDamageImunne.gameObject);
//        c.RemoveAtAffect(index);
//    }
//}


/// <summary>
/// 타겟팅 무시
/// </summary>
//public class Splash : Affects
//{
//    /// <summary>
//    /// 물리데미지 무시
//    /// </summary>
//    /// <param name="c"></param>
//    /// <param name="duration"></param>
//    public Splash(CharacterController c, float duration)
//    {
//        SetController(c);
//        this.duration = duration;
//        affectCate = eAffects.Splash;
//    }
//
//    public override void Init()
//    {
//    }
//
//    public override void AUpdate()
//    {
//        base.AUpdate();
//    }
//
//    public override void Finish()
//    {
//        c.RemoveAtAffect(index);
//    }
//}

/// <summary>
/// 데미지 증가
/// </summary>
//public class DamageIncrease : Affects
//{
//    public float ratio = 0;
//    public float per = 0;
//    private float tempDmg =0;
//    /// <summary>
//    /// 데미지 증가
//    /// </summary>
//    /// <param name="c"></param>
//    /// <param name="duration"></param>
//    /// <param name="ratio"></param>
//    public DamageIncrease(CharacterController c, float duration, float per)
//    {
//        SetController(c);
//        this.duration = duration;
//        this.per = per;
//        this.ratio = per/100;
//        affectCate = eAffects.DamageIncrease;
//    }
//
//    public override void Init()
//    {
//        tempDmg = c._characterStats.damage;
//        c._characterStats.damage = tempDmg * (1 + ratio);
//    }
//
//    public override void AUpdate()
//    {
//        base.AUpdate();
//    }
//
//    public override void Finish()
//    {
//        c._characterStats.damage = tempDmg;
//        c.RemoveAtAffect(index);
//    }
//}


