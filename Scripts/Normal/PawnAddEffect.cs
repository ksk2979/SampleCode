using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PawnAddEffect : MonoBehaviour
{
    public enum EffectState
    {
        birth, // 출현
        death, // 죽음
        deathFinish, // 죽음 끝
        slow, // 슬로우
        mindcontrol, // 마인드컨트롤
        burrow, // 굴
        burrowing, // 잠복 (DisAppear)
        stickOut, // 스틱아웃
        Attack1, // 공격1
        Attack2, // 공격2
        Attack3, // 공격3
        casting, // 캐스팅
        Skill1,
        Skill2,
        casting2, // 캐스팅2
        casting3,
        Spawn, Appear, // 스폰, 등장
        Landing, Running, // 착지, 달리기
        Appear_Small, // 등장작음
        // 나오는 잘잘한 이펙트들 적용
        EffectApplied_0, EffectApplied_1, EffectApplied_2, EffectApplied_3, EffectApplied_4, EffectApplied_5,
        EffectApplied_6, EffectApplied_7, EffectApplied_8, EffectApplied_9, EffectApplied_10, EffectApplied_11,
    }

    public List<PawnDrama> drams;
    // 이펙트를 소환해주는 함수
    public void OnPawnAddEffect(EffectState state)
    {
        drams.Find(x => x.effectState == state).OnEffectDream();
    }

    public void OffPawnAddEffect(EffectState state)
    {
        drams.Find(x => x.effectState == state).OffEffectDream();
    }
}
[System.Serializable]
public class PawnDrama
{
    public PawnAddEffect.EffectState effectState;
    public string effectName = string.Empty;
    public bool addChild = false;
    public Transform perent;
    private GameObject obj;

    public void OnEffectDream()
    {
        obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, effectName, Vector3.zero, Quaternion.identity);
        obj.transform.position = perent.position;
        obj.transform.forward = perent.forward;
        if (addChild)
        {
            obj.transform.SetParent(perent);
        }
    }

    internal void OffEffectDream()
    {
        if (obj != null)
        {
            //obj.transform.SetParent(null);
            SimplePool.Despawn(obj);
        }
    }
}

