using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterAttackEvent : BaseAttack
{
    public myEventIntera m_event;
    public myEvent m_destroy;

    private void OnTriggerEnter(Collider other)
    {
        // 데미지 주기
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            m_event.Invoke(transform, other.GetComponent<Interactable>());
        }

        if (other.CompareTag(CommonStaticDatas.TAG_WALL) || other.CompareTag(CommonStaticDatas.TAG_OBSTACLE))
        {
            m_destroy.Invoke();
        }
    }
}
