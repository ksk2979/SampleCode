using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro.Examples;

public class Structs
{

}


public class UnitDamageData
{
    // 일반용
    public Transform attacker; // 데미지를 준 대상
    public float damage = 0.0f; // 데미지 
    public string[] targetTag; // 맞을 적 태그
    public int layerMask; // 맞을 레이어
    //public string bullet;

    // 특수 효과용
    public string _hitEffectRes = string.Empty; // 맞았을때 나오는 이펙트 (폭팔인것은 여기다가 이름을 줌)
    // 유저 어빌리티 처리
    public PlayerAbility _ability;

    public bool _isHeadShot = false; // 해드샷 효과를 낼것인가?
    public bool _isHealing = false; // 힐인지 판별
    public float explosionRange = 0.0f;
    public float decreaseDamageRatio = 0.0f;

    //public float dmgTotalRatio
    //{
    //    get
    //    {
    //        float increaseRatio = 1;
    //        if (attacker != null)
    //        {
    //            var c = attacker.GetComponent<CharacterController>();
    //            if (c == null)
    //                return 1 - decreaseDamageRatio;
    //            var di = (c.GetAffect(Affects.eAffects.DamageIncrease) as DamageIncrease);
    //            increaseRatio = di == null ? 0 : di.ratio;
    //        }
    //        return 1 - decreaseDamageRatio + increaseRatio;
    //    }
    //}

    // 다른 UnitDamageData 객체의 데이터를 복사
    public void CopyFrom(UnitDamageData source)
    {
        attacker = source.attacker;
        damage = source.damage;
        targetTag = (string[])source.targetTag.Clone();
        layerMask = source.layerMask;
        _hitEffectRes = source._hitEffectRes;
        _isHeadShot = source._isHeadShot;
        _isHealing = source._isHealing;
        explosionRange = source.explosionRange;
        decreaseDamageRatio = source.decreaseDamageRatio;
    }
}


public struct ParabolaData
{
    // 포물선 용
    public Vector3 _origin;
    public Vector3 _targetPos;
    public float attackDist;
    public float _parabolaBoomDist;
    public float _parabolaThrowDuration;
    public float _parabolaThrowHeight;

    internal void InitData()
    {

        //_characterStats = null;
        _origin = Vector3.zero;
        _targetPos = Vector3.zero;
        _parabolaBoomDist = 0;
        _parabolaThrowDuration = 0;
        _parabolaThrowHeight = 0;
    }

    public ParabolaData Clone()
    {
        var para = new ParabolaData();
        para._origin = _origin;
        para._targetPos = _targetPos;
        para.attackDist = attackDist;
        para._parabolaBoomDist = _parabolaBoomDist;
        para._parabolaThrowDuration = _parabolaThrowDuration;
        para._parabolaThrowHeight = _parabolaThrowHeight;
        return para;
    }
}
//[System.Serializable]
//public class FirePointSetter
//{
//    public enum eFireSetterType
//    {
//        ForwardOne = 0,
//        ForwardTwo,
//        ForwardThree,
//        ObliqueOne,
//        ObliqueTwo,
//        lateralOne,
//        lateralTwo,
//        BackOne,
//        BackTwo,
//    }

//    public eFireSetterType _type;
//    public FirePoint[] _firePoints;

//    internal void DoFire()
//    {
//        switch (_type)
//        {
//            case eFireSetterType.ForwardOne: // 기본이니깐 검사하지 않는다가 아니라 검사 해야한다.
//                if (AbilityManager.Current.IsAdditionalShot1())
//                    return;
//                break;
//            case eFireSetterType.ForwardTwo:
//                if (AbilityManager.Current.IsAdditionalShot1Two() == false)
//                    return;
//                break;
//            case eFireSetterType.ForwardThree:
//                if (AbilityManager.Current.IsAdditionalShot1Three() == false)
//                    return;
//                break;


//            case eFireSetterType.ObliqueOne:
//                if (AbilityManager.Current.IsAdditionalShot2One() == false)
//                    return;
//                break;
//            case eFireSetterType.ObliqueTwo:
//                if (AbilityManager.Current.IsAdditionalShot2Two() == false)
//                    return;
//                break;

//            case eFireSetterType.lateralOne:
//                if (AbilityManager.Current.IsAdditionalShot3One() == false)
//                    return;
//                break;
//            case eFireSetterType.lateralTwo:
//                if (AbilityManager.Current.IsAdditionalShot3Two() == false)
//                    return;
//                break;

//            case eFireSetterType.BackOne:
//                if (AbilityManager.Current.IsAdditionalShot4One() == false)
//                    return;
//                break;
//            case eFireSetterType.BackTwo:
//                if (AbilityManager.Current.IsAdditionalShot4Two() == false)
//                    return;
//                break;
//        }

//        foreach (var item in _firePoints)
//        {
//            item.DoFire();
//        }
//    }

//    internal void InitData(CharacterController characterStates)
//    {
//        foreach (var item in _firePoints)
//        {
//            item.InitData(characterStates);
//        }
//    }
//}

